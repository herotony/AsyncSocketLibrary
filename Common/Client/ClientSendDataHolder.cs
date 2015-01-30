using System;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ClientSendDataHolder
	{
		private int numberOfMessagesSent = 0;

		//即每个元素是一次完整请求，相当于长连接一次处理N条请求
		internal List<MessageInfo> arrayOfMessageToSend = new List<MessageInfo>();
			
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

		internal void PutMessagesToSend(List<MessageInfo> theArrayOfMessagesToSend){

			this.arrayOfMessageToSend = theArrayOfMessagesToSend;
		}
	}
}

