using System;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ClientDataHoldingUserToken : DataHoldingUserToken
	{
		internal ClientSendDataHolder theSendDataHolder;

		public ClientDataHoldingUserToken(int rOffset, int sOffset, int receivePrefixLength, int sendPrefixLength, int identifier):base(rOffset,sOffset,receivePrefixLength,sendPrefixLength,identifier){
		}

		internal void CreateNewSendDataHolder()
		{
			theSendDataHolder = new ClientSendDataHolder();
		}
	}
}

