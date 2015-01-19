using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace AsyncSocketLibrary.Common
{
	class IncomingDataPreparer
	{
		private DataHolder theDataHolder;
		private SocketAsyncEventArgs theSaeaObject;

		public IncomingDataPreparer(SocketAsyncEventArgs e)
		{
			this.theSaeaObject = e;
		}

		private Int32 ReceivedTransMissionIdGetter()
		{
			Int32 receivedTransMissionId = Interlocked.Increment (ref StatisticInfo.serverTransMissionId);
			return receivedTransMissionId;
		}

		private EndPoint GetRemoteEndpoint()
		{   
			return this.theSaeaObject.AcceptSocket.RemoteEndPoint;
		}

		internal DataHolder HandleReceivedData(DataHolder incomingDataHolder, SocketAsyncEventArgs theSaeaObject)
		{
			DataHoldingUserToken receiveToken = (DataHoldingUserToken)theSaeaObject.UserToken;

			if (StatisticInfo.watchProgramFlow)   //for loging
			{
				LogManager.Log ("HandleReceivedData() tokenId: " + receiveToken.TokenId);
			}
			theDataHolder = incomingDataHolder;
			theDataHolder.sessionId = receiveToken.SessionId;
			theDataHolder.receivedTransMissionId = this.ReceivedTransMissionIdGetter();            
			theDataHolder.remoteEndpoint = this.GetRemoteEndpoint();

			return theDataHolder;
		}
					  
	}
}

