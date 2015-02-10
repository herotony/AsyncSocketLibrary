using System;
using System.Text;
using System.Net;
using System.Collections.Generic;

using AsyncSocketLibrary.Common.Client;
using AsyncSocketLibrary.Common.Server;

namespace AsyncSocketLibrary.Common
{
	public class ParseSettingInfo
	{
		private readonly Dictionary<string,string> dictOriginalSetting = null;

		private int opsToPreAllocate = 2;
		private int receivePrefixLength = 4;
		private int sendPrefixLength = 4;

		private IPEndPoint IPInfo = null;
		private int maxConnectSocketCount = 0;
		private int maxDataSocketCount = 0;
		private int bufferSize = 0;
		private int timeOutByMS = 0;
		private int numberSendCountPerConnection =0;
		private int queueRequestConnectCount = 0;


		public ParseSettingInfo (Dictionary<string,string> input)
		{
			dictOriginalSetting = input;
		}

		internal SocketClientSettings GetSocketClientSetting(){
		
			int shouldBingoCount = 6;
			int actualBingoCount = 0;

			foreach (string key in dictOriginalSetting.Keys) {

				if (string.IsNullOrEmpty (key))
					continue;

				string lowerKey = key.ToLower ().Trim();
				string value = dictOriginalSetting [key];

				if (lowerKey.StartsWith ("hostinfo")) {

					IPInfo = ParseHost (value);
					if (IPInfo == null)
						return null;	

					actualBingoCount++;

				} else if (lowerKey.StartsWith ("connectsocket_count")) {

					if (!int.TryParse (value, out maxConnectSocketCount))
						return null;

					actualBingoCount++;

				}else if (lowerKey.StartsWith ("datasocket_count")) {

					if (!int.TryParse (value, out maxDataSocketCount))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("buffersize")) {

					if (!int.TryParse (value, out bufferSize))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("timeout")) {

					if (!int.TryParse (value, out timeOutByMS))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("sendcountperconnect")) {

					if (!int.TryParse (value, out numberSendCountPerConnection))
						return null;

					actualBingoCount++;
				}

			}

			if (actualBingoCount.Equals (shouldBingoCount)) {

				return new SocketClientSettings (IPInfo, numberSendCountPerConnection, maxConnectSocketCount,
					maxDataSocketCount, bufferSize, receivePrefixLength, sendPrefixLength, opsToPreAllocate, timeOutByMS);
			} else
				throw new ArgumentNullException ("SocketClientSettings");				
		}

		internal SocketListenerSettings GetSocketListenerSetting(){

			int shouldBingoCount = 6;
			int actualBingoCount = 0;

			foreach (string key in dictOriginalSetting.Keys) {

				if (string.IsNullOrEmpty (key))
					continue;

				string lowerKey = key.ToLower ().Trim();
				string value = dictOriginalSetting [key];

				if (lowerKey.StartsWith ("hostinfo")) {

					IPInfo = ParseHost (value);
					if (IPInfo == null)
						return null;	

					actualBingoCount++;

				} else if (lowerKey.StartsWith ("connectsocket_count")) {

					if (!int.TryParse (value, out maxConnectSocketCount))
						return null;

					actualBingoCount++;

				}else if (lowerKey.StartsWith ("datasocket_count")) {

					if (!int.TryParse (value, out maxDataSocketCount))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("buffersize")) {

					if (!int.TryParse (value, out bufferSize))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("queuereqcount")) {

					if (!int.TryParse (value, out queueRequestConnectCount))
						return null;

					actualBingoCount++;
				}
				else if (lowerKey.StartsWith ("sendcountperconnect")) {

					if (!int.TryParse (value, out numberSendCountPerConnection))
						return null;

					actualBingoCount++;
				}

			}

			if (actualBingoCount.Equals (shouldBingoCount)) {

				return new SocketListenerSettings (maxDataSocketCount, 0,
					queueRequestConnectCount, maxConnectSocketCount, receivePrefixLength, bufferSize, sendPrefixLength, opsToPreAllocate, IPInfo);
			} else
				throw new ArgumentNullException ("SocketListenerSettings");				
		}

		private IPEndPoint  ParseHost(string value){

			string[] arr = value.Split ('|');

			if (value.Length < 2)
				return null;

			IPAddress ipAddr;

			if (!arr [0].ToLower ().StartsWith ("any")) {

				if (!IPAddress.TryParse (arr [0], out ipAddr))
					return null;
			} else
				ipAddr = IPAddress.Any;

			int port;
				
			if (!int.TryParse (arr [1], out port))
				return null;
				
			return new IPEndPoint (ipAddr, port);
		}
	}
}

