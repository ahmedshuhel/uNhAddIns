using NUnit.Framework;
using uNhAddIns.SessionEasier.Conversations;
using System;

namespace uNhAddIns.Test.Conversations
{
	[TestFixture]
	public class ConversationExceptionFixture
	{
		[Test]
		public void IsSerializable()
		{
			Assert.That(typeof(ConversationException), Has.Attribute<SerializableAttribute>());
			var e = new ConversationException("a message");
			Assert.That(e, Is.BinarySerializable);
		}
	}
}