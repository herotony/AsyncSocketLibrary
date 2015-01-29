using System;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Client
{
	public class MessagePreparer
	{
		internal static void GetDataToSend(SocketAsyncEventArgs e)
		{            
			ClientDataHoldingUserToken theUserToken = (ClientDataHoldingUserToken)e.UserToken;
			ClientSendDataHolder dataHolder = theUserToken.theSendDataHolder;

			//In this example code, we will  
			//prefix the message with the length of the message. So we put 2 
			//things into the array.
			// 1) prefix,
			// 2) the message.

			//Determine the length of the message that we will send.
			int lengthOfCurrentOutgoingMessage = dataHolder.arrayOfMessageToSend[dataHolder.NumberOfMessagesSent].Length;

			//convert the message to byte array
			Byte[] arrayOfBytesInMessage = dataHolder.arrayOfMessageToSend[dataHolder.NumberOfMessagesSent];

			//So, now we convert the length integer into a byte array.
			//Aren't byte arrays wonderful? Maybe you'll dream about byte arrays tonight!
			Byte[] arrayOfBytesInPrefix = BitConverter.GetBytes(lengthOfCurrentOutgoingMessage);

			//Create the byte array to send.
			theUserToken.dataToSend = new Byte[theUserToken.sendPrefixLength + lengthOfCurrentOutgoingMessage];

			//Now copy the 2 things to the theUserToken.dataToSend.
			Buffer.BlockCopy(arrayOfBytesInPrefix, 0, theUserToken.dataToSend, 0, theUserToken.sendPrefixLength);
			Buffer.BlockCopy(arrayOfBytesInMessage, 0, theUserToken.dataToSend, theUserToken.sendPrefixLength, lengthOfCurrentOutgoingMessage);

			theUserToken.sendBytesRemainingCount = theUserToken.sendPrefixLength + lengthOfCurrentOutgoingMessage;
			theUserToken.bytesSentAlreadyCount = 0;
		}
	}
}

