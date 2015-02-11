using System;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Server
{
	internal class OutgoingDataPreparer
	{
		internal static void PrepareOutgoingData(SocketAsyncEventArgs e,byte[] sendData){

			ServerDataHoldingUserToken userToken = (ServerDataHoldingUserToken)e.UserToken;

			//In this example code, we will send back the receivedTransMissionId,
			// followed by the
			//message that the client sent to the server. And we must
			//prefix it with the length of the message. So we put 3 
			//things into the array.
			// 1) prefix,
			// 2) receivedTransMissionId,
			// 3) the message that we received from the client, which
			// we stored in our DataHolder until we needed it.
			//That is our communication protocol. The client must know the protocol.

			//Convert the receivedTransMissionId to byte array.
			Byte[] idByteArray = BitConverter.GetBytes(userToken.dataHolder.receiveTransMissionId);

			//Determine the length of all the data that we will send back.
			//Int32 lengthOfCurrentOutgoingMessage = idByteArray.Length + userToken.dataMessageReceived.Length;
			int lengthOfCurrentOutgoingMessage = idByteArray.Length + sendData.Length;

			//So, now we convert the length integer into a byte array.
			//Aren't byte arrays wonderful? Maybe you'll dream about byte arrays tonight!
			Byte[] arrayOfBytesInPrefix = BitConverter.GetBytes(lengthOfCurrentOutgoingMessage);

			//Create the byte array to send.
			userToken.dataToSend = new Byte[userToken.sendPrefixLength + lengthOfCurrentOutgoingMessage];

			//Now copy the 3 things to the theUserToken.dataToSend.
			Buffer.BlockCopy(arrayOfBytesInPrefix, 0, userToken.dataToSend, 0, userToken.sendPrefixLength);
			Buffer.BlockCopy(idByteArray, 0, userToken.dataToSend, userToken.sendPrefixLength, idByteArray.Length);
			//The message that the client sent is already in a byte array, in DataHolder.
			//Buffer.BlockCopy(userToken.dataMessageReceived, 0, userToken.dataToSend, userToken.sendPrefixLength + idByteArray.Length, userToken.dataMessageReceived.Length);
			Buffer.BlockCopy(sendData, 0, userToken.dataToSend, userToken.sendPrefixLength + idByteArray.Length, sendData.Length);

			userToken.sendBytesRemainingCount = userToken.sendPrefixLength + lengthOfCurrentOutgoingMessage;
			userToken.bytesSentAlreadyCount = 0;
		}
	}
}

