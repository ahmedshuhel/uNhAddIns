using log4net;

namespace uNhAddIns.TestUtils.Tests.Logging
{
	public class LoggedClassStub
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(LoggedClassStub));

		public void LogDebug(string message)
		{
			log.Debug(message);
		}
	}
}