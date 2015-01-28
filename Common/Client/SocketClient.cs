using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace AsyncSocketLibrary
{
	public class SocketClient
	{
		private static ConcurrentDictionary<int,byte[]> dictResult = new ConcurrentDictionary<int, byte[]>();

		//相当于83886080 个请求，10兆空间处理8千万请求你/天
		private static BitArray arrTokenId = new BitArray(new byte[10*1024*1024]);
		private static int tokenId = 0;
		private static int timeOutByMS = 1000;//超时设置，单位毫秒
		private static int ClearTime = 3;
		private static bool IsAlreadyClear = false;
		private static int concurrentCount = 0;

		private static Thread threadClear;

		static SocketClient ()
		{
		
			threadClear = new Thread (new ThreadStart (RunClear));
			threadClear.IsBackground = true;//进程结束则直接干掉本线程即可，无需等待!
			threadClear.Start ();
		}

		//唯一对外发送接口
		public static byte[] PushSendDataToPool(byte[] sendData,ref string message){

			int _tokenId = GetNewTokenId ();

			DateTime start = DateTime.Now;

			//数据写入数据池

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

				message = string.Format ("Try get retdata timeout on tokenid:{0}! consumetime:{1} ms", _tokenId, DateTime.Now.Subtract (start).TotalMilliseconds);
				return null;
			}

			if (!TryGetResult (_tokenId, out retData))
				message = string.Format ("Try get retdata failed on tokenid:{0}! consumetime:{1} ms", _tokenId, DateTime.Now.Subtract (start).TotalMilliseconds);
			else
				message = string.Format ("get retdata sucessfully! tokenid:{0} reddata length:{1} consumetime:{2} ms", _tokenId, retData.Length, DateTime.Now.Subtract (start).TotalMilliseconds);

			return retData;
		}

		//用于凌晨某时刻清零...
		private static void ClearAllTokenId(){

			tokenId = 0;
			arrTokenId.SetAll (false);
			dictResult.Clear ();
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

			return Interlocked.Increment (ref tokenId);
		}
			
		//如果false,考虑是否内存泄露!这里需记日志!
		private static bool TryGetResult(int tokenId,out byte[] retData){		

			bool tryRemove =  dictResult.TryRemove (tokenId, out retData);

			if (!tryRemove) {

				//留日志即可...
			}

			return retData!=null && retData.Length>0;
		}

		//用于接收返回结果
		internal static bool TryAdd(int tokenId,byte[] retData){

			byte[] copyData = new byte[retData.Length];

			if (retData.Length > 0)
				Buffer.BlockCopy (retData, 0, copyData, 0, retData.Length);

			arrTokenId [tokenId] = true;

			return dictResult.TryAdd (tokenId, copyData);
		}
	}
}

