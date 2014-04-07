using NUnit.Framework;
using uNhAddIns.Adapters.Common;
using uNhAddIns.TestUtils;

namespace uNhAddIns.Adapters.CommonTests.ConversationManagement
{
	[TestFixture]
	public class ReflectionConversationalMetaInfoInspectorFixture
	{
		[PersistenceConversational]
		private class Sample
		{
			[PersistenceConversation]
			public void PersistentMethod() {}

			[PersistenceConversation(Exclude = true)]
			public void PersistentMethodExcluded() {}

			public void NoPersistentMethod() {}
		}

		[Test]
		public void ShouldIncludeClass()
		{
			var inspector = new ReflectionConversationalMetaInfoInspector();
			inspector.GetInfo(typeof (Sample)).Should().Not.Be.Null();
		}

		[Test]
		public void ShouldIncludeMethod()
		{
			var inspector = new ReflectionConversationalMetaInfoInspector();
			inspector.GetMethodInfo(Reflector.MethodInfo<Sample>(x => x.PersistentMethod())).Should().Not.Be.Null();
		}

		[Test]
		public void ShouldNotIncludeAnyClass()
		{
			var inspector = new ReflectionConversationalMetaInfoInspector();
			inspector.GetInfo(typeof (object)).Should().Be.Null();
		}

		[Test]
		public void ShouldNotIncludeMethodNotMarked()
		{
			var inspector = new ReflectionConversationalMetaInfoInspector();
			inspector.GetMethodInfo(Reflector.MethodInfo<Sample>(x => x.NoPersistentMethod())).Should().Be.Null();
		}

		[Test]
		public void ShouldReturnInfoEvenForExcludedMethods()
		{
			var inspector = new ReflectionConversationalMetaInfoInspector();
			inspector.GetMethodInfo(Reflector.MethodInfo<Sample>(x => x.PersistentMethodExcluded())).Should().Not.Be.Null();
		}
	}
}