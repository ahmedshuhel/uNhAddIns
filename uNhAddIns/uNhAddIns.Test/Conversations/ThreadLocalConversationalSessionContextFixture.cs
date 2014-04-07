using System.Collections.Generic;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class ThreadLocalConversationalSessionContextFixture: TestCase
	{
		private IConversationFactory cf;
		private IConversationsContainerAccessor cca;

		protected override IList<string> Mappings
		{
            get { return new[] { "Conversations.Silly.hbm.xml" }; }
		}

		protected override void Configure(Configuration configuration)
		{
			configuration.Properties[Environment.CurrentSessionContextClass] =
				typeof (ThreadLocalConversationalSessionContext).AssemblyQualifiedName;
			base.Configure(configuration);
		}

		[TestFixtureSetUp]
		public void CreateCoversationStuff()
		{
			TestFixtureSetUp();
			var provider = new SessionFactoryProviderStub(sessions);
			cf = new DefaultConversationFactory(provider, new FakeSessionWrapper());
			cca = new NhConversationsContainerAccessor(provider);
		}

		[Test]
		public void ConversationUsage()
		{
			Assert.Throws<ConversationException>(() => sessions.GetCurrentSession());
			var tc1 = cca.Container;
		    tc1.Bind(cf.CreateConversation("1"));
		    tc1.Bind(cf.CreateConversation("2"));

			var dao = new SillyDao(sessions);
			tc1.SetAsCurrent("1");
			tc1.CurrentConversation.Start();
			var o = new Other3 { Name = "some other silly" };
            var e = new Silly3 { Name = "somebody", Other = o };
			dao.MakePersistent(e);
			tc1.CurrentConversation.Pause();

			tc1.SetAsCurrent("2");
            tc1.CurrentConversation.Start();
            IList<Silly3> sl = dao.GetAll();
            Assert.That(sl.Count, Is.EqualTo(0), "changes in c1 should not have been flushed to db yet");
            tc1.CurrentConversation.Pause();

            tc1.SetAsCurrent("1");
		    tc1.CurrentConversation.Resume();
            tc1.CurrentConversation.End(); //commit the changes

            tc1.SetAsCurrent("2");
            using (var c2 = tc1.CurrentConversation)
            {
                c2.Start();
                sl = dao.GetAll();
                c2.Pause();
                Assert.That(sl.Count, Is.EqualTo(1), "changes in c1 should now be in db");
                Assert.That(!NHibernateUtil.IsInitialized(sl[0].Other));
                // working with entities, even using lazy loading no cause problems
                Assert.That("some other silly", Is.EqualTo(sl[0].Other.Name));
                Assert.That(NHibernateUtil.IsInitialized(sl[0].Other));
                sl[0].Other.Name = "nobody";
                c2.Resume();
                dao.MakePersistent(sl[0]);
            }

			Assert.Throws<ConversationException>(() => tc1.SetAsCurrent("1"));
			Assert.Throws<ConversationException>(() => tc1.SetAsCurrent("2"));
		}

        protected override void OnTearDown()
        {
            CommitInNewSession(session => session.Delete("from Silly3"));
        }
	}
}