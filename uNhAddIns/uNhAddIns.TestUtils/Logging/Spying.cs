using System;
using System.Collections.Generic;

namespace uNhAddIns.TestUtils.Logging
{
	public static class Spying
	{
		public static IWorkToSpy Logger<T>()
		{
			return new LoggerSpy(new WorkerLogSpy(typeof(T)));
		}

		public static IWorkToSpy Logger(string loggerName)
		{
			return new LoggerSpy(new WorkerLogSpy(loggerName));
		}

		public static IWorkToSpy Logger(Type t)
		{
			return new LoggerSpy(new WorkerLogSpy(t));
		}
	}

	public interface IWorkToSpy
	{
		ILoggerResults Execute(Action work);
	}

	public interface ILoggerResults
	{
		string WholeMessage { get; }
		IEnumerable<string> MessageSequence { get; }
	}

	internal class LoggerSpy : IWorkToSpy
	{
		private readonly WorkerLogSpy spy;

		public LoggerSpy(WorkerLogSpy spy)
		{
			this.spy = spy;
		}

		#region Implementation of ILoggerSpy

		public ILoggerResults Execute(Action work)
		{
			spy.Enlist(work);
			return new LoggerResult(spy);
		}

		#endregion
	}

	internal class LoggerResult : ILoggerResults
	{
		private readonly WorkerLogSpy spy;

		public LoggerResult(WorkerLogSpy spy)
		{
			this.spy = spy;
		}

		#region Implementation of ILoggerResults

		public string WholeMessage
		{
			get { return spy.GetWholeLog(); }
		}

		public IEnumerable<string> MessageSequence
		{
			get { return spy.Messages(); }
		}

		#endregion
	}
}