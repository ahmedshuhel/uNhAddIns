using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class DefaultMultiFactoryConfigurationProviderFixture
	{
		[Test]
		public void ShouldLoadTheTwoConfiguredConfigurations()
		{
			var mfc = new DefaultMultiFactoryConfigurationProvider();
			var actual = (Configuration[]) mfc.Configure();
			Assert.That(actual.Count(), Is.EqualTo(2));
			Assert.That(actual[0].Properties.ContainsKey("query.substitutions"));
			Assert.That(!actual[1].Properties.ContainsKey("query.substitutions"));
		}

		[Test]
		public void ShouldNotCallDefaultNhConfigurationIfSelfManagementInTheEvent()
		{
			int configurationCalls = 0;
			var mfc = new DefaultMultiFactoryConfigurationProvider();
			// jump the configuration of the first
			mfc.BeforeConfigure += ((sender, args) => args.Configured = 0 == configurationCalls++);
			var actual = (Configuration[]) mfc.Configure();
			Assert.That(actual.Count(), Is.EqualTo(2));
			Assert.That(!actual[0].Properties.ContainsKey("query.substitutions"), "The first config should not be configured.");
			Assert.That(!actual[1].Properties.ContainsKey("query.substitutions"));
		}

		[Test]
		public void ShouldCallAfterConfigurationForEachConfig()
		{
			int configurationCalls = 0;
			var localConf = new Configuration[2];
			var mfc = new DefaultMultiFactoryConfigurationProvider();
			mfc.AfterConfigure += ((sender, args) =>
			                       	{
			                       		localConf[configurationCalls] = args.Configuration;
			                       		configurationCalls++;
			                       	});
			var actual = (Configuration[]) mfc.Configure();
			Assert.That(ReferenceEquals(actual[0], localConf[0]));
			Assert.That(ReferenceEquals(actual[1], localConf[1]));
		}
	}
}