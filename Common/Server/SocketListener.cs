using System;
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

		private Func<byte[],byte[]> _dataProcessor;

		Semaphore theMaxConnectionsEnforcer;
			
		BufferManager theBufferManager;

		Socket listenSocket = null; 

		SocketListenerSettings socketListenerSettings;

		SocketAsyncEventArgsPool poolOfAcceptEventArgs;
		SocketAsyncEventArgsPool poolOfRecSendEventArgs;

		public SocketListener(Func<byte[],byte[]> dataProcessor){
		
			ConfigManager cfm = new ConfigManager ();
			Dictionary<string,string> dictCfg = cfm.GetOriginalSettingInfo ();

			ParseSettingInfo pInfo = new ParseSettingInfo (dictCfg);

			this._dataProcessor = dataProcessor;
			this.socketListenerSettings = pInfo.GetSocketListenerSetting();

			this.theBufferManager = new BufferManager(this.socketListenerSettings.BufferSize * this.socketListenerSettings.NumberOfSaeaForRecSend * this.socketListenerSettings.OpsToPreAllocate,
				                                      this.socketListenerSettings.BufferSize * this.socketListenerSettings.OpsToPreAllocate);
			this.poolOfRecSendEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.NumberOfSaeaForRecSend);
			this.poolOfAcceptEventArgs = new SocketAsyncEventArgsPool(this.socketListenerSettings.MaxAcceptOps);

			this.theMaxConnectionsEnforcer = new Semaphore(this.socketListenerSettings.MaxConnections, this.socketListenerSettings.MaxConnections);

			Init ();
			StartListen ();
		}

		public string GetConnectInfo(){

			string info = string.Format ("maxconnop:{0}\r\nmaxdataconn:{1}\r\ncurrentop:{2}\r\ncurrentconn:{3}",
				              maxConcurrentConnectOpCount, maxConcurrentRecSendCount, concurrentConnectOpCount, concurrentRecSendCount);

			return info;
		}			

		private void StartListen()
		{		
			listenSocket = new Socket(this.socketListenerSettings.LocalEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			listenSocket.Bind(this.socketListenerSettings.LocalEndPoint);
			listenSocket.Listen(this.socketListenerSettings.Backlog);

			StartAccept();
		}

		private void Init(){					     
		
			this.theBufferManager.InitBuffer();
						         
			for (int i = 0; i < this.socketListenerSettings.MaxAcceptOps; i++)
			{
				this.poolOfAcceptEventArgs.Push(CreateNewSaeaForAccept(poolOfAcceptEventArgs));
			}           
				
			SocketAsyncEventArgs eventArgObjectForPool;

			int tokenId;

			for (int i = 0; i < this.socketListenerSettings.NumberOfSaeaForRecSend; i++)
			{
				eventArgObjectForPool = new SocketAsyncEventArgs();

				this.theBufferManager.SetBuffer(eventArgObjectForPool);

				tokenId = poolOfRecSendEventArgs.AssignTokenId() + 1000000;

				eventArgObjectForPool.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);

				ServerDataHoldingUserToken theTempReceiveSendUserToken = new ServerDataHoldingUserToken(eventArgObjectForPool.Offset, eventArgObjectForPool.Offset + this.socketListenerSettings.BufferSize, this.socketListenerSettings.ReceivePrefixLength, this.socketListenerSettings.SendPrefixLength, tokenId);

				eventArgObjectForPool.UserToken = theTempReceiveSendUserToken;

				this.poolOfRecSendEventArgs.Push(eventArgObjectForPool);
			}

		}

		private void IO_Completed(object sender, SocketAsyncEventArgs e)
		{		
			ServerDataHoldingUserToken receiveSendToken = (ServerDataHoldingUserToken)e.UserToken;

			switch (e.LastOperation)
			{
			case SocketAsyncOperation.Receive:
				                 
				ProcessReceive(e);
				break;

			case SocketAsyncOperation.Send:

				ProcessSend(e);
				break;

			default:

				throw new ArgumentException(string.Format("The last operation completed on the socket was not a receive or send [tokenId:{0}]",receiveSendToken.TokenId));
			}
		}
			
		internal SocketAsyncEventArgs CreateNewSaeaForAccept(SocketAsyncEventArgsPool pool)
		{
			SocketAsyncEventArgs acceptEventArg = new SocketAsyncEventArgs();

			acceptEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(AcceptEventArg_Completed);

			AcceptOpUserToken theAcceptOpToken = new AcceptOpUserToken(pool.AssignTokenId() + 10000);
			acceptEventArg.UserToken = theAcceptOpToken;

			return acceptEventArg;
		}
			
		private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
		{
			if (StatisticInfo.watchProgramFlow == true)   
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
				LogManager.Log("AcceptEventArg_Completed, id " + theAcceptOpToken.TokenId);
			}

			if (StatisticInfo.watchThreads == true) 
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)e.UserToken;
				LogManager.Log(string.Format("AcceptEventArg_Completed():{0}", theAcceptOpToken));
			}

			ProcessAccept(e);
		}

		private void LoopToStartAccept()
		{
			if (StatisticInfo.watchProgramFlow == true) 
			{                                
				LogManager.Log("LoopToStartAccept");
			}

			StartAccept();
		}

		private void HandleBadAccept(SocketAsyncEventArgs acceptEventArgs)
		{
			var acceptOpToken = (acceptEventArgs.UserToken as AcceptOpUserToken);
			LogManager.Log("Closing socket of accept id " + acceptOpToken.TokenId);
						    
			acceptEventArgs.AcceptSocket.Close();

			poolOfAcceptEventArgs.Push(acceptEventArgs);
		}

		private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
		{
			if (acceptEventArgs.SocketError != SocketError.Success)
			{
				LoopToStartAccept();

				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
				LogManager.Log(string.Format("SocketError:{0}, accept id:{1} " ,acceptEventArgs.SocketError, theAcceptOpToken.TokenId));

				HandleBadAccept(acceptEventArgs);

				return;
			}
				
			int numberOfConnectedSockets = Interlocked.Increment(ref this.concurrentConnectOpCount);
			if (numberOfConnectedSockets > this.maxConcurrentConnectOpCount)
			{
				this.maxConcurrentConnectOpCount = numberOfConnectedSockets;
			}

		    if (StatisticInfo.watchProgramFlow == true)   
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArgs.UserToken;
				LogManager.Log("ProcessAccept, accept id " + theAcceptOpToken.TokenId);
			}
				
			LoopToStartAccept();

			SocketAsyncEventArgs receiveSendEventArgs = this.poolOfRecSendEventArgs.Pop();

			ServerDataHoldingUserToken userToken = (ServerDataHoldingUserToken)receiveSendEventArgs.UserToken;

			userToken.CreateSessionId ();

			receiveSendEventArgs.AcceptSocket = acceptEventArgs.AcceptSocket;

			userToken.CreateNewDataHolder (receiveSendEventArgs);
				
			acceptEventArgs.AcceptSocket = null;
			this.poolOfAcceptEventArgs.Push(acceptEventArgs);   

			Interlocked.Decrement(ref this.concurrentConnectOpCount);
			int numberOfRecSendCount = Interlocked.Increment (ref this.concurrentRecSendCount);
			if (numberOfRecSendCount > maxConcurrentRecSendCount)
				this.maxConcurrentRecSendCount = numberOfRecSendCount;


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
				
			if (StatisticInfo.watchThreads == true)  
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
				LogManager.Log(string.Format("StartAccept():{0}", theAcceptOpToken));
			}
			if (StatisticInfo.watchProgramFlow == true)  
			{
				AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
				LogManager.Log("still in StartAccept, id = " + theAcceptOpToken.TokenId);
			}
							         
			this.theMaxConnectionsEnforcer.WaitOne();
						         
			bool willRaiseEvent = listenSocket.AcceptAsync(acceptEventArg);
			if (!willRaiseEvent)
			{                
				if (StatisticInfo.watchProgramFlow == true) 
				{
					AcceptOpUserToken theAcceptOpToken = (AcceptOpUserToken)acceptEventArg.UserToken;
					LogManager.Log("StartAccept in if (!willRaiseEvent), accept token id " + theAcceptOpToken.TokenId);
				}
					
				ProcessAccept(acceptEventArg);
			}                        
		}
			
		private void StartReceive(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ServerDataHoldingUserToken receiveSendToken = (ServerDataHoldingUserToken)receiveSendEventArgs.UserToken;

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
				
			if (receiveSendEventArgs.BytesTransferred == 0)
			{
				receiveSendToken.Reset();
				CloseClientSocket(receiveSendEventArgs);
				return;
			}

	
			int remainingBytesToProcess = receiveSendEventArgs.BytesTransferred;						   

		                    
			if (receiveSendToken.receivedPrefixBytesDoneCount < this.socketListenerSettings.ReceivePrefixLength)
			{
				remainingBytesToProcess = PrefixHandler.HandlePrefix(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);

				if (remainingBytesToProcess == 0)
				{                    
					// We need to do another receive op, since we do not have
					// the message yet, but remainingBytesToProcess == 0.
					StartReceive(receiveSendEventArgs);

					return;
				}
			}


			bool incomingTcpMessageIsReady = MessageHandler.HandleMessage(receiveSendEventArgs, receiveSendToken, remainingBytesToProcess);

			if (incomingTcpMessageIsReady == true)
			{
				if (receiveSendToken.dataMessageReceived != null && receiveSendToken.dataMessageReceived.Length > 0) {

					byte[] processResult = _dataProcessor (receiveSendToken.dataMessageReceived);

					OutgoingDataPreparer.PrepareOutgoingData (receiveSendEventArgs, processResult);
					StartSend(receiveSendEventArgs);

					receiveSendToken.dataMessageReceived = null;
					receiveSendToken.Reset();

				} else {

					receiveSendToken.Reset();
					CloseClientSocket(receiveSendEventArgs);
					return;
				}					
			}
			else
			{
				receiveSendToken.receiveMessageOffset = receiveSendToken.bufferOffsetReceive;
				receiveSendToken.recPrefixBytesDoneThisOp = 0;

				StartReceive(receiveSendEventArgs);
			}            
		}

		private void StartSend(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ServerDataHoldingUserToken receiveSendToken = (ServerDataHoldingUserToken)receiveSendEventArgs.UserToken;


			if (receiveSendToken.sendBytesRemainingCount <= this.socketListenerSettings.BufferSize)
			{
				receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
				Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, receiveSendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, receiveSendToken.sendBytesRemainingCount);
			}
			else
			{
				receiveSendEventArgs.SetBuffer(receiveSendToken.bufferOffsetSend, this.socketListenerSettings.BufferSize);
				Buffer.BlockCopy(receiveSendToken.dataToSend, receiveSendToken.bytesSentAlreadyCount, receiveSendEventArgs.Buffer, receiveSendToken.bufferOffsetSend, this.socketListenerSettings.BufferSize);

				//We'll change the value of sendUserToken.sendBytesRemainingCount
				//in the ProcessSend method.
			}

			//post asynchronous send operation
			bool willRaiseEvent = receiveSendEventArgs.AcceptSocket.SendAsync(receiveSendEventArgs);

			if (!willRaiseEvent)
			{
				ProcessSend(receiveSendEventArgs);
			}            
		}

		private void ProcessSend(SocketAsyncEventArgs receiveSendEventArgs)
		{
			ServerDataHoldingUserToken receiveSendToken = (ServerDataHoldingUserToken)receiveSendEventArgs.UserToken;			                   

			if (receiveSendEventArgs.SocketError == SocketError.Success)
			{
				receiveSendToken.sendBytesRemainingCount = receiveSendToken.sendBytesRemainingCount - receiveSendEventArgs.BytesTransferred;                 

				if (receiveSendToken.sendBytesRemainingCount == 0)
				{
					// If we are within this if-statement, then all the bytes in
					// the message have been sent. 
					StartReceive(receiveSendEventArgs);
				}
				else
				{					                 
					receiveSendToken.bytesSentAlreadyCount += receiveSendEventArgs.BytesTransferred;
					// So let's loop back to StartSend().
					StartSend(receiveSendEventArgs);
				}
			}
			else
			{
				receiveSendToken.Reset();
				CloseClientSocket(receiveSendEventArgs);
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
			receiveSendToken.dataMessageReceived = null;

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

