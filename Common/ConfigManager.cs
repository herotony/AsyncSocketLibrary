using System;
using System.IO;
using System.Collections.Generic;

namespace AsyncSocketLibrary.Common
{
	public class ConfigManager
	{	
		private FileSystemWatcher watcher = null;
		private string path =string.Empty;

		public delegate void OnConfigChangedEventHandler(object sender,SocketSettingEventArgs arg);

		public event OnConfigChangedEventHandler ConfigChanged;

		public ConfigManager(){

			path = AppDomain.CurrentDomain.BaseDirectory;

			if(!path.EndsWith(@"\"))
				path+=@"\";
			watcher = new FileSystemWatcher (path, "*.txt");
			watcher.NotifyFilter = NotifyFilters.LastWrite|NotifyFilters.LastAccess|NotifyFilters.Size;
			watcher.Changed += new FileSystemEventHandler (ShouldInvoked);
		}

		private void ShouldInvoked(object sender,FileSystemEventArgs arg){

			string fileName = arg.Name;

			Console.WriteLine (fileName);

			if (!fileName.ToLower ().Equals ("socketsetting.txt"))
				return;

			Dictionary<string,string> dictConfig = new Dictionary<string, string> ();

			string content = string.Empty;

			using (FileStream fs = new FileStream (arg.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {

				StreamReader reader = new StreamReader (fs);

				content = reader.ReadToEnd ();
			}

			if (string.IsNullOrEmpty (content))
				return;

			string[] lines = content.Split (new string[]{ "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

			if (lines == null || lines.Length.Equals (0))
				return;

			for (int i = 0; i < lines.Length; i++) {

				string[] configInfo = lines [i].Split (':');

				if (configInfo.Length < 2)
					continue;

				if (dictConfig.ContainsKey (configInfo [0]))
					dictConfig [configInfo [0]] = configInfo [1];
				else
					dictConfig.Add (configInfo [0], configInfo [1]);
			}

			if (dictConfig.Count > 0) {

				if (ConfigChanged != null)
					ConfigChanged (this, new SocketSettingEventArgs(dictConfig));
			}				
		}			
	}
}

