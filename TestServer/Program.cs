﻿using System;
using System.Net;
using System.Threading;
using AsyncSocketLibrary.Common;
using AsyncSocketLibrary.Common.Server;

namespace TestServer
{
	class MainClass
	{
		private static SocketListener listener;

		public static void Main (string[] args)
		{		
			Console.WriteLine ("Hello World!");

			try{			

				listener = new SocketListener (process);

				Thread th = new Thread(new ThreadStart(Run));
				th.IsBackground = true;
				th.Start();

			}catch(Exception initErr){

				LogManager.Log (string.Empty, initErr);
			}


			Console.ReadKey ();
		}

		private static byte[] process(byte[] input){
		
			//LogManager.Log(string.Format ("th id:{0}", Thread.CurrentThread.ManagedThreadId));
			return input;

		}

		private static void Run(){

			while (true) {

				Console.WriteLine (listener.GetConnectInfo ());

				Thread.Sleep (30000);
			}
		}
	}
}
