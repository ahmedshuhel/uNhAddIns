using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class SessionFactoryProviderFixture
	{
		public SessionFactoryProviderFixture()
		{
			XmlConfigurator.Configure();
		}

		public class MultiConfStub : AbstractConfigurationProvider
		{
			public override IEnumerable<Configuration> Configure()
			{
				return new[] {(new Configuration()).Configure(), new Configuration()};
			}
		}

		[Test]
		public void CtorProtectecion()
		{
			Assert.Throws<ArgumentNullException>(() => new SessionFactoryProvider(null));
		}

		[Test]
		public void DisposeDoNotInitialize()
		{
			var sfp = new SessionFactoryProvider();
			Assert.That(Spying.Logger<SessionFactoryProvider>().Execute(sfp.Dispose).WholeMessage,
			            Text.DoesNotContain("Initialize a new session factory"));
		}

		[Test]
		public void InitializeShouldLog()
		{
			var sfp = new SessionFactoryProvider();
			Assert.That(Spying.Logger<SessionFactoryProvider>().Execute(sfp.Initialize).WholeMessage,
			            Text.Contains("Initialize a new session factory"));
		}

		[Test]
		public void ShouldExecuteDisposeOlnyOne()
		{
			SessionFactoryProvider sfp;
			ISessionFactory sf1;
			bool disposed = false;
			using (sfp = new SessionFactoryProvider())
			{
				sfp.BeforeCloseSessionFactory += ((sender, e) => disposed = true);
				sf1 = sfp.GetFactory(null);
			}
			Assert.That(disposed);
			Assert.That(sf1.IsClosed, "The session-factory should be closed.");
			disposed = false;
			sfp.Dispose();
			Assert.That(!disposed);
		}

		[Test]
		public void ShouldHasOnlyOneInstanceOfFactory()
		{
			var sfp = new SessionFactoryProvider();
			ISessionFactory sf1 = sfp.GetFactory(null);
			ISessionFactory sf2 = sfp.GetFactory(null);
			Assert.That(sf1, Is.Not.Null);
			Assert.That(ReferenceEquals(sf1, sf2));
			Assert.That(sfp.Count(), Is.EqualTo(1));
			IEnumerator en = ((IEnumerable)sfp).GetEnumerator();
			int i = 0;
			while (en.MoveNext())
			{
				i++;
			}
			Assert.That(i, Is.EqualTo(1));
		}

		[Test]
		public void ShouldInitializeOnlyOneTime()
		{
			var sfp = new SessionFactoryProvider();
			Assert.That(sfp.GetFactory(null), Is.Not.Null);

			Assert.That(Spying.Logger<SessionFactoryProvider>().Execute(sfp.Initialize).WholeMessage,
			            Text.DoesNotContain("Initialize a new session factory"));
		}

		[Test]
		public void ShouldLogMultiConfigurationIgnored()
		{
			var sfp = new SessionFactoryProvider(new MultiConfStub());
			Assert.That(Spying.Logger<SessionFactoryProvider>().Execute(sfp.Initialize).WholeMessage,
			            Text.Contains("More than one configurations are available"));
		}
	}
}