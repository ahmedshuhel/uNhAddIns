using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace uNhAddIns.TestUtils.Logging
{
	public class LogSpy : ILogSpy
	{
		private readonly MemoryAppender appender;
		private readonly Logger logger;
		private readonly Level prevLogLevel;

		public LogSpy(ILoggerWrapper log)
		{
			logger = (Logger) log.Logger;

			// Change the log level to DEBUG and temporarily save the previous log level
			prevLogLevel = logger.Level;
			logger.Level = Level.Debug;

			// Add a new MemoryAppender to the logger.
			appender = new MemoryAppender();
			logger.AddAppender(appender);
		}

		public LogSpy(Type loggerType) : this(LogManager.GetLogger(loggerType)) {}

		public LogSpy(string loggerName) : this(LogManager.GetLogger(loggerName)) {}

		protected MemoryAppender Appender
		{
			get { return appender; }
		}

		#region ILogSpy Members

		public virtual string GetWholeLog()
		{
			var wholeMessage = new StringBuilder();
			foreach (string singleMessage in Appender.GetEvents().Select(x => x.RenderedMessage))
			{
				wholeMessage.Append(singleMessage);
			}
			return wholeMessage.ToString();
		}

		public virtual IEnumerable<string> Messages()
		{
			return Appender.GetEvents().Select(x => x.RenderedMessage);
		}

		public void Dispose()
		{
			// Restore the previous log level of the SQL logger and remove the MemoryAppender
			logger.RemoveAppender(appender);
			logger.Level = prevLogLevel;
		}

		#endregion
	}
}