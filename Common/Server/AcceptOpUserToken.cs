using System;
using System.Net.Sockets;
using System.Text;

namespace AsyncSocketLibrary.Common.Server
{
	//仅仅是为了统计跟踪，否则，该类并没必要
    internal class AcceptOpUserToken
	{
		private int id; 
		internal int socketHandleNumber; 

		public AcceptOpUserToken(int identifier)
		{
			id = identifier;
		}

		public int TokenId
		{
			get
			{
				return id;
			}
			set
			{
				id = value;
			}
		}
	}
}

