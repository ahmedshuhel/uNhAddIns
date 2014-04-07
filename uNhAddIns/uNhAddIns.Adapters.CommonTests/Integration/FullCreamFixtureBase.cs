using System;
using System.Collections.Generic;
using System.Linq;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using uNhAddIns.TestUtils.Logging;
using uNhAddIns.TestUtils.NhIntegration;

namespace uNhAddIns.Adapters.CommonTests.Integration
{
	public abstract class FullCreamFixtureBase
	{
		private const string NHibernateFlushEventsLoggerName = "NHibernate.Event.Default.AbstractFlushingEventListener";
		private const string NHibernateSessionLoggerName = "NHibernate.Impl.SessionImpl";

		private const string ExecutingFlushMessage = "executing flush";
		private const string SessionDisposedMessage = "running ISession.Dispose";
		private const string SessionOpenedMessage = "opened session";

		protected FullCreamFixtureBase()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Initialize the ServiceLocator registering all services needed by this test.
		/// </summary>
		/// <remarks>
		/// The initialization must include the initialization of <see cref="ServiceLocator"/>.
		/// 
		/// Services needed, in this test, are:
		/// 
		/// - uNhAddIns.SessionEasier.ISessionFactoryProvider 
		///		This is a factory where we need to use GetFactory
		///		Implementation: uNhAddIns.SessionEasier.SessionFactoryProvider
		/// 
		/// - uNhAddIns.SessionEasier.ISessionWrapper
		///		Implementation: uNhAddIns.YourAdapter.SessionWrapper if available
		/// 
		/// - uNhAddIns.SessionEasier.Conversations.IConversationFactory
		///		Implementation: uNhAddIns.SessionEasier.Conversations.DefaultConversationFactory
		/// 
		/// - uNhAddIns.SessionEasier.Conversations.IConversationsContainerAccessor
		///		Implementation: uNhAddIns.SessionEasier.Conversations.NhConversationsContainerAccessor
		/// 
		/// - NHibernate.ISessionFactory
		///		Taking the instance using ISessionFactoryProvider.GetFactory with a string parameter setted
		///		using the name of session-factory-configuration (from available template the name is "uNhAddIns")
		/// 
		/// - Microsoft.Practices.ServiceLocation.IServiceLocator
		///		The service locator itself is used by the implementation of DaoFactory
		/// 
		/// - uNhAddIns.Adapters.CommonTests.IDaoFactory
		///		Implementation: uNhAddIns.Adapters.CommonTests.Integration.DaoFactory
		/// 
		/// - uNhAddIns.Adapters.CommonTests.ISillyDao
		///		Implementation : uNhAddIns.Adapters.CommonTests.Integration.SillyDao
		/// 
		/// - uNhAddIns.Adapters.CommonTests.ISillyCrudModel
		///		Implementation : uNhAddIns.Adapters.CommonTests.Integration.SillyCrudModel
		///		Lyfe : Transient
		/// 
		/// - uNhAddIns.Adapters.CommonTests.ISillyReportModel
		///		Implementation : uNhAddIns.Adapters.CommonTests.Integration.SillyReportModel
		///		Lyfe : Transient
		/// </remarks>
		protected abstract void InitializeServiceLocator();

		[TestFixtureSetUp]
		public void CreateDb()
		{
			Configuration cfg = new Configuration().Configure();
			new SchemaExport(cfg).Create(false, true);
			InitializeServiceLocator();
		}

		[TearDown]
		public void TearDown()
		{
			var factory = ServiceLocator.Current.GetInstance<ISessionFactory>();
			factory.EncloseInTransaction(session => session.Delete("from Silly"));
		}

		[Test]
		public void ShouldNotStartUnitOfWorkWhenConversationModelFirstCreated()
		{
			Assert.That(
				Spying.Logger(NHibernateSessionLoggerName).Execute(() => ServiceLocator.Current.GetInstance<ISillyCrudModel>()).
					WholeMessage, Text.DoesNotContain(SessionOpenedMessage));
		}

		[Test]
		public void ShouldStartUnitOfWorkWhenFirstMethodCalledOnConversationModel()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();

			Assert.That(Spying.Logger(NHibernateSessionLoggerName).Execute(() => scm.GetEntirelyList()).WholeMessage,
			            Text.Contains(SessionOpenedMessage));
		}

		[Test]
		public void ShouldRetainUnitOfWorkBetweenMethodCallsToConversationModel()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			Assert.That(Spying.Logger(NHibernateSessionLoggerName).Execute(() =>
			                                                               	{
			                                                               		scm.GetEntirelyList();
			                                                               		scm.GetEntirelyList();
			                                                               	}).WholeMessage,
			            Text.DoesNotContain(SessionDisposedMessage));
		}

		[Test]
		public void SusequentCallsShouldExecuteWithinSameUnitOfWork()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			Assert.That(Spying.Logger(NHibernateSessionLoggerName).Execute(() =>
			                                                               	{
			                                                               		scm.GetEntirelyList();
			                                                               		scm.GetEntirelyList();
			                                                               	}).MessageSequence.Where(
			            	msg => msg.Contains(SessionOpenedMessage)).Count(), Is.EqualTo(1));
		}

		[Test]
		public void CallingMethodThatEndsConversationShouldFlushUnitOfWork()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			scm.Save(new Silly());
			Assert.That(Spying.Logger(NHibernateFlushEventsLoggerName).Execute(scm.AcceptAll).WholeMessage,
			            Text.Contains(ExecutingFlushMessage));
		}

		[Test]
		public void SubsequentCallsAfterEndingConversationShouldStartAnotherUnitOfWork()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			scm.Save(new Silly());
			scm.AcceptAll();

			Assert.That(Spying.Logger(NHibernateSessionLoggerName).Execute(() => scm.GetEntirelyList()).WholeMessage,
			            Text.Contains(SessionOpenedMessage));
		}

		[Test]
		public void CallingMethodThatAbortsConversationShouldThrowAwayUnitOfWork()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			scm.Save(new Silly());

			//when
			string msgs = Spying.Logger(NHibernateSessionLoggerName).Execute(scm.Abort).WholeMessage;

			//then
			Assert.That(msgs, Text.DoesNotContain(ExecutingFlushMessage));
			Assert.That(msgs, Text.Contains(SessionDisposedMessage));
		}

		[Test]
		public void SubsequentCallsAfterAbortingConversationShouldStartAnotherUnitOfWork()
		{
			//given
			var scm = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			scm.Save(new Silly());
			scm.Abort();

			Assert.That(Spying.Logger(NHibernateSessionLoggerName).Execute(() => scm.GetEntirelyList()).WholeMessage,
			            Text.Contains(SessionOpenedMessage));
		}

		[Test]
		public void SimultaneousConversationExample()
		{
			// This test shows what happens using something else than identity
			var scm1 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			var scm2 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			Assert.That(scm1, Is.Not.SameAs(scm2),
			            "The model is expected to be configured to return a new instance for every ISillyCrudModel.");

			IList<Silly> l1 = scm1.GetEntirelyList();
			Assert.That(l1.Count, Is.EqualTo(0));

			// We save in a conversation, and then retrieve the results from a different one.
			var s = new Silly {Name = "fiamma"};
			scm1.Save(s);
			Assert.That(s.Id, Is.Not.EqualTo(0));
			Guid savedId = s.Id;
			scm1.AcceptAll(); // <== To have a result available to other conversation you must end yours

			IList<Silly> l2 = scm2.GetEntirelyList();
			Assert.That(l2.Count, Is.EqualTo(1));
			Assert.That("fiamma", Is.EqualTo(l2[0].Name));
			Assert.That(l2[0], Is.Not.SameAs(s), "the two instances of Silly should be of a different session");
			scm2.Delete(l2[0]);
			scm2.AcceptAll();

			// the conversation should resume and the entity should not be associated to the current session
			Assert.That(scm1.GetIfAvailable(savedId), Is.Null);
			scm1.AcceptAll();
		}

		[Test]
		public void ConversationAbort()
		{
			var scm1 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();

			IList<Silly> l1 = scm1.GetEntirelyList();
			Assert.That(l1.Count, Is.EqualTo(0));

			var s = new Silly {Name = "fiamma"};
			scm1.Save(s);
			Assert.That(s.Id, Is.Not.EqualTo(0), "The entity didn't receive the hilo id!?!");
			Guid savedId = s.Id;
			scm1.Abort();

			var scm2 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			Assert.That(scm2.GetIfAvailable(savedId), Is.Null, "If it was aborted then it is not present in another conversation");
			scm2.AcceptAll();
		}

		[Test]
		public void ConversationCommitAndContinue()
		{
			var scm1 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();

			IList<Silly> l1 = scm1.GetEntirelyList();
			Assert.That(l1.Count, Is.EqualTo(0));

			var s = new Silly {Name = "fiamma"};
			scm1.Save(s);
			Assert.That(s.Id, Is.Not.EqualTo(0), "The entity didn't receive the hilo id!?!");
			scm1.AcceptAll();
			Guid savedId = s.Id;

			scm1.ImmediateDelete(s);

			var scm2 = ServiceLocator.Current.GetInstance<ISillyCrudModel>();
			Silly silly = scm2.GetIfAvailable(savedId);
			Assert.That(silly, Is.Null, "Silly was supposed to be removed immediately");
		}


		[Test]
		public void ShouldWorkWithAllowOutsidePersistentCall()
		{
			var model = ServiceLocator.Current.GetInstance<ISillyReportModel>();
			var sillies = model.GetSillies().ToList();
            Assert.AreEqual(sillies.Count, 0);

		}

		[Test]
		public void ShouldWorkWithAllowOutsidePersistentCallWhenCommitTransaction()
		{
			var model = ServiceLocator.Current.GetInstance<ISillyReportModel>();
			model.GetSillies().ToList();
			model.End();
			
		}
	}
}