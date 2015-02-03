using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ProcessClientSocketEventManager
	{	

		internal delegate bool PushResult(int messageTokenId,byte[] retData);

		internal PushResult PushResultCallback;

		private SocketAsyncEventArgsPool poolOfRecSendEventArgs;
		private SocketAsyncEventArgsPool poolOfConnectEventArgs;

		private int bufferSize;
		private int numberMessagesOfPerConnection;
		private int prefixHandleLength;

		Semaphore theMaxConnectionsEnforcer;


		public ProcessClientSocketEventManager(SocketAsyncEventArgsPool connectPool,SocketAsyncEventArgsPool recSendPool,int maxRecSendConnection,int BufferSize,int numberMessageOfPerConnection,int prefixHandleLength){

			this.poolOfConnectEventArgs = connectPool;
			this.poolOfRecSendEventArgs = recSendPool;

			this.theMaxConnectionsEnforcer = new Semaphore (maxRecSendConnection, maxRecSendConnection);
			this.bufferSize = BufferSize;
			this.numberMessagesOfPerConnection = numberMessageOfPerConnection;
			this.prefixHandleLength = prefixHandleLength;
		}

		internal void SendMessage(List<MessageInfo> messages,IPEndPoint serverEndPoint){

			theMaxConnectionsEnforcer.WaitOne ();

			SocketAsyncEventArgs connectEventArgs = this.poolOfConnectEventArgs.Pop();

			//or make a new one.            
			if (connectEventArgs == null) {
				connectEventArgs = new SocketAsyncEventArgs ();            
				connectEventArgs.Completed += new EventHandler<SocketAsyncEventArgs> (IO_Completed);

				ConnectOpUserToken theConnectingToken = new ConnectOpUserToken (poolOfConnectEventArgs.AssignTokenId () + 10000);
				connectEventArgs.UserToken = theConnectingToken;

				theConnectingToken.outgoingMessageHolder = new OutgoingMessageHolder (messages);

			} else {

				ConnectOpUserToken theConnectingToken = (ConnectOpUserToken)connectEventArgs.UserToken;
				theConnectingToken.outgoingMessageHolder = new OutgoingMessageHolder (messages);
			}								

			StartConnect (connectEventArgs, serverEndPoint);
		}

		private  void StartConnect(SocketAsyncEventArgs connectEventArgs,IPEndPoint serverEndPoint)        
		{
			ConnectOpUserToken theConnectingToken = (ConnectOpUserToken)connectEventArgs.UserToken;

			connectEventArgs.RemoteEndPoint = serverEndPoint;
			connectEventArgs.AcceptSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			//Post the connect operation on the socket.
			//A local port is assigned by the Windows OS during connect op.            
			bool willRaiseEvent = connectEventArgs.AcceptSocket.ConnectAsync(connectEventArgs);
			if (!willRaiseEvent)
			{
				ProcessConnect(connectEventArgs);
			}
		}

		//____________________________________________________________________________
		// Pass the connection info from the connecting object to the object
		// that will do send/receive. And put the connecting object back in the pool.
		//
		private  void ProcessConnect(SocketAsyncEventArgs connectEventArgs)
		{
			ConnectOpUserToken theConnectingToken = (ConnectOpUserToken)connectEventArgs.UserToken;

			if (connectEventArgs.SocketError == SocketError.Success)
			{
				SocketAsyncEventArgs receiveSendEventArgs = poolOfRecSendEventArgs.Pop();

				//所以考虑好并发量，需要通过性能测试来设置一个合理值来规避null
				if (receiveSendEventArgs == null) {

					LogManager.Log (string.Empty, new Exception (string.Format ("fetch receiveSendEventArgs failed for connect(tokenId:{0})", theConnectingToken.TokenId)));
					return;
				}

				receiveSendEventArgs.AcceptSocket = connectEventArgs.AcceptSocket;

				ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;

				receiveSendToken.theSendDataHolder.PutMessagesToSend(theConnectingToken.outgoingMessageHolder.arrayOfMessages);

				MessagePreparer.GetDataToSend(receiveSendEventArgs);

				receiveSendToken.startTime = theConnectingToken.outgoingMessageHolder.arrayOfMessages [0].startTime;

				StartSend(receiveSendEventArgs);

				//release connectEventArgs object back to the pool.
				connectEventArgs.AcceptSocket = null;

				this.poolOfConnectEventArgs.Push(connectEventArgs);
			}				
			else
			{
				ProcessConnectionError(connectEventArgs);
			}
		}

		private void StartSend(SocketAsyncEventArgs receiveSendEventArgs)
		{

			ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;            

			if (receiveSendToken.sendBytesRemainingCount <= this.bufferSize)
			{
				receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
				//Copy the bytes to the buffer associated with this SAEA object.
				Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, receiveSendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
			}
			else
			{
				//We cannot try to set the buffer any larger than its size.
				//So since receiveSendToken.sendBytesRemaining > its size, we just
				//set it to the maximum size, to send the most data possible.
				receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, this.bufferSize);
				//Copy the bytes to the buffer associated with this SAEA object.
				Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, receiveSendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, this.bufferSize);

				//We'll change the value of sendUserToken.sendBytesRemaining
				//in the ProcessSend method.
			}

			//post the send
			bool willRaiseEvent = receiveSendEventArgs.AcceptSocket.SendAsync(receiveSendEventArgs);
			if (!willRaiseEvent)
			{
				ProcessSend(receiveSendEventArgs);
			}
		}

		private void ProcessSend(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;

			if (receiveSendEventArgs.SocketError == SocketError.Success)
			{
				receiveSendToken.sendBytesRemainingCount = receiveSendToken.sendBytesRemainingCount - receiveSendEventArgs.BytesTransferred;
				// If this if statement is true, then we have sent all of the
				// bytes in the message. Otherwise, at least one more send
				// operation will be required to send the data.
				if (receiveSendToken.sendBytesRemainingCount == 0)
				{
					//incrementing count of messages sent on this connection                
					receiveSendToken.theSendDataHolder.NumberOfMessagesSent++;
					StartReceive(receiveSendEventArgs);                    
				}
				else
				{
					// So since (receiveSendToken.sendBytesRemaining == 0) is false,
					// we have more bytes to send for this message. So we need to 
					// call StartSend, so we can post another send message.
					receiveSendToken.bytesSentAlreadyCount += receiveSendEventArgs.BytesTransferred;
					StartSend(receiveSendEventArgs);
				}
			}
			else
			{
				// We'll just close the socket if there was a
				// socket error when receiving data from the client.

				if (PushResultCallback != null) {

					LogManager.Log (string.Format ("messageTokenId:{0} send failed! SocketError:{1}", receiveSendToken.messageTokenId,receiveSendEventArgs.SocketError));

					PushResultCallback (receiveSendToken.messageTokenId, null);
				}

				receiveSendToken.Reset();
				StartDisconnect(receiveSendEventArgs);
			}            
		}   

		private void StartReceive(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;
			//Set buffer for receive.          
			receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetReceive, this.bufferSize);
						        

			bool willRaiseEvent = receiveSendEventArgs.AcceptSocket.ReceiveAsync(receiveSendEventArgs);
			if (!willRaiseEvent)
			{
				ProcessReceive(receiveSendEventArgs);
			}

		}

		private void ProcessReceive(SocketAsyncEventArgs receiveSendEventArgs)
		{

			ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;
			// If there was a socket error, close the connection.
			if (receiveSendEventArgs.SocketError != SocketError.Success)
			{
				receiveSendToken.Reset();
				StartDisconnect(receiveSendEventArgs);
				return;
			}

			//If no data was received, close the connection.
			if (receiveSendEventArgs.BytesTransferred == 0)
			{
				receiveSendToken.Reset();
				StartDisconnect(receiveSendEventArgs);
				return;
			}
				

			int remainingBytesToProcess = receiveSendEventArgs.BytesTransferred;


			// If we have not got all of the prefix then we need to work on it. 
			// receivedPrefixBytesDoneCount tells us how many prefix bytes were
			// processed during previous receive ops which contained data for 
			// this message. (In normal use, usually there will NOT have been any 
			// previous receive ops here. So receivedPrefixBytesDoneCount would be 0.)
			if (receiveSendToken.receivedPrefixBytesDoneCount < this.prefixHandleLength)
			{
				remainingBytesToProcess = PrefixHandler.HandlePrefix(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);

				if (remainingBytesToProcess == 0)
				{                    
					// We need to do another receive op, since we do not have
					// the message yet.
					StartReceive(receiveSendEventArgs);

					//Jump out of the method, since there is no more data.
					return;
				}
			}

			// If we have processed the prefix, we can work on the message now.
			// We'll arrive here when we have received enough bytes to read
			// the first byte after the prefix.
			bool incomingTcpMessageIsReady = MessageHandler.HandleMessage(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);            

			if (incomingTcpMessageIsReady == true)
			{						
				//null out the byte array, for the next message
				receiveSendToken.theSendDataHolder.arrayOfMessageToSend = null;

				LogManager.Log (string.Format ("messageTokeId:{0} consume time :{1} ms", receiveSendToken.messageTokenId, DateTime.Now.Subtract (receiveSendToken.startTime).TotalMilliseconds));

				//将数据写入缓存字典中
				if (PushResultCallback != null)
					PushResultCallback (receiveSendToken.messageTokenId, receiveSendToken.dataMessageReceived);

				//Reset the variables in the UserToken, to be ready for the
				//next message that will be received on the socket in this
				//SAEA object.
				receiveSendToken.Reset();

				//If we have not sent all the messages, get the next message, and
				//loop back to StartSend.
				if (receiveSendToken.theSendDataHolder.NumberOfMessagesSent < this.numberMessagesOfPerConnection)
				{
					ClientDataHoldingUserToken theUserToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;
					ClientSendDataHolder dataHolder = theUserToken.theSendDataHolder;
					if (dataHolder.arrayOfMessageToSend.Count > receiveSendToken.theSendDataHolder.NumberOfMessagesSent) {

						//No need to reset the buffer for send here.
						//It is reset in the StartSend method.
						MessagePreparer.GetDataToSend(receiveSendEventArgs);
						StartSend(receiveSendEventArgs);
					}else
						StartDisconnect(receiveSendEventArgs);
						
				}
				else
				{
					//Since we have sent all the messages that we planned to send,
					//time to disconnect.                    
					StartDisconnect(receiveSendEventArgs);
				}
			}
			else
			{
				// Since we have NOT gotten enough bytes for the whole message,
				// we need to do another receive op. Reset some variables first.

				// All of the data that we receive in the next receive op will be
				// message. None of it will be prefix. So, we need to move the 
				// receiveSendToken.receiveMessageOffset to the beginning of the 
				// buffer space for this SAEA.
				receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;

				// Do NOT reset receiveSendToken.receivedPrefixBytesDoneCount here.
				// Just reset recPrefixBytesDoneThisOp.
				receiveSendToken.recPrefixBytesDoneThisOp = 0;                

				StartReceive(receiveSendEventArgs);
			}
		}

		internal void IO_Completed(object sender, SocketAsyncEventArgs e)
		{
			// determine which type of operation just completed and call the associated handler
			switch (e.LastOperation)
			{

			case SocketAsyncOperation.Connect:

				ProcessConnect(e);
				break;

			case SocketAsyncOperation.Receive:

				ProcessReceive(e);
				break;

			case SocketAsyncOperation.Send:

				ProcessSend(e);
				break;

			case SocketAsyncOperation.Disconnect:

				ProcessDisconnectAndCloseSocket(e);                    
				break;

			default:
				{
					ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)e.UserToken;

					if (PushResultCallback != null) {

						PushResultCallback (receiveSendToken.messageTokenId, null);
					}

					theMaxConnectionsEnforcer.Release ();
					LogManager.Log (string.Empty, new ArgumentException ("\r\nError in I/O Completed, id = " + receiveSendToken.TokenId));                       
				}   
				break;
			}
		}

		private void ProcessConnectionError(SocketAsyncEventArgs connectEventArgs)
		{
			try{

				ConnectOpUserToken theConnectingToken = (ConnectOpUserToken)connectEventArgs.UserToken;

				// If connection was refused by server or timed out or not reachable, then we'll keep this socket.
				// If not, then we'll destroy it.
				if ((connectEventArgs.SocketError != SocketError.ConnectionRefused) && (connectEventArgs.SocketError != SocketError.TimedOut)  && (connectEventArgs.SocketError != SocketError.HostUnreachable))
				{
					CloseSocket(connectEventArgs.AcceptSocket);
				}            				

				//返回null数据
				if (PushResultCallback != null) {

					for (int i = 0; i < theConnectingToken.outgoingMessageHolder.arrayOfMessages.Count; i++)
						PushResultCallback (theConnectingToken.outgoingMessageHolder.arrayOfMessages [i].MessageTokenId, null);
				}

				//it is time to release connectEventArgs object back to the pool.
				poolOfConnectEventArgs.Push(connectEventArgs);  

			}catch(Exception closeErr){

				LogManager.Log (string.Empty, closeErr);

			}finally{

				theMaxConnectionsEnforcer.Release();
			}				
		}

		private void StartDisconnect(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;

			receiveSendEventArgs.AcceptSocket.Shutdown(SocketShutdown.Both);
			bool willRaiseEvent = receiveSendEventArgs.AcceptSocket.DisconnectAsync(receiveSendEventArgs);
			if (!willRaiseEvent)
			{
				ProcessDisconnectAndCloseSocket(receiveSendEventArgs);
			}
		}

		private void ProcessDisconnectAndCloseSocket(SocketAsyncEventArgs receiveSendEventArgs)
		{
			try{

				ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;							

				//This method closes the socket and releases all resources, both
				//managed and unmanaged. It internally calls Dispose.
				receiveSendEventArgs.AcceptSocket.Close();

				//create an object that we can write data to.
				receiveSendToken.CreateNewSendDataHolder();

				// It is time to release this SAEA object.
				this.poolOfRecSendEventArgs.Push(receiveSendEventArgs);

			}catch(Exception shutdownErr){

				LogManager.Log (string.Empty, shutdownErr);

			}finally{

				this.theMaxConnectionsEnforcer.Release();
			}												
		}

		private void CloseSocket(Socket theSocket){

			try
			{
				theSocket.Shutdown(SocketShutdown.Both);
			}
			catch
			{
			}
			theSocket.Close();
		}

	}
}

