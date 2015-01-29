using System;

namespace AsyncSocketLibrary.Common.Client
{
	internal class ConnectOpUserToken
	{
		internal OutgoingMessageHolder outgoingMessageHolder;

		private int id; //for loging

		public ConnectOpUserToken(int identifier)
		{
			id = identifier;
		}

		public int TokenId
		{
			get
			{
				return id;
			}
		}
	}
}

