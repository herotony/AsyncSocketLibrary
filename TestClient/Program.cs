using System;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace TestClient
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Console.WriteLine ("Hello World!");
			Console.ReadKey ();

			Stopwatch sw = new Stopwatch ();

			sw.Start ();
			int testCount = 1000;
			int successCount = 0;
			int failedCount = 0;

			Task[] testTasks = new Task[testCount];

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
						else
							Interlocked.Increment(ref successCount);


					}catch(Exception e){

						Console.WriteLine(e.Message);
					}


				});					
			}

			Task.WaitAll (testTasks);

			Console.WriteLine (string.Format("总耗时:{0} ms 成功:{1} 条，失败:{2}条", sw.ElapsedMilliseconds,successCount,failedCount));

			Console.ReadKey ();
		}
	}
}
