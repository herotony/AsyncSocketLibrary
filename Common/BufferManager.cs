using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace AsyncSocketLibrary.Common
{
	class BufferManager
	{
		int totalBytesInBufferBlock;

		// Byte array maintained by the Buffer Manager.
		byte[] bufferBlock;         
		Stack<int> freeIndexPool;     
		int currentIndex;
		int bufferBytesAllocatedForEachSaea;

		//totalBytes % totalBufferBytesInEachSaeaObject should must equals 0 
		public BufferManager(int totalBytes, int totalBufferBytesInEachSaeaObject)
		{
			totalBytesInBufferBlock = totalBytes;
			this.currentIndex = 0;
			this.bufferBytesAllocatedForEachSaea = totalBufferBytesInEachSaeaObject;
			this.freeIndexPool = new Stack<int>();
		}
			
		internal void InitBuffer()
		{
			// Create one large buffer block.
			this.bufferBlock = new byte[totalBytesInBufferBlock];
		}


		internal bool SetBuffer(SocketAsyncEventArgs args)
		{

			if (this.freeIndexPool.Count > 0)
			{
				args.SetBuffer(this.bufferBlock, this.freeIndexPool.Pop(), this.bufferBytesAllocatedForEachSaea);
			}
			else
			{
				if ((totalBytesInBufferBlock - this.bufferBytesAllocatedForEachSaea) < this.currentIndex)
				{
					return false;
				}
				args.SetBuffer(this.bufferBlock, this.currentIndex, this.bufferBytesAllocatedForEachSaea);
				this.currentIndex += this.bufferBytesAllocatedForEachSaea;
			}
			return true;
		}
			
		// on the server
		// keep the same buffer space assigned to one SAEA object for the duration of
		// this app's running.
		internal void FreeBuffer(SocketAsyncEventArgs args)
		{
			this.freeIndexPool.Push(args.Offset);
			args.SetBuffer(null, 0, 0);
		}

	}
}

