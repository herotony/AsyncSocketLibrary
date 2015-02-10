using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.Collections.Generic;
using AsyncSocketLibrary.Common;

namespace TestClient
{
	class MainClass
	{
		private static object lockObj = new object ();
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Console.ReadKey ();

			ConfigManager cfm = new ConfigManager ();
			cfm.ConfigChanged += new ConfigManager.OnConfigChangedEventHandler (GetNewConfig);

			Console.WriteLine ("cfm init over");

			Console.ReadKey ();

			Stopwatch sw = new Stopwatch ();

			sw.Start ();
			int testCount = 1000;
			int successCount = 0;
			int failedCount = 0;

			Task[] testTasks = new Task[testCount];

			StringBuilder sb = new StringBuilder ();

			for (int i = 0; i < testCount; i++) {
			
				testTasks[i] = Task.Factory.StartNew (() => {

					try{

						byte[] sendData = Encoding.UTF8.GetBytes(string.Format("id测试:{0} 靠谱!",Task.CurrentId));
						string message = string.Empty;

						byte[] result = AsyncSocketLibrary.Common.Client.SocketClient.PushSendDataToPool (sendData, ref message); 

						if(result==null){
							Console.WriteLine("failed on message:{0}",message);
							Interlocked.Increment(ref failedCount);
						}
						else{
							Interlocked.Increment(ref successCount);

							lock(lockObj){
								sb.AppendFormat("\r\ntranid:{0} retdata:{1}",BitConverter.ToInt32(result,0),Encoding.UTF8.GetString(result,4,result.Length-4));
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

		private static void GetNewConfig(object sender,AsyncSocketLibrary.Common.SocketSettingEventArgs e){

			Console.WriteLine ("come in here");

			Dictionary<string,string> dict = e.DictArg;

			if (dict != null)
				foreach (string key in dict.Keys)
					Console.WriteLine (string.Format ("{0} - {1}", key, dict [key]));
		}
	}
}
