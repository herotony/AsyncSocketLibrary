using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;


namespace AsyncSocketLibrary.Common.Server
{
	public class ServerDataHoldingUserToken : DataHoldingUserToken
	{

		//The session ID correlates with all the data sent in a connected session.
		//It is different from the transmission ID in the DataHolder, which relates
		//to one TCP message. A connected session could have many messages, if you
		//set up your app to allow it.
		private int sessionId;    

		public ServerDataHoldingUserToken(SocketAsyncEventArgs e, int rOffset, int sOffset, int receivePrefixLength, int sendPrefixLength, int identifier):base(e,rOffset,sOffset,receivePrefixLength,sendPrefixLength,identifier){
		}

		//Used to create sessionId variable in DataHoldingUserToken.
		//Called in ProcessAccept().
		internal void CreateSessionId()
		{
			sessionId = Interlocked.Increment(ref StatisticInfo.serverSessionId);                        
		}
	}
}

