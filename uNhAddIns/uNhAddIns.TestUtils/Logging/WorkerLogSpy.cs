using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Core;

namespace uNhAddIns.TestUtils.Logging
{
	public class WorkerLogSpy : IWorker
	{
		private readonly ILoggerWrapper logger;
		private readonly List<Action> enlistments = new List<Action>(2);
		private IEnumerable<string> messages;
		private bool done;

		private WorkerLogSpy(ILoggerWrapper log)
		{
			logger = log;
		}

		public WorkerLogSpy(Type loggerType) : this(LogManager.GetLogger(loggerType)) {}

		public WorkerLogSpy(string loggerName) : this(LogManager.GetLogger(loggerName)) {}

		public void Enlist(Action work)
		{
			if (work == null)
			{
				throw new ArgumentNullException("work");
			}
			enlistments.Add(work);
		}

		public void ExecuteEnlistments()
		{
			if(done)
			{
				return;
			}
			using (var ls = new LogSpy(logger))
			{
				foreach (Action action in enlistments)
				{
					action();
				}
				messages = ls.Messages().ToArray();
			}
			done = true;
		}

		public string GetWholeLog()
		{
			ExecuteEnlistments();
			var sb = new StringBuilder(100);
			foreach (var message in messages)
			{
				sb.Append(message);
			}
			return sb.ToString();
		}

		public IEnumerable<string> Messages()
		{
			ExecuteEnlistments();
			return messages;
		}
	}
}