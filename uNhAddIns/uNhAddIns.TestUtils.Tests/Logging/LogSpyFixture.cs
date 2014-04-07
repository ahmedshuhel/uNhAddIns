using System.Linq;
using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.TestUtils.Tests.Logging
{
	[TestFixture]
	public class LogSpyFixture
	{
		static LogSpyFixture()
		{
			XmlConfigurator.Configure();
		}

		[Test]
		public void ShouldCatchLogAndRestorePreviousState()
		{
			const string expectedMessage = "my message";
			var stub = new LoggedClassStub();
			var logger = (Logger) (LogManager.GetLogger(typeof (LoggedClassStub)).Logger);
			logger.Level = Level.Info;

			using (var ls = new LogSpy(typeof (LoggedClassStub)))
			{
				Assert.That(logger.Level == Level.Debug);
				stub.LogDebug(expectedMessage);
				Assert.That(ls.GetWholeLog(), Text.Contains(expectedMessage));
				Assert.That(ls.Messages().Count(), Is.EqualTo(1));
				Assert.That(ls.Messages(), Is.EqualTo(new[] {expectedMessage}));
			}
			Assert.That(logger.Level == Level.Info);
		}
	}
}