using System;
using System.Collections.Generic;
using NHibernate;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.SessionEasier.Conversations;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class NHConversationFixture : TestCase
	{
		protected override IList<string> Mappings
		{
			get { return new[] {"Conversations.Silly.hbm.xml"}; }
		}

		protected override void OnTearDown()
		{
			CommitInNewSession(session => session.Delete("from Silly3"));
		}

		private void AssertExistsInDb(Silly3 persistedObject)
		{
			Assert.That(ExistsInDb<Silly3>(persistedObject.Id), Is.True, "object expected to be in db");
		}

		private void AssertDoesNotExistInDb(Silly3 persistedObject)
		{
			Assert.That(ExistsInDb<Silly3>(persistedObject.Id), Is.False, "object expected not to be in db");
		}

		private void AssertIsOpen(ISession s)
		{
			Assert.That(s, Is.Not.Null);
			Assert.That(s.IsOpen);
		}

		private void AssertIsPaused(ISession s)
		{
			AssertIsOpen(s);
			//Assert.That(s.IsConnected, Is.False); need something else in NH
			Assert.That(!s.Transaction.IsActive);
		}

		private NhConversation NewConversation()
		{
			return new NhConversation(new SessionFactoryProviderStub(sessions), new FakeSessionWrapper());
		}

		private NhConversation NewStartedConversation()
		{
			NhConversation c = NewConversation();
			c.Start();
			ISession s = c.GetSession(sessions);
			Assert.That(s.IsOpen, Is.True, "UoW expected to be started");
			return c;
		}

		private NhConversation NewPausedConversation()
		{
			NhConversation c = NewConversation();
			c.Start();
			c.Pause();
			AssertIsPaused(c.GetSession(sessions));
			return c;
		}

		[Test]
		public void ConversationUsage()
		{
			CommitInNewSession(session =>
			                   	{
			                   		var o = new Other3 {Name = "some other silly"};
			                   		var e = new Silly3 {Name = "somebody", Other = o};
			                   		session.Save(e);
			                   	});

			using (NhConversation c = NewConversation())
			{
				c.Start();
				ISession s = c.GetSession(sessions);
				IList<Silly3> sl = s.CreateQuery("from Silly3").List<Silly3>();
				c.Pause();
				Assert.That(sl.Count == 1);
				Assert.That(!NHibernateUtil.IsInitialized(sl[0].Other));
				// working with entities, even using lazy loading
				Assert.That(!s.Transaction.IsActive);
				Assert.That("some other silly", Is.EqualTo(sl[0].Other.Name));
				Assert.That(NHibernateUtil.IsInitialized(sl[0].Other));
				sl[0].Other.Name = "nobody";
				c.Resume();
				s = c.GetSession(sessions);
				s.SaveOrUpdate(sl[0]);
				// the dispose auto-end the conversation
			}

			using (NhConversation c = NewConversation())
			{
				c.Start();
				ISession s = c.GetSession(sessions);
				s.Delete("from Silly3");
				c.End();
			}
		}

		[Test]
		public void Ctor()
		{
			Assert.Throws<ArgumentNullException>(() => new NhConversation(null, null));
			Assert.Throws<ArgumentNullException>(() => new NhConversation(null, null, "aKey"));
			Assert.Throws<ArgumentNullException>(() => new NhConversation(new SessionFactoryProviderStub(sessions), null));
			Assert.Throws<ArgumentNullException>(() => new NhConversation(new SessionFactoryProviderStub(sessions), null, "aKey"));
		}

		[Test]
		public void Destructor()
		{
			NhConversation c = NewStartedConversation();
			ISession s = c.GetSession(sessions);
			c.Dispose();
			Assert.That(s, Is.Not.Null);
			Assert.That(s.IsOpen, Is.False);
			Assert.Throws<ConversationException>(() => s = c.GetSession(sessions));
		}

		[Test]
		public void EndShouldCloseStartedUnitOfWork()
		{
			NhConversation c = NewStartedConversation();

			c.End();
			ISession s = c.GetSession(sessions);
			Assert.That(s, Is.Not.Null);
			Assert.That(s.IsOpen, Is.False);
		}

		[Test]
		public void EndShouldFlushResumedUnitOfWork()
		{
			NhConversation c = NewPausedConversation();
			c.Resume();
			ISession s = c.GetSession(sessions);
			var persistedObj = new Silly3();
			s.Save(persistedObj);

			c.End();
			AssertExistsInDb(persistedObj);
		}

		[Test]
		public void EndShouldFlushStartedUnitOfWork()
		{
			NhConversation c = NewStartedConversation();
			ISession s = c.GetSession(sessions);
			var persistedObj = new Silly3();
			s.Save(persistedObj);

			c.End();
			AssertExistsInDb(persistedObj);
		}

		[Test]
		public void EndShouldNotFlushPausedUnitOfWork()
		{
			NhConversation c = NewPausedConversation();
			ISession s = c.GetSession(sessions);
			var persistedObj = new Silly3();
			s.Save(persistedObj);

			c.End();
			AssertDoesNotExistInDb(persistedObj);
		}

		[Test]
		public void EndWithoutStartShouldNotThrow()
		{
			NhConversation c = NewConversation();
			Assert.DoesNotThrow(c.End);
		}

		[Test]
		public void GetSessionsShouldThrowWhenConversationNotYetStarted()
		{
			NhConversation c = NewConversation();
			Assert.Throws<ConversationException>(() => c.GetSession(sessions));
		}

		[Test]
		public void PauseShouldDisconnectButNotCloseUnitOfWork()
		{
			NhConversation c = NewStartedConversation();

			c.Pause();
			ISession s = c.GetSession(sessions);
			AssertIsPaused(s);
		}

		[Test]
		public void PauseShouldNotFlushUnitOfWork()
		{
			NhConversation c = NewStartedConversation();
			var persistedObj = new Silly3();
			c.GetSession(sessions).Save(persistedObj);

			c.Pause();
			AssertDoesNotExistInDb(persistedObj);
		}

		[Test]
		public void PauseWithoutStartShouldNotThrow()
		{
			NhConversation c = NewConversation();
			Assert.DoesNotThrow(c.Pause);
		}

		[Test]
		public void ResumeAfterEndShouldStartAnotherUnitOfWork()
		{
			NhConversation c = NewPausedConversation();
			c.Resume();
			ISession previousUoW = c.GetSession(sessions);
			c.End();

			c.Resume();
			ISession currentUoW = c.GetSession(sessions);
			AssertIsOpen(currentUoW);
			Assert.That(currentUoW, Is.Not.SameAs(previousUoW));
		}

		[Test]
		public void ResumeAfterPauseShouldNotStatAnotherUnitOfWork()
		{
			NhConversation c = NewPausedConversation();
			ISession pausedUoW = c.GetSession(sessions);

			c.Resume();
			ISession currentUoW = c.GetSession(sessions);
			Assert.That(currentUoW, Is.SameAs(pausedUoW));
		}

		[Test]
		public void ResumeAfterStartShouldNotStatAnotherUnitOfWork()
		{
			NhConversation c = NewStartedConversation();
			ISession startedUoW = c.GetSession(sessions);

			c.Resume();
			ISession currentUoW = c.GetSession(sessions);
			Assert.That(currentUoW, Is.SameAs(startedUoW));
		}

		[Test]
		public void ResumeShouldReconnectUnitOfWork()
		{
			NhConversation c = NewPausedConversation();

			c.Resume();
			ISession s = c.GetSession(sessions);
			AssertIsOpen(s);
			Assert.That(s.IsConnected);
		}

		[Test]
		public void ResumeWithoutPauseShouldNotThrow()
		{
			NhConversation c = NewStartedConversation();
			Assert.DoesNotThrow(c.Resume);
		}

		[Test]
		public void ResumeWithoutStartShouldNotThrow()
		{
			NhConversation c = NewConversation();
			Assert.DoesNotThrow(c.Resume);
		}

		[Test]
		public void StartAfterEndShouldStartAnotherUnitOfWork()
		{
			NhConversation c = NewStartedConversation();
			ISession previousUoW = c.GetSession(sessions);
			c.End();

			c.Start();
			ISession currentUoW = c.GetSession(sessions);
			AssertIsOpen(currentUoW);
			Assert.That(currentUoW, Is.Not.SameAs(previousUoW));
		}

		[Test]
		public void StartCalledTwiceWithoutEndShouldNotStartAnotherUnitOfWork()
		{
			NhConversation c = NewConversation();
			c.Start();
			ISession s = c.GetSession(sessions);

			c.Start();
			Assert.That(c.GetSession(sessions), Is.SameAs(s), "ReStart of the same conversation should not change the session.");
			Assert.That(s.IsOpen);
		}

		[Test]
		public void StartedUnitOfWorkShouldNotFlushToDatabaseUntilEndOfConversation()
		{
			NhConversation c = NewStartedConversation();

			ISession s = c.GetSession(sessions);

			Assert.That(s.FlushMode, Is.EqualTo(FlushMode.Never));
		}

		[Test]
		public void StartShouldStartUnitOfWork()
		{
			NhConversation c = NewConversation();

			c.Start();
			ISession s = c.GetSession(sessions);
			AssertIsOpen(s);
		}

		[Test]
		public void FlushAndPauseShouldFlushStartedUnitOfWork()
		{
			NhConversation c = NewStartedConversation();
			ISession s = c.GetSession(sessions);
			var persistedObj = new Silly3();
			s.Save(persistedObj);

			c.FlushAndPause();
			AssertExistsInDb(persistedObj);
			AssertIsPaused(c.GetSession(sessions));
		}

		[Test]
		public void FlushAndPauseShouldNotCloseTheSession()
		{
			NhConversation c = NewStartedConversation();
			c.FlushAndPause();
			AssertIsPaused(c.GetSession(sessions));
		}

		[Test]
		public void SessionCloseOutsideTheConversation()
		{
			NhConversation c = NewStartedConversation();
			ISession session = c.GetSession(sessions);
			session.Close();
			Assert.That(c.GetSession(sessions), Is.SameAs(session));
			Assert.DoesNotThrow(c.Pause);
			Assert.That(Spying.Logger<NhConversation>().Execute(c.Resume).WholeMessage,
									Text.DoesNotContain("Already session bound on call to Bind()"),
									"No warning is needed for a closed session.");
			Assert.That(c.GetSession(sessions), Is.Not.SameAs(session));
			c.Dispose();
		}

		[Test]
		public void SessionDisposeOutsideTheConversation()
		{
			NhConversation c = NewStartedConversation();
			using(c.GetSession(sessions))
			{
				// Do nothing only simulate a possible usage
				// of sessions.GetCurrentSession() outside conversation management
			}
			// Need some new events in NH about session.Close (Event/Listener)
			// Assert.That(c.GetSession(sessions), Is.Null, "should auto unbind the session.");
			Assert.DoesNotThrow(c.Pause);
			Assert.That(Spying.Logger<NhConversation>().Execute(c.Resume).WholeMessage,
									Text.DoesNotContain("Already session bound on call to Bind()"),
									"No warning is needed for a closed session.");
			Assert.That(c.GetSession(sessions), Is.Not.Null);
			c.Dispose();
		}

		[Test]
		public void ManualBindSessionToConversationShouldUnbindOrphanedSession()
		{
			// TODO: recreate this test in ConversationFixtureBase to check the wrapper
			NhConversation c = NewStartedConversation();

			ISession s = sessions.OpenSession();
			Assert.That(Spying.Logger<NhConversation>().Execute(() => c.Bind(s)).WholeMessage,
			            Text.Contains("Already session bound on call to Bind()"));
			ISession sessionFromConversation = c.GetSession(sessions);
			Assert.That(c.Wrapper.IsWrapped(sessionFromConversation), "The new bind session should be wrapped and managed by the Wrapper.");
			Assert.That(sessionFromConversation.FlushMode, Is.EqualTo(FlushMode.Never), "The FlushMode of new bind session should be changed to Never.");
		}

		[Test]
		public void ManualUnbindSessionDoNotCreateProblemsInARunningConversation()
		{
			NhConversation c = NewStartedConversation();

			ISession s = c.GetSession(sessions);
			c.Unbind(s);
			Assert.That(c.GetSession(sessions), Is.Null, "A session still bind after manual Unbind.");
			c.Resume();
			Assert.That(c.GetSession(sessions).IsOpen);
			c.End();
		}
	}
}