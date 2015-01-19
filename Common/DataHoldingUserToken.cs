using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketLibrary.Common
{
	class DataHoldingUserToken
	{
		internal Mediator theMediator;
		internal DataHolder theDataHolder;

		internal Int32 socketHandleNumber;

		internal readonly Int32 bufferOffsetReceive;
		internal readonly Int32 permanentReceiveMessageOffset;
		internal readonly Int32 bufferOffsetSend;

		private Int32 idOfThisObject; //for testing only        

		internal Int32 lengthOfCurrentIncomingMessage;

		internal Int32 receiveMessageOffset;        
		internal Byte[] byteArrayForPrefix;        
		internal readonly Int32 receivePrefixLength;

		internal Int32 receivedPrefixBytesDoneCount = 0;
		internal Int32 receivedMessageBytesDoneCount = 0;

		internal Int32 recPrefixBytesDoneThisOp = 0;

		internal Int32 sendBytesRemainingCount;
		internal readonly Int32 sendPrefixLength;
		internal Byte[] dataToSend;
		internal Int32 bytesSentAlreadyCount;

		//The session ID correlates with all the data sent in a connected session.
		//It is different from the transmission ID in the DataHolder, which relates
		//to one TCP message. A connected session could have many messages, if you
		//set up your app to allow it.
		private Int32 sessionId;                

		public DataHoldingUserToken(SocketAsyncEventArgs e, Int32 rOffset, Int32 sOffset, Int32 receivePrefixLength, Int32 sendPrefixLength, Int32 identifier)
		{
			this.idOfThisObject = identifier;

			//Create a Mediator that has a reference to the SAEA object.
			this.theMediator = new Mediator(e);
			this.bufferOffsetReceive = rOffset;
			this.bufferOffsetSend = sOffset;
			this.receivePrefixLength = receivePrefixLength;
			this.sendPrefixLength = sendPrefixLength;
			this.receiveMessageOffset = rOffset + receivePrefixLength;
			this.permanentReceiveMessageOffset = this.receiveMessageOffset;            
		}

		//Let's use an ID for this object during testing, just so we can see what
		//is happening better if we want to.
		public Int32 TokenId
		{
			get
			{
				return this.idOfThisObject;
			}
		}

		internal void CreateNewDataHolder()
		{
			theDataHolder = new DataHolder();
		}

		//Used to create sessionId variable in DataHoldingUserToken.
		//Called in ProcessAccept().
		internal void CreateSessionId()
		{
			sessionId = Interlocked.Increment(ref StatisticInfo.serverSessionId);                        
		}

		public Int32 SessionId
		{
			get
			{
				return this.sessionId;
			}
		}

		public void Reset()
		{
			this.receivedPrefixBytesDoneCount = 0;
			this.receivedMessageBytesDoneCount = 0;
			this.recPrefixBytesDoneThisOp = 0;
			this.receiveMessageOffset = this.permanentReceiveMessageOffset;
		}
	}
}

