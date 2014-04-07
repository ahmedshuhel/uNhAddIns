using log4net.Config;
using NUnit.Framework;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.TestUtils.Tests.Logging
{
	[TestFixture]
	public class SpyingFixture
	{
		static SpyingFixture()
		{
			XmlConfigurator.Configure();
		}

		[Test]
		public void ShouldWorkFine()
		{
			Assert.That(
				Spying.Logger<LoggedClassStub>()
					.Execute(() => (new LoggedClassStub()).LogDebug("my message"))
					.MessageSequence,
				Is.EqualTo(new[] { "my message" }));
		}
	}
}