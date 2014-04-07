using log4net;
using log4net.Config;
using log4net.Core;
using log4net.Repository.Hierarchy;
using NUnit.Framework;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.TestUtils.Tests.Logging
{
	[TestFixture]
	public class WorkerLogSpyFixture
	{
		static WorkerLogSpyFixture()
		{
			XmlConfigurator.Configure();
		}

		[Test]
		public void OnlyEnlistmentsWorksShouldBeEnclosed()
		{
			bool work1Done = false;
			var logger = (Logger) (LogManager.GetLogger(typeof (LoggedClassStub)).Logger);
			logger.Level = Level.Info;
			var wls = new WorkerLogSpy(typeof (LoggedClassStub));

			Assert.That(logger.Level == Level.Info);

			wls.Enlist(() => work1Done = true);
			wls.ExecuteEnlistments();

			Assert.That(logger.Level == Level.Info);
			Assert.That(work1Done);
		}

		[Test]
		public void ShouldExecuteWorksAndLog()
		{
			bool work1Done = false, work2Done = false;
			var wls = new WorkerLogSpy(typeof (LoggedClassStub));
			var lcs = new LoggedClassStub();
			wls.Enlist(() => work1Done = true);
			wls.Enlist(() => work2Done = true);
			wls.Enlist(() => lcs.LogDebug("The message"));

			Assert.That(wls.GetWholeLog(), Text.Contains("The message"));
			Assert.That(work1Done && work2Done);
		}

		[Test]
		public void ShouldRespectMessagesSequence()
		{
			var wls = new WorkerLogSpy(typeof (LoggedClassStub));
			var lcs = new LoggedClassStub();
			wls.Enlist(() => lcs.LogDebug("message 1"));
			wls.Enlist(() => lcs.LogDebug("message 2"));
			wls.Enlist(() => lcs.LogDebug("message 3"));

			Assert.That(wls.Messages(), Is.EqualTo(new[] {"message 1", "message 2", "message 3"}));
		}
	}
}