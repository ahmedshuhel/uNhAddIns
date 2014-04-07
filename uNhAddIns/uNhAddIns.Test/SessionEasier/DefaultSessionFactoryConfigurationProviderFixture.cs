using NHibernate.Cfg;
using NUnit.Framework;
using System.Linq;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class DefaultSessionFactoryConfigurationProviderFixture
	{
		[Test]
		public void ShouldLoadOnlyOneConfiguration()
		{
			var dc = new DefaultSessionFactoryConfigurationProvider();
			Assert.That(dc.Configure().Count(), Is.EqualTo(1));
		}

		[Test]
		public void ShouldCallDefaultNhConfiguration()
		{
			Configuration nhcfg = null;
			var dc = new DefaultSessionFactoryConfigurationProvider();
			dc.AfterConfigure += ((sender, args) => nhcfg = args.Configuration);
			dc.Configure();
			Assert.That(nhcfg, Is.Not.Null);
			Assert.That(nhcfg.Properties.ContainsKey(Environment.ConnectionString));
		}

		[Test]
		public void ShouldNotCallDefaultNhConfigurationIfSelfManagementInTheEvent()
		{
			Configuration nhcfg = null;
			var dc = new DefaultSessionFactoryConfigurationProvider();
			dc.BeforeConfigure += ((sender, args) => { nhcfg = args.Configuration;
			                                         	args.Configured = true; });
			dc.Configure();
			Assert.That(nhcfg, Is.Not.Null);
			Assert.That(!nhcfg.Properties.ContainsKey(Environment.ConnectionString));
		}
	}
}