using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ProcessClientSocketEventManager
	{	
		private SocketAsyncEventArgsPool poolOfRecSendEventArgs;
		private SocketAsyncEventArgsPool poolOfConnectEventArgs;


		public ProcessClientSocketEventManager(SocketAsyncEventArgsPool connectPool,SocketAsyncEventArgsPool recSendPool){

			this.poolOfConnectEventArgs = connectPool;
			this.poolOfRecSendEventArgs = recSendPool;
		}

		internal  void StartConnect(SocketAsyncEventArgs connectEventArgs,IPEndPoint serverEndPoint)        
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
		internal  void ProcessConnect(SocketAsyncEventArgs connectEventArgs)
		{
			ConnectOpUserToken theConnectingToken = (ConnectOpUserToken)connectEventArgs.UserToken;

			if (connectEventArgs.SocketError == SocketError.Success)
			{
				SocketAsyncEventArgs receiveSendEventArgs = this.poolOfRecSendEventArgs.Pop();

				if (receiveSendEventArgs == null) {

					LogManager.Log (string.Empty, new Exception (string.Format ("fetch receiveSendEventArgs failed for connect(tokenId:{0})", theConnectingToken.TokenId)));
					return;
				}

				receiveSendEventArgs.AcceptSocket = connectEventArgs.AcceptSocket;

				//Earlier, in the UserToken of connectEventArgs we put an array 
				//of messages to send. Now we move that array to the DataHolder in
				//the UserToken of receiveSendEventArgs.
				ClientDataHoldingUserToken receiveSendToken = (ClientDataHoldingUserToken)receiveSendEventArgs.UserToken;

				receiveSendToken.theSendDataHolder.PutMessagesToSend(theConnectingToken.outgoingMessageHolder.arrayOfMessages);

				MessagePreparer.GetDataToSend(receiveSendEventArgs);

				//StartSend(receiveSendEventArgs);

				//release connectEventArgs object back to the pool.
				connectEventArgs.AcceptSocket = null;

				this.poolOfConnectEventArgs.Push(connectEventArgs);
			}				
			else
			{
				//ProcessConnectionError(connectEventArgs);
			}
		}


	}
}

