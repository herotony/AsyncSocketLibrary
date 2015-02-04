﻿using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace TestClient
{
	class MainClass
	{
		private static object lockObj = new object ();
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Console.ReadKey ();

			Stopwatch sw = new Stopwatch ();

			sw.Start ();
			int testCount = 10000;
			int successCount = 0;
			int failedCount = 0;

			Task[] testTasks = new Task[testCount];

			StringBuilder sb = new StringBuilder ();

			for (int i = 0; i < testCount; i++) {
			
				testTasks[i] = Task.Factory.StartNew (() => {

					try{

						byte[] sendData = Encoding.UTF8.GetBytes(string.Format("id:{0}",Task.CurrentId));
						string message = string.Empty;

						byte[] result = AsyncSocketLibrary.Common.Client.SocketClient.PushSendDataToPool (sendData, ref message); 

						if(result==null){
							Console.WriteLine("failed on message:{0}",message);
							Interlocked.Increment(ref failedCount);
						}
						else{
							Interlocked.Increment(ref successCount);

							lock(lockObj){
								sb.AppendFormat("\r\n{0}",Encoding.UTF8.GetString(result));
							}
						}


					}catch(Exception e){

						AsyncSocketLibrary.Common.LogManager.Log(string.Format("error on taskId:{0}",Task.CurrentId),e);
					}


				});					
			}

			Task.WaitAll (testTasks);

			sb.Append (string.Format("\r\n总耗时:{0} ms 成功:{1} 条，失败:{2}条", sw.ElapsedMilliseconds,successCount,failedCount));
			sb.Append(string.Format("\r\n 最大并发：recsend:{0}  conncetop:{1}",AsyncSocketLibrary.Common.Client.SocketClient.GetMaxConcurrentCount(true),AsyncSocketLibrary.Common.Client.SocketClient.GetMaxConcurrentCount(false)));

			AsyncSocketLibrary.Common.LogManager.Log (sb.ToString ());

			Console.ReadKey ();
		}
	}
}
