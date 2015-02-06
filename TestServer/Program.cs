using System;
using System.Net;
using AsyncSocketLibrary.Common;
using AsyncSocketLibrary.Common.Server;

namespace TestServer
{
	class MainClass
	{
		public static void Main (string[] args)
		{
		
			Console.WriteLine ("Hello World!");

			try{

				SocketListenerSettings settings = new SocketListenerSettings (100, 100, 1000, 100, 4, 128, 4, 2, new IPEndPoint (IPAddress.Any, 6969));

				SocketListener listener = new SocketListener (settings, process);

			}catch(Exception initErr){

				LogManager.Log (string.Empty, initErr);
			}


			Console.ReadKey ();
		}

		private static byte[] process(byte[] input){

			return input;

		}
	}
}
