using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketLibrary.Common
{
	class DataHoldingUserToken
	{	
		private int idOfThisObject; //for loging only 

		internal DateTime startTime;
		internal DateTime endTime;

		internal readonly int bufferOffsetReceive;
		internal readonly int permanentReceiveMessageOffset;
		internal readonly int bufferOffsetSend;
		internal readonly int receivePrefixLength;
		internal readonly int sendPrefixLength;						     

		internal int lengthOfCurrentIncomingMessage;
		internal int receiveMessageOffset;   
		internal int receivedMessageBytesDoneCount = 0;

		internal Byte[] byteArrayForPrefix;        
		internal int receivedPrefixBytesDoneCount = 0;
		internal int recPrefixBytesDoneThisOp = 0;

		internal int messageTokenId = 0;

		internal Byte[] dataToSend;
		internal int bytesSentAlreadyCount;
		internal int sendBytesRemainingCount;	

		internal Byte[] dataMessageReceived;

		public DataHoldingUserToken(SocketAsyncEventArgs e, int rOffset, int sOffset, int receivePrefixLength, int sendPrefixLength, int identifier)
		{
			this.idOfThisObject = identifier;

			this.bufferOffsetReceive = rOffset;
			this.bufferOffsetSend = sOffset;
			this.receivePrefixLength = receivePrefixLength;
			this.sendPrefixLength = sendPrefixLength;
			//会不断修正，permanentReceiveMessageOffset则是用于清零归位使用
			this.receiveMessageOffset = rOffset + receivePrefixLength;
			this.permanentReceiveMessageOffset = this.receiveMessageOffset;            
		}
			
		public int TokenId
		{
			get
			{
				return this.idOfThisObject;
			}
		}
						

		public void Reset()
		{
			this.messageTokenId = 0;
			this.receivedPrefixBytesDoneCount = 0;
			this.receivedMessageBytesDoneCount = 0;
			this.recPrefixBytesDoneThisOp = 0;
			this.receiveMessageOffset = this.permanentReceiveMessageOffset;
		}
	}
}

