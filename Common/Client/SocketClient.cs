using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace AsyncSocketLibrary.Common.Client
{
	public class SocketClient
	{
		//缓存返回数据
		private static ConcurrentDictionary<int,byte[]> dictResult = new ConcurrentDictionary<int, byte[]>();

		//关键是为了记录发送到返回的起始时间点，便于查看耗时
		private static ConcurrentQueue<MessageInfo> SendingMessages = new ConcurrentQueue<MessageInfo> ();

		//相当于83886080 个请求，10兆空间处理8千万请求你/天，不是并发哦，是一天一个前端的累计访问量
		private static BitArray arrTokenId = new BitArray(new byte[10*1024*1024]);

		private static int msgTokenId = 0;
		private static int timeOutByMS = 1000;//超时设置，单位毫秒

		//定时清理参数
		private static int ClearTime = 3;
		private static bool IsAlreadyClear = false;

		//查询并发
		private static int concurrentCount = 0;

		//核心部分！
		private static BufferManager bufferManager;

		//开启两个线程，一个负责清零，一个负责发送
		private static Thread threadClear;
		private static Thread threadSending;

		//最终要修改为读取配置
		private static SocketClientSettings _settings;

		private static SocketAsyncEventArgsPool poolOfRecSendEventArgs;
		private static SocketAsyncEventArgsPool poolOfConnectEventArgs;

		private static ProcessClientSocketEventManager processManager;

		static SocketClient(){		

			Init ();
		}

		private static void Init(){

			Stopwatch sw = new Stopwatch ();

			sw.Start ();

			ConfigManager cfm = new ConfigManager ();

			Dictionary<string,string> dictCfg = cfm.GetOriginalSettingInfo ();

			ParseSettingInfo pInfo = new ParseSettingInfo (dictCfg);

			//未来改成读配置文件！
			SocketClientSettings settings = pInfo.GetSocketClientSetting ();

			_settings = settings;

			bufferManager = new BufferManager (_settings.BufferSize*_settings.OpsToPreAllocate*_settings.NumberOfSaeaForRecSend,_settings.BufferSize*_settings.OpsToPreAllocate);
			bufferManager.InitBuffer ();

			//用于负责建立连接的saea，无关buffermanager，10个足够！这部分实际在processManager中可以动态增加
			poolOfConnectEventArgs = new SocketAsyncEventArgsPool (_settings.MaxConnectOps);

			//用于负责在建立好的连接上传输数据，涉及buffermanager，目前测试100～200个足够！这部分目前不支持动态增加！
			//因其buffermanager是事先分配好的一大块连续的固定内存区域，强烈建议不再更改，需要做好的就是事先的大小评估。
			poolOfRecSendEventArgs = new SocketAsyncEventArgsPool (_settings.NumberOfSaeaForRecSend);

			//实际负责处理相关传输数据的关键核心类
			processManager = new ProcessClientSocketEventManager (poolOfConnectEventArgs, poolOfRecSendEventArgs,_settings.MaxConnectOps,_settings.BufferSize,_settings.NumberOfMessagesPerConnection,_settings.ReceivePrefixLength);

			for (int i = 0; i < _settings.MaxConnectOps; i++) {

				SocketAsyncEventArgs connectEventArg = new SocketAsyncEventArgs();            

				connectEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(processManager.IO_Completed);

				//关键负责标识saea和更关键的传输待发送的数据给传输用的saea。
				ConnectOpUserToken theConnectingToken = new ConnectOpUserToken(poolOfConnectEventArgs.AssignTokenId() + 10000);
				connectEventArg.UserToken = theConnectingToken;

				poolOfConnectEventArgs.Push (connectEventArg);
			}
							 

			for (int i = 0; i < _settings.NumberOfSaeaForRecSend; i++) {			
			
				SocketAsyncEventArgs eventArgObjectForPool = new SocketAsyncEventArgs();

				//事先为每个saea分配固定不变的内存位置！
				bufferManager.SetBuffer(eventArgObjectForPool);

				eventArgObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(processManager.IO_Completed);

				ClientDataHoldingUserToken receiveSendToken = new ClientDataHoldingUserToken(eventArgObjectForPool.Offset, eventArgObjectForPool.Offset + _settings.BufferSize, _settings.ReceivePrefixLength, _settings.SendPrefixLength, (poolOfRecSendEventArgs.AssignTokenId() + 1000000));

				//用于传递待发送的数据，一旦完成发送可以重新new一个。
				receiveSendToken.CreateNewSendDataHolder();

				eventArgObjectForPool.UserToken = receiveSendToken;

				poolOfRecSendEventArgs.Push(eventArgObjectForPool);
			}
				
			//用于保存异步处理完后返回的结果数据。
			processManager.PushResultCallback = TryAdd;

			threadClear = new Thread (new ThreadStart (RunClear));
			threadClear.IsBackground = true;//进程结束则直接干掉本线程即可，无需等待!
			threadClear.Start ();

			threadSending = new Thread (new ThreadStart (SendMessageOverAndOver));
			threadSending.IsBackground = true;
			threadSending.Start ();

			sw.Stop ();

			LogManager.Log (string.Format("SocketClient Init by FirstInvoke Completed! ConsumeTime:{0} ms",sw.ElapsedMilliseconds));
		}
			
		private static void SendMessageOverAndOver(){

			List<MessageInfo> listSend = new List<MessageInfo>();

			bool dequeueOk = false;

			MessageInfo firstMsgInfo = null;
			MessageInfo msgInfo = null;

			while (true) {

				try{

					//确保长连接多发时，不会因为小于多发的数量而此时队列没数据了导致的发送丢失
					if(listSend.Count>0){

						processManager.SendMessage(listSend,_settings.ServerEndPoint);
						listSend = new List<MessageInfo>();
					}
										
					bool isFirstInPerLoop = true;

					dequeueOk = SendingMessages.TryDequeue(out firstMsgInfo);

					while(dequeueOk){
					
						if( isFirstInPerLoop && firstMsgInfo!=null){

							isFirstInPerLoop = false;

							firstMsgInfo.startTime=DateTime.Now;

							//待转交saea
							listSend.Add(firstMsgInfo);


						}else{

							if(msgInfo!=null){

								msgInfo.startTime=DateTime.Now;

								//待转交saea
								listSend.Add(msgInfo);
							}
						}

						if(listSend.Count>0){

							if(listSend.Count<_settings.NumberOfMessagesPerConnection){

								//否则，继续追加待发送数据直至达到一个长连接发送的配置量
								if(SendingMessages.IsEmpty){

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

			Stopwatch sw = new Stopwatch ();

			sw.Start ();

			int _tokenId = GetNewTokenId ();

			MessageInfo msgInfo = new MessageInfo ();

			msgInfo.MessageTokenId = _tokenId;
			msgInfo.Content = sendData;

			//数据写入数据池
			SendingMessages.Enqueue (msgInfo);

			byte[] retData;
			bool isTimeOut = false;

			//wait result...
			while (!IsIOComplete (_tokenId)) {

				if (sw.ElapsedMilliseconds > timeOutByMS) {

					sw.Stop ();
					isTimeOut = true;
					break;
				}

				Thread.Sleep (1);
			}


			if (isTimeOut) {

				message = string.Format ("Try get retdata timeout on MsgTokenId:{0}! consumetime:{1} ms", _tokenId, sw.ElapsedMilliseconds);
				return null;
			}

			if (!TryGetResult (_tokenId, out retData))
				message = string.Format ("Try get retdata from dictionary failed on MsgTokenId:{0}! consumetime:{1} ms", _tokenId, sw.ElapsedMilliseconds);
			else
				message = string.Format ("get retdata sucessfully! MsgTokenId:{0} reddata length:{1} consumetime:{2} ms", _tokenId, retData.Length, sw.ElapsedMilliseconds);

			sw.Stop ();

			return retData;
		}

		public static int GetMaxConcurrentCount(bool getRecSend){

			return getRecSend ? processManager.maxConcurrentRecSendCount : processManager.maxConcurrentConnectOpCount;
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

