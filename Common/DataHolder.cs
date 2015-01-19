using System;
using System.Net;

namespace AsyncSocketLibrary.Common
{
	public class DataHolder
	{
		internal Byte[] dataMessageReceived;

		internal Int32 receivedTransMissionId;

		internal Int32 sessionId;

		//With a packet analyzer this can help you see specific connections.
		internal EndPoint remoteEndpoint;
	}
}

