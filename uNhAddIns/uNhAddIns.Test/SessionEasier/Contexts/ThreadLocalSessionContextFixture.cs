using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Engine;
using NUnit.Framework;
using uNhAddIns.SessionEasier.Contexts;

namespace uNhAddIns.Test.SessionEasier.Contexts
{
	[TestFixture]
	public class ThreadLocalSessionContextFixture: TestCase
	{
		protected override IList<string> Mappings
		{
			get { return new[] { "SessionEasier.Silly2.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			base.Configure(cfg);
			cfg.SetProperty(Environment.CurrentSessionContextClass, typeof(TestableThreadStaticContext).AssemblyQualifiedName);
			cfg.SetProperty(Environment.GenerateStatistics, "true");
		}

		protected override void BuildSessionFactory()
		{
			base.BuildSessionFactory();
			CurrentSessionContext.Wrapper = TestHelpers.GetSessionWrapper();
		}

		[Test]
		public void ShouldBeWrapped()
		{
			ISession session = sessions.GetCurrentSession();
			var wrapper = CurrentSessionContext.Wrapper;
			Assert.That(wrapper.IsWrapped(session));
			session.Close();
		}

		[Test]
		public void ContextCleanup()
		{
			ISession session = sessions.GetCurrentSession();

			session.BeginTransaction();
			session.Transaction.Commit();
			Assert.IsFalse(session.IsOpen, "session open after txn completion");
			Assert.IsFalse(TestableThreadStaticContext.IsSessionBound(session), "session still bound after txn completion");

			ISession session2 = sessions.GetCurrentSession();
			Assert.IsFalse(session.Equals(session2), "same session returned after txn completion");
			session2.Close();
			Assert.IsFalse(session2.IsOpen, "session open after closing");
			Assert.IsFalse(TestableThreadStaticContext.IsSessionBound(session2), "session still bound after closing");
		}

		[Test]
		public void TransactionProtection()
		{
			using (ISession session = sessions.GetCurrentSession())
			{
				try
				{
					session.CreateQuery("from Silly2");
					Assert.Fail("method other than beginTransaction{} allowed");
				}
				catch (HibernateException)
				{
					// ok
				}
			}
		}
	}

	public class TestableThreadStaticContext : ThreadLocalSessionContext
	{
		private static TestableThreadStaticContext me;

		public TestableThreadStaticContext(ISessionFactoryImplementor factory)
			: base(factory)
		{
			me = this;
		}

		public static bool IsSessionBound(ISession session)
		{
			return Context != null && Context.ContainsKey(me.factory)
			       && Context[me.factory] == session;
		}

		public static bool HasBind()
		{
			return Context != null && Context.ContainsKey(me.factory);
		}
	}
}