using System;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ClientDataHoldingUserToken : DataHoldingUserToken
	{
		internal ClientSendDataHolder theSendDataHolder;

		public ClientDataHoldingUserToken(SocketAsyncEventArgs e, int rOffset, int sOffset, int receivePrefixLength, int sendPrefixLength, int identifier):base(e,rOffset,sOffset,receivePrefixLength,sendPrefixLength,identifier){
		}

		internal void CreateNewSendDataHolder()
		{
			theSendDataHolder = new ClientSendDataHolder();
		}
	}
}

