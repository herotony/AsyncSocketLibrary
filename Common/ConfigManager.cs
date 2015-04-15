using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common
{
	public class ConfigManager
	{	
		private FileSystemWatcher watcher = null;
		private string path =string.Empty;
		private string watchFileName = "socketsetting.txt";

		public delegate void OnConfigChangedEventHandler(object sender,SocketSettingEventArgs arg);

		public event OnConfigChangedEventHandler ConfigChanged;

		public ConfigManager(bool startWatcher = false){

			path = AppDomain.CurrentDomain.BaseDirectory;

			if(!path.EndsWith(@"\"))
				path+=@"\";

			if (startWatcher) {

				watcher = new FileSystemWatcher (path, "*.txt");				
				watcher.NotifyFilter = NotifyFilters.LastWrite;
				watcher.Changed += new FileSystemEventHandler (ShouldInvoked);

				//开始监控变化
				watcher.EnableRaisingEvents = true;
			}				
		}

		private void ShouldInvoked(object sender,FileSystemEventArgs arg){

			//莫名会触发两次，这里屏蔽一次
			if (Monitor.TryEnter (this, 1)) {

				string fileName = arg.Name;				

				if (!fileName.ToLower ().Equals (watchFileName))
					return;

				Dictionary<string,string> dictConfig = GetOriginalSettingInfo ();

				if (dictConfig!=null) {

					if (ConfigChanged != null)
						ConfigChanged (this, new SocketSettingEventArgs(dictConfig));
				}
			}									
		}		

		public Dictionary<string,string> GetOriginalSettingInfo(){

			Dictionary<string,string> dictConfig = new Dictionary<string, string> ();

			string content = string.Empty;

			using (FileStream fs = new FileStream (path+watchFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

				StreamReader reader = new StreamReader (fs);

				content = reader.ReadToEnd ();
			}

			if (string.IsNullOrEmpty (content))
				return null;

			string[] lines = content.Split (new string[]{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

			if (lines == null || lines.Length.Equals (0))
				return null;

			for (int i = 0; i < lines.Length; i++) {

				string[] configInfo = lines [i].Split (':');

				if (configInfo.Length < 2)
					continue;

				if (dictConfig.ContainsKey (configInfo [0]))
					dictConfig [configInfo [0]] = configInfo [1];
				else
					dictConfig.Add (configInfo [0], configInfo [1]);
			}
				
			return dictConfig.Count > 0 ? dictConfig : null;
		}
	}
}

