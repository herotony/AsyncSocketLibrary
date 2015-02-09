using System;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common
{
	public class SocketSettingEventArgs : EventArgs
	{
		public SocketSettingEventArgs (Dictionary<string,string> data)
		{
			this.DictArg = data;
		}

		public Dictionary<string,string> DictArg{ get; private set; }
	}
}

