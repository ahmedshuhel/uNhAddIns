using System;
using NUnit.Framework;
using uNhAddIns.Adapters.Common;

namespace uNhAddIns.Adapters.CommonTests.ConversationManagement
{
	[TestFixture]
	public class ConversationalMetaInfoStoreFixture
	{
		public class Sample
		{
			public void PersistentMethod() {}
			public void NoPersistentMethod() {}
		}

		[Test]
		public void GetMetadataForUndefinedClass()
		{
			var store = new ConversationalMetaInfoStore();
			store.GetMetadataFor(typeof (object)).Should("GetMetadataFor undefined class should return null").Be.Null();
		}

		[Test]
		public void NotAllowDuplication()
		{
			var settings = new PersistenceConversationalAttribute();
			var classDef = new ConversationalMetaInfoHolder(typeof (Sample), settings);
			var classDefDup = new ConversationalMetaInfoHolder(typeof (Sample), settings);
			var store = new ConversationalMetaInfoStore();
			store.AddMetadata(classDef);
			Assert.Throws<ArgumentException>(() => store.AddMetadata(classDefDup));
		}

		[Test]
		public void ShouldWork()
		{
			var settings = new PersistenceConversationalAttribute();
			var classDef = new ConversationalMetaInfoHolder(typeof (Sample), settings);
			var store = new ConversationalMetaInfoStore();
			store.AddMetadata(classDef);
			store.MetaData.Should().Not.Be.Empty();
			store.MetaData.Should().Contain(classDef);
			store.GetMetadataFor(typeof(Sample)).Should().Be.SameInstanceAs(classDef);
		}
	}
}