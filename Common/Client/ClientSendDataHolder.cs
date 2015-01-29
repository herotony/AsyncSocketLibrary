using System;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ClientSendDataHolder
	{
		private int numberOfMessagesSent = 0;

		//即每个元素是一次完整请求，相当于长连接一次处理N条请求
		internal List<Byte[]> arrayOfMessageToSend = new List<byte[]>();
			
		public int NumberOfMessagesSent
		{
			get
			{
				return this.numberOfMessagesSent;
			}
			set
			{
				this.numberOfMessagesSent = value;
			}
		}

		internal void PutMessagesToSend(List<byte[]> theArrayOfMessagesToSend){

			this.arrayOfMessageToSend = theArrayOfMessagesToSend;
		}
	}
}

