using System;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;

namespace uNhAddIns.TestUtils.NhIntegration
{
	public abstract class FunctionalTestCaseTemplate
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FunctionalTestCaseTemplate));

		static FunctionalTestCaseTemplate()
		{
			XmlConfigurator.Configure();
		}

		protected abstract IFunctionalTestSettings Settings { get; set; }

		public Configuration Cfg { get; private set; }

		public ISessionFactoryImplementor SessionFactory { get; private set; }

		public void SetUpNhibernate()
		{
			try
			{
				Configure();
				Settings.SchemaSetup(Cfg);
				BuildSessionFactory();
			}
			catch (Exception e)
			{
				log.Error("Error while setting up NHibernate", e);
				throw;
			}
		}

		public void ShutdownNhibernate()
		{
			Settings.BeforeSchemaShutdown(SessionFactory);
			Settings.SchemaShutdown(Cfg);
			Cleanup();
		}

		private void BuildSessionFactory()
		{
			Settings.BeforeSessionFactoryBuilt(Cfg);
			SessionFactory = (ISessionFactoryImplementor) Cfg.BuildSessionFactory();
			Settings.AfterSessionFactoryBuilt(SessionFactory);
		}

		protected void Configure()
		{
			Cfg = new Configuration();
			Settings.Configure(Cfg);
			Settings.LoadMappings(Cfg);
		}

		private void Cleanup()
		{
			SessionFactory.Close();
			SessionFactory = null;
			Cfg = null;
		}

		protected void AssertAllDataRemovedIfNeeded()
		{
			if (Settings.AssertAllDataRemoved)
			{
				if (!DatabaseWasCleaned())
				{
					Assert.Fail("Test didn't clean up after itself");
				}
			}
		}

		private bool DatabaseWasCleaned()
		{
			if (SessionFactory.GetAllClassMetadata().Count == 0)
			{
				// Return early in the case of no mappings, also avoiding a warning when executing the HQL below.
				return true;
			}

			bool empty;
			using (ISession s = SessionFactory.OpenSession())
			{
				empty = s.CreateQuery("from System.Object o").List().Count == 0;
			}

			if (!empty && Settings.SchemaShutdownAfterFailure)
			{
				log.Error("Test case didn't clean up the database after itself.");
				Settings.SchemaShutdown(Cfg);
				Settings.SchemaSetup(Cfg);
			}

			return empty;
		}
	}
}