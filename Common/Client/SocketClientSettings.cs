using System;
using System.Net;

namespace AsyncSocketLibrary.Common.Client
{
	internal class SocketClientSettings
	{

		//设置最大并发处理连接的saea
		private int maxSimultaneousConnectOps;

		//设置最大并发处理数据传输的saea连接
		private int maxSimultaneousConnections;

		//设置最大并发处理数据传输的saea
		private int numberOfSaeaForRecSend;

		//buffer size to use for each socket receive operation
		private int bufferSize;

		//length of message prefix for receive ops
		private int receivePrefixLength;

		//length of message prefix for send ops
		private int sendPrefixLength;

		// 1 for receive, 1 for send. used in BufferManager，所以一般为2，代表一个用于收数据，一个用于发数据
		//用于BufferManager时，就是：numberOfSaeaForRecSend * bufferSize * 2 而每个连接的是 buffersize * 2
		private int opsToPreAllocate;

		private int timeOutByMS;//单位：毫秒

		private IPEndPoint serverEndPoint;

		//可考虑大并发时，一个连接发用于送多条请求消息后再关闭...
		private int numberOfMessagesPerConnection;

		public SocketClientSettings(IPEndPoint theServerEndPoint, int numberOfMessagesPerConnection, int maxSimultaneousConnectOps, int theMaxConnections, int bufferSize=128, int receivePrefixLength = 4, int sendPrefixLength = 4, int opsToPreAlloc = 2,int timeOut=1000 )
		{			         
			this.maxSimultaneousConnectOps = maxSimultaneousConnectOps;
			this.maxSimultaneousConnections = theMaxConnections;
			this.numberOfSaeaForRecSend = theMaxConnections + 1;
			this.receivePrefixLength = receivePrefixLength;
			this.bufferSize = bufferSize;
			this.sendPrefixLength = sendPrefixLength;
			this.opsToPreAllocate = opsToPreAlloc;
			this.serverEndPoint = theServerEndPoint;
			this.numberOfMessagesPerConnection = numberOfMessagesPerConnection;
			this.timeOutByMS = timeOut;
		}
			
		public int TimeOutByMS{

			get{ 

				return this.timeOutByMS;
			}
		}

		public int MaxConnectOps
		{
			get
			{
				return this.maxSimultaneousConnectOps;
			}
		}

		public int MaxConnections
		{
			get
			{
				return this.maxSimultaneousConnections;
			}
		}

		public int NumberOfSaeaForRecSend
		{
			get
			{
				return this.numberOfSaeaForRecSend;
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
				return this.bufferSize;
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

		public IPEndPoint ServerEndPoint
		{
			get
			{
				return this.serverEndPoint;
			}
		}

		public int NumberOfMessagesPerConnection
		{
			get
			{
				return this.numberOfMessagesPerConnection;
			}
		}                
	}
}

