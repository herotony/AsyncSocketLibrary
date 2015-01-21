using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketLibrary.Common
{
	class DataHoldingUserToken
	{	
		private Int32 idOfThisObject; //for loging only 

		internal readonly Int32 bufferOffsetReceive;
		internal readonly Int32 permanentReceiveMessageOffset;
		internal readonly Int32 bufferOffsetSend;
		internal readonly Int32 receivePrefixLength;
		internal readonly Int32 sendPrefixLength;						     

		internal DataHolder theDataHolder;
		internal Int32 lengthOfCurrentIncomingMessage;
		internal Int32 receiveMessageOffset;   
		internal Int32 receivedMessageBytesDoneCount = 0;

		internal Byte[] byteArrayForPrefix;        
		internal Int32 receivedPrefixBytesDoneCount = 0;
		internal Int32 recPrefixBytesDoneThisOp = 0;

		internal Byte[] dataToSend;
		internal Int32 bytesSentAlreadyCount;
		internal Int32 sendBytesRemainingCount;				          

		public DataHoldingUserToken(SocketAsyncEventArgs e, Int32 rOffset, Int32 sOffset, Int32 receivePrefixLength, Int32 sendPrefixLength, Int32 identifier)
		{
			this.idOfThisObject = identifier;

			this.bufferOffsetReceive = rOffset;
			this.bufferOffsetSend = sOffset;
			this.receivePrefixLength = receivePrefixLength;
			this.sendPrefixLength = sendPrefixLength;
			this.receiveMessageOffset = rOffset + receivePrefixLength;
			this.permanentReceiveMessageOffset = this.receiveMessageOffset;            
		}

		//Let's use an ID for this object , just so we can see what
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

		public void Reset()
		{
			this.receivedPrefixBytesDoneCount = 0;
			this.receivedMessageBytesDoneCount = 0;
			this.recPrefixBytesDoneThisOp = 0;
			this.receiveMessageOffset = this.permanentReceiveMessageOffset;
		}
	}
}

