using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;



namespace AsyncSocketLibrary.Common
{
	class PrefixHandler
	{
		public static int HandlePrefix(SocketAsyncEventArgs e, DataHoldingUserToken receiveSendToken, int remainingBytesToProcess)
		{            
			if (receiveSendToken.receivedPrefixBytesDoneCount == 0)
			{
				if (StatisticInfo.watchProgramFlow)   //for loging
				{
					LogManager.Log("PrefixHandler,first packet ! create prefix array tokenId: " + receiveSendToken.TokenId);
				}
				receiveSendToken.byteArrayForPrefix = new Byte[receiveSendToken.receivePrefixLength];
			}
				
			if (remainingBytesToProcess >= receiveSendToken.receivePrefixLength - receiveSendToken.receivedPrefixBytesDoneCount)
			{
				if (StatisticInfo.watchProgramFlow)   //for loging
				{
					LogManager.Log("PrefixHandler, enough for prefix tokenId: " + receiveSendToken.TokenId + ". remainingBytesToProcess = " + remainingBytesToProcess);
				}

				Buffer.BlockCopy(e.Buffer, receiveSendToken.receiveMessageOffset - receiveSendToken.receivePrefixLength + receiveSendToken.receivedPrefixBytesDoneCount, receiveSendToken.byteArrayForPrefix, receiveSendToken.receivedPrefixBytesDoneCount, receiveSendToken.receivePrefixLength - receiveSendToken.receivedPrefixBytesDoneCount);

				remainingBytesToProcess = remainingBytesToProcess - receiveSendToken.receivePrefixLength + receiveSendToken.receivedPrefixBytesDoneCount;

				receiveSendToken.recPrefixBytesDoneThisOp = receiveSendToken.receivePrefixLength - receiveSendToken.receivedPrefixBytesDoneCount;

				receiveSendToken.receivedPrefixBytesDoneCount = receiveSendToken.receivePrefixLength;

				receiveSendToken.lengthOfCurrentIncomingMessage = BitConverter.ToInt32(receiveSendToken.byteArrayForPrefix, 0);
			}				
			else
			{
				if (StatisticInfo.watchProgramFlow)   //for loging
				{
					LogManager.Log("PrefixHandler, NOT all of prefix tokenId: " + receiveSendToken.TokenId + ". remainingBytesToProcess = " + remainingBytesToProcess);
				}

				Buffer.BlockCopy(e.Buffer, receiveSendToken.receiveMessageOffset - receiveSendToken.receivePrefixLength + receiveSendToken.receivedPrefixBytesDoneCount, receiveSendToken.byteArrayForPrefix, receiveSendToken.receivedPrefixBytesDoneCount, remainingBytesToProcess);

				receiveSendToken.recPrefixBytesDoneThisOp = remainingBytesToProcess;
				receiveSendToken.receivedPrefixBytesDoneCount += remainingBytesToProcess;
				remainingBytesToProcess = 0;
			}
				
			if (remainingBytesToProcess == 0)
			{   
				receiveSendToken.receiveMessageOffset = receiveSendToken.receiveMessageOffset - receiveSendToken.recPrefixBytesDoneThisOp;
				receiveSendToken.recPrefixBytesDoneThisOp = 0;
			}

			return remainingBytesToProcess;
		}
	}
}

