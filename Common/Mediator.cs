using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common
{
	class Mediator
	{
		private IncomingDataPreparer theIncomingDataPreparer;
		private OutgoingDataPreparer theOutgoingDataPreparer;
		private DataHolder theDataHolder;
		private SocketAsyncEventArgs saeaObject;
		private DataHoldingUserToken receiveSendToken;

		public Mediator(SocketAsyncEventArgs e)
		{
			this.saeaObject = e;
			this.theIncomingDataPreparer = new IncomingDataPreparer(saeaObject);
			this.theOutgoingDataPreparer = new OutgoingDataPreparer();            
		}


		internal void HandleData(DataHolder incomingDataHolder)
		{   
			if (StatisticInfo.watchProgramFlow)   //for loging
			{
				receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
				LogManager.Log("Mediator HandleData() tokenId: " + receiveSendToken.TokenId);
			}
			theDataHolder = theIncomingDataPreparer.HandleReceivedData(incomingDataHolder, this.saeaObject);
		}

		internal void PrepareOutgoingData()
		{
			if (StatisticInfo.watchProgramFlow)   //for loging
			{
				receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
				LogManager.Log("Mediator PrepareOutgoingData() tokenId: " + receiveSendToken.TokenId);
			}

			//真正处理请求数据和返回数据的地!
			theOutgoingDataPreparer.PrepareOutgoingData(saeaObject, theDataHolder);            
		}


		internal SocketAsyncEventArgs GiveBack()
		{
			if (StatisticInfo.watchProgramFlow)   //for loging
			{
				receiveSendToken = (DataHoldingUserToken)this.saeaObject.UserToken;
				LogManager.Log("Mediator GiveBack() tokenId: " + receiveSendToken.TokenId);
			}
			return saeaObject;
		}
	}
}

