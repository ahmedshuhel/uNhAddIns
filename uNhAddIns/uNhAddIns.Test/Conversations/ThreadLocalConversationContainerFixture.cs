using System;
using NUnit.Framework;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class ThreadLocalConversationContainerFixture
	{
		private class ThreadLocalConversationContainerStub : ThreadLocalConversationContainer
		{
			// The fixture is executed in one trhead so we need something to clean the cotainer
			// at the end of each test.
			public void Reset()
			{
				store.Clear();
				currentId = null;
			}

			public int BindedConversationCount { get { return store.Count; } }
		}
	
		[Test]
		public void Get()
		{
			var tc = new ThreadLocalConversationContainerStub();
			Assert.Throws<ArgumentNullException>(() => tc.Get(null));
			Assert.That(tc.Get(""), Is.Null);
			Assert.That(tc.Get("anyThing"), Is.Null);

			tc.Bind(new TestConversation("theKey"));
			Assert.That(tc.Get("theKey"), Is.Not.Null);
			tc.Reset();
		}

		[Test]
		public void CurrentConversation()
		{
			var tc = new ThreadLocalConversationContainerStub();
			Assert.That(tc.CurrentConversation, Is.Null);
			tc.Reset();
		}

		[Test]
		public void Unbind()
		{
			var tc = new ThreadLocalConversationContainerStub();
			Assert.That(tc.Unbind(null), Is.Null);
			Assert.That(tc.Unbind(""), Is.Null);

			IConversation c = new TestConversation();
			tc.Bind(c);
			Assert.That(tc.Unbind(c.Id), Is.SameAs(c));
			Assert.That(tc.BindedConversationCount, Is.EqualTo(0));
			Assert.That(tc.CurrentConversation, Is.Null);

			tc.Reset();
		}

		[Test]
		public void Bind()
		{
			var tc = new ThreadLocalConversationContainerStub();
			IConversation c = new TestConversation();
			tc.Bind(c);
			Assert.That(tc.BindedConversationCount, Is.EqualTo(1));
			Assert.That(tc.CurrentConversation, Is.SameAs(c));
			tc.Bind(new TestConversation());
			Assert.That(tc.BindedConversationCount, Is.EqualTo(2));
			Assert.That(tc.CurrentConversation, Is.SameAs(c),"the current conversation was changed after a bind.");
			
			tc.Reset();
		}

		[Test]
		public void SetAsCurrentWithConversation()
		{
			var tc = new ThreadLocalConversationContainerStub();
			Assert.Throws<ArgumentNullException>(() => tc.SetAsCurrent((IConversation)null));
			tc.Bind(new TestConversation());
			IConversation c = new TestConversation();
			tc.SetAsCurrent(c);
			Assert.That(tc.BindedConversationCount, Is.EqualTo(2));
			Assert.That(tc.CurrentConversation, Is.SameAs(c));
			tc.SetAsCurrent(c);
			Assert.That(tc.BindedConversationCount, Is.EqualTo(2),"a new conversation was added even if is contained.");
			Assert.That(tc.CurrentConversation, Is.SameAs(c));
			tc.Reset();
		}

		[Test]
		public void SetAsCurrentWithId()
		{
			var tc = new ThreadLocalConversationContainerStub();
			Assert.Throws<ArgumentNullException>(() => tc.SetAsCurrent((string)null));
			IConversation c1 = new TestConversation();
			tc.Bind(c1);
			IConversation c2 = new TestConversation();
			Assert.Throws<ConversationException>(() => tc.SetAsCurrent(c2.Id));
			Assert.That(tc.BindedConversationCount, Is.EqualTo(1));
			Assert.That(tc.CurrentConversation, Is.SameAs(c1));
			tc.Bind(c2);
			tc.SetAsCurrent(c2.Id);
			Assert.That(tc.CurrentConversation, Is.SameAs(c2));
			tc.Reset();
		}

		[Test]
		public void FullPlay()
		{
			var tc1 = new ThreadLocalConversationContainerStub();
			var tc2 = new ThreadLocalConversationContainerStub();
			IConversation c1 = new TestConversation();
			IConversation c2 = new TestConversation();
			tc1.Bind(c1);
			tc2.Bind(c2);

			Assert.That(tc1.BindedConversationCount, Is.EqualTo(2));
			Assert.That(tc2.CurrentConversation, Is.SameAs(c1));
			tc2.SetAsCurrent(c2);
			Assert.That(tc1.CurrentConversation, Is.SameAs(c2));
			Assert.That(tc1.Unbind(c2.Id), Is.SameAs(c2));
			Assert.That(tc1.BindedConversationCount, Is.EqualTo(1));
			Assert.That(tc2.CurrentConversation, Is.SameAs(c1));
			tc1.Reset();			
		}

		[Test]
		public void TheAutoUnBindDefaultShouldBeTrue()
		{
			var tc = new ThreadLocalConversationContainerStub();
			IConversation c = new TestConversation();
			tc.Bind(c);
			c.End();
			Assert.That(tc.BindedConversationCount, Is.EqualTo(0));
			tc.Reset();
		}

		[Test]
		public void ShouldWorkWithAutoUnBindFalse()
		{
			var tc = new ThreadLocalConversationContainerStub { AutoUnbindAfterEndConversation = false };
			IConversation c = new TestConversation();
			tc.Bind(c);
			c.End();
			Assert.That(tc.BindedConversationCount, Is.EqualTo(1));
			tc.Reset();
		}
	}
}