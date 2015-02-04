using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace AsyncSocketLibrary.Common.Server
{
	public class SocketListenerSettings
	{
		// the maximum number of connections the sample is designed to handle simultaneously 
		private int maxConnections;

		// this variable allows us to create some extra SAEA objects for the pool,
		// if we wish.
		private int numberOfSaeaForRecSend;

		// max # of pending connections the listener can hold in queue
		private int backlog;

		// tells us how many objects to put in pool for accept operations
		private int maxSimultaneousAcceptOps;

		private int receiveBufferSize;

		private int receivePrefixLength;

		private int sendPrefixLength;

		private int opsToPreAllocate;

		// Endpoint for the listener.
		private IPEndPoint localEndPoint;

		public SocketListenerSettings(int maxConnections, int excessSaeaObjectsInPool, int backlog, int maxSimultaneousAcceptOps, int receivePrefixLength, int receiveBufferSize, int sendPrefixLength, int opsToPreAlloc, IPEndPoint theLocalEndPoint)
		{
			this.maxConnections = maxConnections;
			this.numberOfSaeaForRecSend = maxConnections + excessSaeaObjectsInPool;
			this.backlog = backlog;
			this.maxSimultaneousAcceptOps = maxSimultaneousAcceptOps;
			this.receivePrefixLength = receivePrefixLength;
			this.receiveBufferSize = receiveBufferSize;
			this.sendPrefixLength = sendPrefixLength;
			this.opsToPreAllocate = opsToPreAlloc;
			this.localEndPoint = theLocalEndPoint;
		}

		public int MaxConnections
		{
			get
			{
				return this.maxConnections;
			}
		}
		public int NumberOfSaeaForRecSend
		{
			get
			{
				return this.numberOfSaeaForRecSend;
			}
		}
		public int Backlog
		{
			get
			{
				return this.backlog;
			}
		}
		public int MaxAcceptOps
		{
			get
			{
				return this.maxSimultaneousAcceptOps;
			}
		}
		public int ReceivePrefixLength
		{
			get
			{
				return this.receivePrefixLength;
			}
		}
		public int BufferSize
		{
			get
			{
				return this.receiveBufferSize;
			}
		}
		public int SendPrefixLength
		{
			get
			{
				return this.sendPrefixLength;
			}
		}
		public int OpsToPreAllocate
		{
			get
			{
				return this.opsToPreAllocate;
			}
		}
		public IPEndPoint LocalEndPoint
		{
			get
			{
				return this.localEndPoint;
			}
		}  
	}
}

