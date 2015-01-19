using System;

using log4net;

namespace AsyncSocketLibrary.Common
{
	public class LogManager
	{
		private static ILog log = log4net.LogManager.GetLogger(typeof(LogManager));

		public static void Log(string message,Exception e = null,bool isDebug = false){

			if (string.IsNullOrEmpty (message) && e == null)
				return;

			if (e != null) {

				string details = string.Format ("\r\nmessage:{0}\r\nerr_desc:{1}\r\nstacktrace:{2}\r\nsource:{3}{4}", message, e.Message, e.StackTrace,e.Source,GetInnerExceptionInfo(e.InnerException));

				log.Error (details);
			}
			else {

				if (isDebug)
					log.Debug (message);
				else
					log.Info (message);
			}
		}

		private static string GetInnerExceptionInfo(Exception e){		

			if (e == null)
				return string.Empty;

			return string.Format ("\r\ninnerException:\r\n\tinner_err_desc:{0}\r\n\tinner_stacktrace:{1}\r\n\tinner_source:{2}",e.Message,e.StackTrace,e.Source);
		}
	}
}

