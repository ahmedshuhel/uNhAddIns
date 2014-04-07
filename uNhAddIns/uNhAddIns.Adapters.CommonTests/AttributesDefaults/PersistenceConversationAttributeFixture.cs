using NUnit.Framework;

namespace uNhAddIns.Adapters.CommonTests.AttributesDefaults
{
	[TestFixture]
	public class PersistenceConversationAttributeFixture
	{
		[Test]
		public void ShouldWorkWithDefaultValues()
		{
			// This test is useful to check future Breaking-changes
			var a = new PersistenceConversationAttribute();
			Assert.That(a.ConversationEndMode, Is.EqualTo(EndMode.Unspecified));
			Assert.That(a.Exclude, Is.False);
		}
	}
}