using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using log4net.Config;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace uNhAddIns.Test
{
	/// <summary>
	/// Ported from NH oficial tests.
	/// </summary>
	public abstract class TestCase
	{
		private const bool OutputDdl = false;
		protected Configuration cfg;
		protected ISessionFactoryImplementor sessions;

		private static readonly ILog log = LogManager.GetLogger(typeof (TestCase));

		private ISession lastOpenedSession;
		private DebugConnectionProvider connectionProvider;

		/// <summary>
		/// Mapping files used in the TestCase
		/// </summary>
		protected abstract IList<string> Mappings { get; }

		/// <summary>
		/// Assembly to load mapping files from (default is NHibernate.DomainModel).
		/// </summary>
		protected virtual string MappingsAssembly
		{
			get { return "uNhAddIns.Test"; }
		}

		public ISession LastOpenedSession
		{
			get { return lastOpenedSession; }
		}

		static TestCase()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Creates the tables used in this TestCase
		/// </summary>
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			try
			{
				Configure();
				CreateSchema();
				BuildSessionFactory();
			}
			catch (Exception e)
			{
				log.Error("Error while setting up the test fixture", e);
				throw;
			}
		}

		/// <summary>
		/// Removes the tables used in this TestCase.
		/// </summary>
		/// <remarks>
		/// If the tables are not cleaned up sometimes SchemaExport runs into
		/// Sql errors because it can't drop tables because of the FKs.  This 
		/// will occur if the TestCase does not have the same hbm.xml files
		/// included as a previous one.
		/// </remarks>
		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			DropSchema();
			Cleanup();
		}

		protected virtual void OnSetUp() {}

		/// <summary>
		/// Set up the test. This method is not overridable, but it calls
		/// <see cref="OnSetUp" /> which is.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			OnSetUp();
		}

		protected virtual void OnTearDown() {}

		/// <summary>
		/// Checks that the test case cleans up after itself. This method
		/// is not overridable, but it calls <see cref="OnTearDown" /> which is.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			OnTearDown();

			bool wasClosed = CheckSessionWasClosed();
			bool wasCleaned = CheckDatabaseWasCleaned();
			bool wereConnectionsClosed = CheckConnectionsWereClosed();
			bool fail = !wasClosed || !wasCleaned || !wereConnectionsClosed;

			if (fail)
			{
				Assert.Fail("Test didn't clean up after itself");
			}
		}

		private bool CheckSessionWasClosed()
		{
			if (lastOpenedSession != null && lastOpenedSession.IsOpen)
			{
				log.Error("Test case didn't close a session, closing");
				lastOpenedSession.Close();
				return false;
			}

			return true;
		}

		private bool CheckDatabaseWasCleaned()
		{
			if (sessions.GetAllClassMetadata().Count == 0)
			{
				// Return early in the case of no mappings, also avoiding
				// a warning when executing the HQL below.
				return true;
			}

			bool empty;
			using (ISession s = sessions.OpenSession())
			{
				empty = s.CreateQuery("from System.Object o").List().Count == 0;
			}

			if (!empty)
			{
				log.Error("Test case didn't clean up the database after itself, re-creating the schema");
				DropSchema();
				CreateSchema();
			}

			return empty;
		}

		private bool CheckConnectionsWereClosed()
		{
			if (connectionProvider == null || !connectionProvider.HasOpenConnections)
			{
				return true;
			}

			log.Error("Test case didn't close all open connections, closing");
			connectionProvider.CloseAllConnections();
			return false;
		}

		protected virtual void Configure()
		{
			cfg = new Configuration();
			Assembly assembly = Assembly.Load(MappingsAssembly);
			Configure(cfg);

			foreach (var file in Mappings)
			{
				cfg.AddResource(MappingsAssembly + "." + file, assembly);
			}
		}

		private void CreateSchema()
		{
			new SchemaExport(cfg).Create(OutputDdl, true);
		}

		private void DropSchema()
		{
			new SchemaExport(cfg).Drop(OutputDdl, true);
		}

		protected virtual void BuildSessionFactory()
		{
			sessions = (ISessionFactoryImplementor) cfg.BuildSessionFactory();
			connectionProvider = sessions.ConnectionProvider as DebugConnectionProvider;
		}

		private void Cleanup()
		{
			sessions.Close();
			sessions = null;
			connectionProvider = null;
			lastOpenedSession = null;
			cfg = null;
		}

		protected virtual ISession OpenSession()
		{
			lastOpenedSession = sessions.OpenSession();
			return lastOpenedSession;
		}

		#region Properties overridable by subclasses

		protected virtual void Configure(Configuration configuration)
		{
			configuration.Configure();
		}

		#endregion

		protected void CommitInNewSession(Action<ISession> executeInNewSession)
		{
			using (ISession session = sessions.OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					executeInNewSession(session);
					session.Flush();
					tx.Commit();
				}
			}
		}

		protected bool ExistsInDb<T>(object id)
		{
			using (ISession s = OpenSession())
			{
				return !ReferenceEquals(s.Get<T>(id), null);
			}
		}
	}
}