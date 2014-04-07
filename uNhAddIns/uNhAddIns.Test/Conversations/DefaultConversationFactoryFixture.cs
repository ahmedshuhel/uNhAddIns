using System;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class DefaultConversationFactoryFixture
	{
		[Test]
		public void CTorProtection()
		{
			Assert.Throws<ArgumentNullException>(() => new DefaultConversationFactory(null, null));
			Assert.Throws<ArgumentNullException>(() => new DefaultConversationFactory(new SessionFactoryProvider(), null));
			Assert.Throws<ArgumentNullException>(() => new DefaultConversationFactory(null, new FakeSessionWrapper()));
		}

		[Test]
		public void CreateNewConversationWithoutId()
		{
			var cf = new DefaultConversationFactory(new SessionFactoryProvider(), new FakeSessionWrapper());
			var nc = cf.CreateConversation();
			Assert.That(nc, Is.Not.Null);
			Assert.That(cf.CreateConversation(), Is.Not.EqualTo(nc));
		}

		[Test]
		public void CreateConversationWithId()
		{
			var cf = new DefaultConversationFactory(new SessionFactoryProvider(), new FakeSessionWrapper());
			var nc = cf.CreateConversation("MyId");
			Assert.That(nc, Is.Not.Null);
			Assert.That(cf.CreateConversation("MyId"), Is.Not.SameAs(nc));
		}
	}
}