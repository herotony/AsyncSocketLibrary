using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketLibrary.Common.Client
{
	public class SocketClient
	{
		private static ConcurrentDictionary<int,byte[]> dictResult = new ConcurrentDictionary<int, byte[]>();
		private static ConcurrentQueue<MessageInfo> SendingMessages = new ConcurrentQueue<MessageInfo> ();

		//相当于83886080 个请求，10兆空间处理8千万请求你/天，不是并发哦，是一天一个前端的累计访问量
		private static BitArray arrTokenId = new BitArray(new byte[10*1024*1024]);
		private static int msgTokenId = 0;
		private static int timeOutByMS = 1000;//超时设置，单位毫秒

		//定时清理参数
		private static int ClearTime = 3;
		private static bool IsAlreadyClear = false;

		private static int concurrentCount = 0;

		private static BufferManager bufferManager;

		//开启两个线程，一个负责清零，一个负责发送
		private static Thread threadClear;
		private static Thread threadSending;

		private static SocketClientSettings _settings;

		private static SocketAsyncEventArgsPool poolOfRecSendEventArgs;
		private static SocketAsyncEventArgsPool poolOfConnectEventArgs;
		private static ProcessClientSocketEventManager processManager;

		static SocketClient(){

			Init ();
		}

		private static void Init(){

			DateTime start = DateTime.Now;

			SocketClientSettings settings = null;

			_settings = settings;

			bufferManager = new BufferManager (_settings.BufferSize*_settings.OpsToPreAllocate*_settings.NumberOfSaeaForRecSend,_settings.BufferSize*_settings.OpsToPreAllocate);
			bufferManager.InitBuffer ();

			poolOfConnectEventArgs = new SocketAsyncEventArgsPool (_settings.MaxConnectOps);
			poolOfRecSendEventArgs = new SocketAsyncEventArgsPool (_settings.NumberOfSaeaForRecSend);

			processManager = new ProcessClientSocketEventManager (poolOfConnectEventArgs, poolOfRecSendEventArgs,_settings.MaxConnectOps,_settings.BufferSize,_settings.NumberOfMessagesPerConnection,_settings.ReceivePrefixLength);

			for (int i = 0; i < _settings.MaxConnectOps; i++) {

				SocketAsyncEventArgs connectEventArg = new SocketAsyncEventArgs();            

				connectEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(processManager.IO_Completed);

				ConnectOpUserToken theConnectingToken = new ConnectOpUserToken(poolOfConnectEventArgs.AssignTokenId() + 10000);
				connectEventArg.UserToken = theConnectingToken;

				poolOfConnectEventArgs.Push (connectEventArg);
			}
							 

			for (int i = 0; i < _settings.NumberOfSaeaForRecSend; i++) {			

				//Allocate the SocketAsyncEventArgs object.
				SocketAsyncEventArgs eventArgObjectForPool = new SocketAsyncEventArgs();

				// assign a byte buffer from the buffer block to 
				//this particular SocketAsyncEventArg object
				bufferManager.SetBuffer(eventArgObjectForPool);

				eventArgObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(processManager.IO_Completed);

				ClientDataHoldingUserToken receiveSendToken = new ClientDataHoldingUserToken(eventArgObjectForPool,eventArgObjectForPool.Offset, eventArgObjectForPool.Offset + _settings.BufferSize, _settings.ReceivePrefixLength, _settings.SendPrefixLength, (poolOfRecSendEventArgs.AssignTokenId() + 1000000));

				//Create an object that we can write data to, and remove as an object
				//from the UserToken, if we wish.
				receiveSendToken.CreateNewSendDataHolder();

				eventArgObjectForPool.UserToken = receiveSendToken;

				poolOfRecSendEventArgs.Push(eventArgObjectForPool);
			}
				
			processManager.PushResultCallback = TryAdd;

			threadClear = new Thread (new ThreadStart (RunClear));
			threadClear.IsBackground = true;//进程结束则直接干掉本线程即可，无需等待!
			threadClear.Start ();

			threadSending = new Thread (new ThreadStart (SendMessageOverAndOver));
			threadSending.IsBackground = true;
			threadSending.Start ();

			LogManager.Log (string.Format("SocketClient Fist Invoke Complete! ConsumeTime:{0} ms",DateTime.Now.Subtract(start).TotalMilliseconds));
		}
			

		private static void SendMessageOverAndOver(){

			while (true) {

				try{

					bool dequeueOk = false;

					MessageInfo firstMsgInfo;
					bool isFirstIn = true;

					dequeueOk = SendingMessages.TryDequeue(out firstMsgInfo);

					List<MessageInfo> listSend = new List<MessageInfo>();

					MessageInfo msgInfo = null;

					while(dequeueOk){
					
						if( isFirstIn && firstMsgInfo!=null){

							isFirstIn = false;

							//..转交saea
							listSend.Add(firstMsgInfo);


						}else{

							if(msgInfo!=null){

								//..转交saea
								listSend.Add(msgInfo);
							}
						}

						if(listSend.Count>0){

							if(listSend.Count<_settings.NumberOfMessagesPerConnection){

								if(SendingMessages.Count.Equals(0)){

									processManager.SendMessage(listSend,_settings.ServerEndPoint);
									listSend = new List<MessageInfo>();
								}
							}else{

								processManager.SendMessage(listSend,_settings.ServerEndPoint);
								listSend = new List<MessageInfo>();
							}
						}
							
						dequeueOk = SendingMessages.TryDequeue(out msgInfo);
					}						
						
				}catch(InvalidOperationException e){

					LogManager.Log (string.Format ("SendingMessages queue maybe empty(count:{0})", SendingMessages.Count), e);

				}catch(Exception otherError){

					LogManager.Log ("SendMessageOverAndOver occur Error!", otherError);
				}

				Thread.Sleep (1);
			}
		}

		//唯一对外发送接口
		public static byte[] PushSendDataToPool(byte[] sendData,ref string message){		

			int _tokenId = GetNewTokenId ();

			DateTime start = DateTime.Now;

			MessageInfo msgInfo = new MessageInfo ();
			msgInfo.MessageTokenId = _tokenId;
			msgInfo.Content = sendData;

			//数据写入数据池
			SendingMessages.Enqueue (msgInfo);

			byte[] retData;
			bool isTimeOut = false;

			//wait result...
			while (!IsIOComplete (_tokenId)) {

				if (DateTime.Now.Subtract (start).TotalMilliseconds > timeOutByMS) {

					isTimeOut = true;
					break;
				}

				Thread.Sleep (10);
			}


			if (isTimeOut) {

				message = string.Format ("Try get retdata timeout on MsgTokenId:{0}! consumetime:{1} ms", _tokenId, DateTime.Now.Subtract (start).TotalMilliseconds);
				return null;
			}

			if (!TryGetResult (_tokenId, out retData))
				message = string.Format ("Try get retdata failed on MsgTokenId:{0}! consumetime:{1} ms", _tokenId, DateTime.Now.Subtract (start).TotalMilliseconds);
			else
				message = string.Format ("get retdata sucessfully! MsgTokenId:{0} reddata length:{1} consumetime:{2} ms", _tokenId, retData.Length, DateTime.Now.Subtract (start).TotalMilliseconds);

			return retData;
		}



		//用于凌晨某时刻清零...
		private static void ClearAllTokenId(){

			msgTokenId = 0;
			arrTokenId.SetAll (false);
			dictResult.Clear ();

			LogManager.Log ("ClearAllTokenId Complete Successfully!");
		}

		//凌晨清零!
		private static void RunClear(){

			int retryCount = 0;

			while (true) {

				if (IsAlreadyClear) {

					if (!DateTime.Now.Hour.Equals (ClearTime))
						IsAlreadyClear = false;
				}

				if (!IsAlreadyClear  && DateTime.Now.Hour.Equals (ClearTime)) {

					if (concurrentCount < 3 || retryCount > 10) {
														
						//清理							
						ClearAllTokenId ();
						IsAlreadyClear = true;
						retryCount = 0;
					} else
						retryCount++;
				}
					
				Thread.Sleep (10000);
			}
		}

		private static bool IsIOComplete(int tokenId){

			return arrTokenId [tokenId];
		}


		internal static int GetNewTokenId(){

			return Interlocked.Increment (ref msgTokenId);
		}
			
		//如果false,考虑是否内存泄露!这里需记日志!
		private static bool TryGetResult(int tokenId,out byte[] retData){		

			bool tryRemove =  dictResult.TryRemove (tokenId, out retData);

			if (!tryRemove) {

				//留日志即可...
				LogManager.Log (string.Format ("TryRemove MsgTokenId:{0} failed!", tokenId));
			}

			return retData!=null && retData.Length>0;
		}

		//用于接收返回结果
		internal static bool TryAdd(int tokenId,byte[] retData){

			byte[] copyData = new byte[retData.Length];

			if (retData.Length > 0)
				Buffer.BlockCopy (retData, 0, copyData, 0, retData.Length);

			arrTokenId [tokenId] = true;

			bool addRet =  dictResult.TryAdd (tokenId, copyData);

			if (!addRet)
				LogManager.Log (string.Empty, new Exception (string.Format ("TryAdd retData[{0} byte] Failed on MsgTokenId:{1}", copyData.Length,tokenId)));

			return addRet;
		}
	}
}

