using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using uNhAddIns.TestUtils;
using uNhAddIns.Adapters.Common;

namespace uNhAddIns.Adapters.CommonTests.ConversationManagement
{
	[TestFixture]
	public class ConversationalMetaInfoHolderFixture
	{
		public class Sample
		{
			public void PersistentMethod() { }
			public void NoPersistentMethod() { }
		}

		[Test]
		public void CTorProtection()
		{
			Assert.Throws<ArgumentNullException>(() => new ConversationalMetaInfoHolder(null, null))
				.ParamName.Should().Be.EqualTo("conversationalClass");
			Assert.Throws<ArgumentNullException>(() => new ConversationalMetaInfoHolder(typeof(object), null))
				.ParamName.Should().Be.EqualTo("setting");

			var settings = new PersistenceConversationalAttribute();
			Assert.Throws<ArgumentNullException>(() => new ConversationalMetaInfoHolder(null, settings))
				.ParamName.Should().Be.EqualTo("conversationalClass");
		}

		[Test]
		public void PropertiesDefault()
		{
			var settings = new PersistenceConversationalAttribute();
			var classDef = new ConversationalMetaInfoHolder(typeof(Sample), settings);
			classDef.ConversationalClass.Should().Be.EqualTo(typeof (Sample));
			classDef.Setting.Should().Be.SameInstanceAs(settings);
			classDef.Methods.Should().Be.Empty();

			Assert.That(!classDef.Contains(Reflector.MethodInfo<object>(o => o.GetHashCode())));
		}

		[Test]
		public void AddMethodInfoShouldWork()
		{
			ConversationalMetaInfoHolder classDef = CreateNewSampleDef();
			var methodSetting = new PersistenceConversationAttribute();
			MethodInfo methodInfo = Reflector.MethodInfo<Sample>(o => o.PersistentMethod());
			classDef.AddMethodInfo(methodInfo, methodSetting);
			classDef.Methods.Count().Should().Be.EqualTo(1);
			classDef.Methods.Should().Contain(methodInfo);
			classDef.Contains(methodInfo).Should().Be.True();
			classDef.GetConversationInfoFor(methodInfo).Should().Be.SameInstanceAs(methodSetting);
		}

		private static ConversationalMetaInfoHolder CreateNewSampleDef()
		{
			var settings = new PersistenceConversationalAttribute();
			return new ConversationalMetaInfoHolder(typeof (Sample), settings);
		}

		[Test]
		public void AddMethodInfoShouldBeProtected()
		{
			ConversationalMetaInfoHolder classDef = CreateNewSampleDef();
			MethodInfo methodInfo = Reflector.MethodInfo<Sample>(o => o.NoPersistentMethod());
			var methodSetting = new PersistenceConversationAttribute();
			Assert.Throws<ArgumentNullException>(() => classDef.AddMethodInfo(methodInfo, null))
				.ParamName.Should().Be.EqualTo("persistenceConversationInfo");
			Assert.Throws<ArgumentNullException>(() => classDef.AddMethodInfo(null, methodSetting))
				.ParamName.Should().Be.EqualTo("methodInfo");
		}

		[Test]
		public void GetConversationInfoFor()
		{
			ConversationalMetaInfoHolder classDef = CreateNewSampleDef();
			MethodInfo methodInfo = Reflector.MethodInfo<Sample>(o => o.NoPersistentMethod());
			classDef.GetConversationInfoFor(methodInfo).Should("return null when method is unknow").Be.Null();
		}
	}
}