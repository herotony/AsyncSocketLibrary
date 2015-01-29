using System;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common.Client
{
	//用于传递给最终要数据传输的saea
	internal class OutgoingMessageHolder
	{
		internal List<byte[]> arrayOfMessages;
		internal int countOfConnectionsRetries = 0;

		public OutgoingMessageHolder(List<byte[]> theArrayOfMessages)
		{
			this.arrayOfMessages = theArrayOfMessages;
		}
	}
}

