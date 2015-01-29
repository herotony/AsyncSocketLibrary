using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace AsyncSocketLibrary.Common
{
	internal class SocketAsyncEventArgsPool
	{
		Stack<SocketAsyncEventArgs> pool;

		//just for log
		private int nextTokenId = 0;

		internal SocketAsyncEventArgsPool(Int32 capacity)
		{
			this.pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		internal int AssignTokenId()
		{
			int tokenId = Interlocked.Increment(ref nextTokenId);
			return tokenId;
		}

		internal int Count
		{
			get { return this.pool.Count; }
		}

		internal SocketAsyncEventArgs Pop()
		{
			lock (this.pool)
			{
				if (this.pool.Count > 0)
				{
					return this.pool.Pop();
				}
				else
					return null;
			}
		}

		internal void Push(SocketAsyncEventArgs item)
		{
			if (item == null)
			{
				throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null");
			}

			lock (this.pool)
			{
				this.pool.Push(item);
			}
		}
	}
}

