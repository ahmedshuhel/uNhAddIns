using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Util;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.Test.SessionEasier
{
	[TestFixture]
	public class MultiSessionFactoryProviderFixture
	{
		public MultiSessionFactoryProviderFixture()
		{
			XmlConfigurator.Configure();
		}

		[Test]
		public void CtorProtectecion()
		{
			Assert.Throws<ArgumentNullException>(() => new MultiSessionFactoryProvider(null));
		}

		[Test]
		public void DisposeDoNotInitialize()
		{
			var sfp = new MultiSessionFactoryProvider();
			Assert.That(Spying.Logger<MultiSessionFactoryProvider>().Execute(sfp.Dispose).WholeMessage,
			            Text.DoesNotContain("Initialize new session factories reading the configuration."));
		}

		[Test]
		public void InitializeShouldLog()
		{
			var sfp = new MultiSessionFactoryProvider();
			Assert.That(Spying.Logger<MultiSessionFactoryProvider>().Execute(sfp.Initialize).WholeMessage,
			            Text.Contains("Initialize new session factories reading the configuration."));
		}

		[Test]
		public void ShouldExecuteDisposeOlnyOneForEachFactory()
		{
			MultiSessionFactoryProvider sfp;
			ISessionFactory sf1;
			int disposed = 0;
			using (sfp = new MultiSessionFactoryProvider())
			{
				sfp.BeforeCloseSessionFactory += ((sender, e) => disposed++);
				sf1 = sfp.GetFactory(null);
			}
			Assert.That(disposed, Is.EqualTo(2));
			Assert.That(sf1.IsClosed);
			disposed = 0;
			sfp.Dispose();
			Assert.That(disposed, Is.EqualTo(0));
		}

		[Test]
		public void ShouldHasOnlyTwoInstancesOfFactory()
		{
			var sfp = new MultiSessionFactoryProvider();
			Assert.That(sfp.Count(), Is.EqualTo(2));
			IEnumerator en = ((IEnumerable) sfp).GetEnumerator();
			int i = 0;
			while (en.MoveNext())
			{
				i++;
			}
			Assert.That(i, Is.EqualTo(2));
		}

		[Test]
		public void WorkWitheDefaultAndSpecificSessionFactory()
		{
			var sfp = new MultiSessionFactoryProvider();
			Assert.That(sfp.GetFactory(null), Is.Not.Null);
			ISessionFactory sf1 = sfp.GetFactory(null);
			ISessionFactory sf2 = sfp.GetFactory(null);
			Assert.That(ReferenceEquals(sf1, sf2));

			ISessionFactory sfp1 = sfp.GetFactory("FACTORY1");
			Assert.That(ReferenceEquals(sf1, sfp1));
			ISessionFactory sfp2 = sfp.GetFactory("FACTORY2");
			Assert.That(!ReferenceEquals(sfp1, sfp2));
		}

		[Test]
		public void ShouldInitializeOnlyOneTime()
		{
			var sfp = new MultiSessionFactoryProvider();
			Assert.That(sfp.GetFactory(null), Is.Not.Null);

			Assert.That(Spying.Logger<MultiSessionFactoryProvider>().Execute(sfp.Initialize).WholeMessage,
			            Text.DoesNotContain("Initialize new session factories reading the configuration."));
		}

		[Test]
		public void ThrowSpecificExceptionForInvalidFactoryName()
		{
			var sfp = new MultiSessionFactoryProvider();
			sfp.Initialize();
			try
			{
				sfp.GetFactory("NotExistFactory");
			}
			catch (ArgumentException e)
			{
				Assert.That(e.Message, Text.StartsWith("The session-factory-id was not register"));
			}
		}

		public class SpecificFileConfigurationLoader : AbstractConfigurationProvider
		{
			private readonly string filePath;

			public SpecificFileConfigurationLoader(string filePath)
			{
				this.filePath = filePath;
			}

			public override IEnumerable<Configuration> Configure()
			{
				var cfg = new Configuration();
				bool configured;
				DoBeforeConfigure(cfg, out configured);
				if (!configured)
				{
					cfg.Configure(filePath);
				}
				DoAfterConfigure(cfg);
				return new SingletonEnumerable<Configuration>(cfg);
			}
		}

		[Test]
		public void ConfiguringOneSessionFactoryWithoutName()
		{
			var sfp = new MultiSessionFactoryProvider(new SpecificFileConfigurationLoader("TestConfigWithFactoryName.xml"));
			Assert.Throws<ArgumentException>(() => sfp.GetFactory(null));
		}
	}
}