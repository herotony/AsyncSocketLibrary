﻿using System;
using System.IO;
using System.Collections.Generic; 
using System.Net.Sockets;
using System.Threading; //for Semaphore and Interlocked
using System.Net;
using System.Text; 
using System.Diagnostics; 

namespace AsyncSocketLibrary.Common.Server
{
	public class SocketListener{		

		internal int maxConcurrentConnectOpCount = 0;
		internal int maxConcurrentRecSendCount = 0;

		private int concurrentConnectOpCount = 0;
		private int concurrentRecSendCount = 0;

		//A Semaphore has two parameters, the initial number of available slots
		// and the maximum number of slots. We'll make them the same. 
		//This Semaphore is used to keep from going over max connection #. (It is not about 
		//controlling threading really here.)   
		Semaphore theMaxConnectionsEnforcer;
			
		BufferManager theBufferManager;

		Socket listenSocket; 

		SocketListenerSettings socketListenerSettings;

		PrefixHandler prefixHandler;
		MessageHandler messageHandler;

		SocketAsyncEventArgsPool poolOfAcceptEventArgs;
		SocketAsyncEventArgsPool poolOfRecSendEventArgs;

		public SocketListener(SocketListenerSettings theSocketListenerSettings){
		
			this.socketListenerSettings = theSocketListenerSettings;

			this.theBufferManager = new BufferManager(this.socketListenerSettings.BufferSize * this.socketListenerSettings.NumberOfSaeaForRecSend * this.socketListenerSettings.OpsToPreAllocate,
				                                      this.socketListenerSettings.BufferSize * this.socketListenerSettings.OpsToPreAllocate);

			this.poolOfRecSendEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.NumberOfSaeaForRecSend);
			this.poolOfAcceptEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.MaxAcceptOps);

			// Create connections count enforcer
			this.theMaxConnectionsEnforcer = new Semaphore(this.socketListenerSettings.MaxConnections, this.socketListenerSettings.MaxConnections);
		}

		private void Init(){



		}
			
		internal SocketAsyncEventArgs CreateNewSaeaForAccept(SocketAsyncEventArgsPool pool)
		{
			//Allocate the SocketAsyncEventArgs object. 
			SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();

			acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

			AcceptOpUserToken theAcceptOpToken = new AcceptOpUserToken(pool.AssignTokenId() + 10000);
			acceptEventArg.UserToken = theAcceptOpToken;

			return acceptEventArg;
		}
			
		private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (StatisticInfo.watchProgramFlow == true)   //for testing
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
				LogManager.Log("AcceptEventArg_Completed, id " + theAcceptOpToken.TokenId);
			}

			if (StatisticInfo.watchThreads == true)   //for testing
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
				LogManager.Log("AcceptEventArg_Completed()", theAcceptOpToken);
			}

			ProcessAccept(e);
		}

		private void LoopToStartAccept()
		{
			if (StatisticInfo.watchProgramFlow == true)   //for testing
			{                                
				LogManager.Log("LoopToStartAccept");
			}

			StartAccept();
		}

		private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
		{
			var acceptOpToken = (acceptEventArgs.UserToken as AcceptOpUserToken);
			LogManager.Log("Closing socket of accept id " + acceptOpToken.TokenId);

			//This method closes the socket and releases all resources, both
			//managed and unmanaged. It internally calls Dispose.           
			acceptEventArgs.AcceptSocket.Close();

			//Put the SAEA back in the pool.
			poolOfAcceptEventArgs.Push(acceptEventArgs);
		}

		private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
		{
			// This is when there was an error with the accept op. That should NOT
			// be happening often. It could indicate that there is a problem with
			// that socket. If there is a problem, then we would have an infinite
			// loop here, if we tried to reuse that same socket.
			if (acceptEventArgs.SocketError != SocketError.Success)
			{
				// Loop back to post another accept op. Notice that we are NOT
				// passing the SAEA object here.
				LoopToStartAccept();

				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
				LogManager.Log(string.Format("SocketError:{0}, accept id:{1} " ,acceptEventArgs.SocketError, theAcceptOpToken.TokenId));

				//Let's destroy this socket, since it could be bad.
				HandleBadAccept(acceptEventArgs);

				//Jump out of the method.
				return;
			}
				
			int numberOfConnectedSockets = Interlocked.Increment(ref this.concurrentConnectOpCount);
			if (numberOfConnectedSockets > this.maxConcurrentConnectOpCount)
			{
				Interlocked.Increment(ref this.maxConcurrentConnectOpCount);
			}

		    if (StatisticInfo.watchProgramFlow == true)   
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
				LogManager.Log("ProcessAccept, accept id " + theAcceptOpToken.TokenId);
			}


			//Now that the accept operation completed, we can start another
			//accept operation, which will do the same. Notice that we are NOT
			//passing the SAEA object here.
			LoopToStartAccept();

			// Get a SocketAsyncEventArgs object from the pool of receive/send op 
			//SocketAsyncEventArgs objects
			SocketAsyncEventArgs receiveSendEventArgs = this.poolOfRecSendEventArgs.Pop();

			//Create sessionId in UserToken.
			((ServerDataHoldingUserToken)receiveSendEventArgs.UserToken).CreateSessionId();

			//A new socket was created by the AcceptAsync method. The 
			//SocketAsyncEventArgs object which did the accept operation has that 
			//socket info in its AcceptSocket property. Now we will give
			//a reference for that socket to the SocketAsyncEventArgs 
			//object which will do receive/send.
			receiveSendEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;

			if (StatisticInfo.watchProgramFlow == true)
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
				LogManager.Log("Accept id " + theAcceptOpToken.TokenId + ". RecSend id " + ((DataHoldingUserToken)receiveSendEventArgs.UserToken).TokenId + ".  Remote endpoint = " + IPAddress.Parse(((IPEndPoint)receiveSendEventArgs.AcceptSocket.RemoteEndPoint).Address.ToString()) + ": " + ((IPEndPoint)receiveSendEventArgs.AcceptSocket.RemoteEndPoint).Port.ToString() + ". client(s) connected = " + this.numberOfAcceptedSockets);
			}
				
			acceptEventArgs.AcceptSocket = null;
			this.poolOfAcceptEventArgs.Push(acceptEventArgs);            

			StartReceive(receiveSendEventArgs);
		}
					       
		internal void StartAccept()        
		{
			SocketAsyncEventArgs acceptEventArg;
						       
			if (this.poolOfAcceptEventArgs.Count > 1)
			{
				try
				{
					acceptEventArg = this.poolOfAcceptEventArgs.Pop();
				}
				catch
				{
					acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
				}
			}
			else
			{
				acceptEventArg = CreateNewSaeaForAccept(poolOfAcceptEventArgs);
			}
				
			if (StatisticInfo.watchThreads == true)   //for testing
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
				LogManager.Log("StartAccept()", theAcceptOpToken);
			}
			if (StatisticInfo.watchProgramFlow == true)   //for testing
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
				LogManager.Log("still in StartAccept, id = " + theAcceptOpToken.TokenId);
			}
							         
			this.theMaxConnectionsEnforcer.WaitOne();
						         
			bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
			if (!willRaiseEvent)
			{                
				if (StatisticInfo.watchProgramFlow == true)   //for testing
				{
					AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
					LogManager.Log("StartAccept in if (!willRaiseEvent), accept token id " + theAcceptOpToken.TokenId);
				}
					
				ProcessAccept(acceptEventArg);
			}                        
		}


		private void StartReceive(SocketAsyncEventArgs receiveSendEventArgs)
		{
			DataHoldingUserToken receiveSendToken = (DataHoldingUserToken)receiveSendEventArgs.UserToken;

			receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetReceive, this.socketListenerSettings.BufferSize);                    

			bool willRaiseEvent = receiveSendEventArgs.AcceptSocket.ReceiveAsync(receiveSendEventArgs);
			if (!willRaiseEvent)
			{
				ProcessReceive(receiveSendEventArgs);                
			}            
		}

		private void ProcessReceive(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ServerDataHoldingUserToken receiveSendToken = (ServerDataHoldingUserToken)receiveSendEventArgs.UserToken;

			if (receiveSendEventArgs.SocketError != SocketError.Success)
			{								
				LogManager.Log(string.Format("ProcessReceive ERROR: {0}, receiveSendToken id:{1}",receiveSendEventArgs.SocketError , receiveSendToken.TokenId));

				receiveSendToken.Reset();
				CloseClientSocket(receiveSendEventArgs);

				return;
			}

			// If no data was received, close the connection. This is a NORMAL
			// situation that shows when the client has finished sending data.
			if (receiveSendEventArgs.BytesTransferred == 0)
			{
				receiveSendToken.Reset();
				CloseClientSocket(receiveSendEventArgs);
				return;
			}

			//The BytesTransferred property tells us how many bytes 
			//we need to process.
			int remainingBytesToProcess = receiveSendEventArgs.BytesTransferred;						   

			//If we have not got all of the prefix already, 
			//then we need to work on it here.                                
			if (receiveSendToken.receivedPrefixBytesDoneCount < this.socketListenerSettings.ReceivePrefixLength)
			{
				remainingBytesToProcess = prefixHandler.HandlePrefix(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);

				if (remainingBytesToProcess == 0)
				{                    
					// We need to do another receive op, since we do not have
					// the message yet, but remainingBytesToProcess == 0.
					StartReceive(receiveSendEventArgs);

					return;
				}
			}

			// If we have processed the prefix, we can work on the message now.
			// We'll arrive here when we have received enough bytes to read
			// the first byte after the prefix.
			bool incomingTcpMessageIsReady = messageHandler.HandleMessage(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);

			if (incomingTcpMessageIsReady == true)
			{
				// Pass the DataHolder object to the Mediator here. The data in
				// this DataHolder can be used jifor all kinds of things that an
				// intelligent and creative person like you might think of.                        
				receiveSendToken.theMediator.HandleData(receiveSendToken.theDataHolder);

				receiveSendToken.dataMessageReceived = null;
				receiveSendToken.Reset();

				receiveSendToken.theMediator.PrepareOutgoingData();
				StartSend(receiveSendToken.theMediator.GiveBack());
			}
			else
			{
				receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;
				receiveSendToken.recPrefixBytesDoneThisOp = 0;

				StartReceive(receiveSendEventArgs);
			}            
		}

		private void CloseClientSocket(SocketAsyncEventArgs e)
		{
			var receiveSendToken = (e.UserToken as ServerDataHoldingUserToken);

			// do a shutdown before you close the socket
			try
			{
				e.AcceptSocket.Shutdown(SocketShutdown.Both);
			}
			// throws if socket was already closed
			catch (Exception closeErr)
			{
				if (StatisticInfo.watchProgramFlow == true)   //for testing
				{
					LogManager.Log("CloseClientSocket, Shutdown catch, id " + receiveSendToken.TokenId + "\r\n",closeErr);
				}
			}

			//This method closes the socket and releases all resources, both
			//managed and unmanaged. It internally calls Dispose.
			e.AcceptSocket.Close();

			//Make sure the new DataHolder has been created for the next connection.
			//If it has, then dataMessageReceived should be null.
			if (receiveSendToken.dataMessageReceived != null)
			{
				receiveSendToken.dataMessageReceived = null;
			}

			// Put the SocketAsyncEventArg back into the pool,
			// to be used by another client. This 
			this.poolOfRecSendEventArgs.Push(e);

			// decrement the counter keeping track of the total number of clients 
			//connected to the server, for testing
			Interlocked.Decrement(ref this.concurrentRecSendCount);

			//Release Semaphore so that its connection counter will be decremented.
			//This must be done AFTER putting the SocketAsyncEventArg back into the pool,
			//or you can run into problems.
			this.theMaxConnectionsEnforcer.Release();
		}
	}

}

