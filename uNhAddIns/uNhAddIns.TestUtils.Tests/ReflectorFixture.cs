using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;

namespace uNhAddIns.TestUtils.Tests
{
	[TestFixture]
	public class ReflectorFixture
	{
		public class MyClass
		{
			public string aField;
			public string AProperty { get; set; }
			public void AVoidMehotdWithoutParameters() {}
			public void AVoidMehotdWithoutParameters(int aParam) {}
			public void ANoVoidMehotdWithoutParameters() {}
			public void ANoVoidMehotdWithoutParameters(int aParam) {}
			public bool ABoleanProperty { get; set; }
		}

		[Test]
		public void ShouldWorkWithPublicField()
		{
			MemberInfo mi = (typeof (MyClass).GetMember("aField")).First();
			Assert.That(Reflector.MemberInfo<MyClass>(x => x.aField), Is.EqualTo(mi));
		}

		[Test]
		public void ShouldWorkWithPublicProperty()
		{
			MemberInfo mi = (typeof (MyClass).GetMember("AProperty")).First();
			Assert.That(Reflector.MemberInfo<MyClass>(x => x.AProperty), Is.EqualTo(mi));

			mi = (typeof(MyClass).GetMember("ABoleanProperty")).First();
			Assert.That(Reflector.MemberInfo<MyClass>(x => x.ABoleanProperty), Is.EqualTo(mi));
		}

		[Test]
		public void ShouldWorkPublicPropertyGetter()
		{
			MethodInfo mi =
				((typeof (MyClass).GetMember("AProperty")).OfType<PropertyInfo>().First()).GetAccessors()
					.Where(x => x.Name.StartsWith("get_")).First();
			Assert.That(Reflector.PropertyGetter<MyClass>(x => x.AProperty), Is.EqualTo(mi));

			mi = ((typeof(MyClass).GetMember("ABoleanProperty")).OfType<PropertyInfo>().First()).GetAccessors()
					.Where(x => x.Name.StartsWith("get_")).First();
			Assert.That(Reflector.PropertyGetter<MyClass>(x => x.ABoleanProperty), Is.EqualTo(mi));
		}

		[Test]
		public void ShouldWorkPublicPropertySetter()
		{
			MethodInfo mi =
				((typeof (MyClass).GetMember("AProperty")).OfType<PropertyInfo>().First()).GetAccessors()
					.Where(x => x.Name.StartsWith("set_")).First();
			Assert.That(Reflector.PropertySetter<MyClass>(x => x.AProperty), Is.EqualTo(mi));

			mi = ((typeof(MyClass).GetMember("ABoleanProperty")).OfType<PropertyInfo>().First()).GetAccessors()
		.Where(x => x.Name.StartsWith("set_")).First();
			Assert.That(Reflector.PropertySetter<MyClass>(x => x.ABoleanProperty), Is.EqualTo(mi));
		}

		[Test]
		public void ShouldWorkWithVoidPublicMethod()
		{
			MethodInfo mb = (typeof (MyClass).GetMethod("AVoidMehotdWithoutParameters", new[] {typeof (int)}));
			Assert.That(Reflector.MethodInfo<MyClass>(x => x.AVoidMehotdWithoutParameters(5)), Is.EqualTo(mb));

			mb = (typeof (MyClass).GetMethod("AVoidMehotdWithoutParameters", new Type[0]));
			Assert.That(Reflector.MethodInfo<MyClass>(x => x.AVoidMehotdWithoutParameters()), Is.EqualTo(mb));
		}

		[Test]
		public void ShouldWorkWithNoVoidPublicMethod()
		{
			MethodInfo mb = (typeof (MyClass).GetMethod("ANoVoidMehotdWithoutParameters", new[] {typeof (int)}));
			Assert.That(Reflector.MethodInfo<MyClass>(x => x.ANoVoidMehotdWithoutParameters(5)), Is.EqualTo(mb));

			mb = (typeof (MyClass).GetMethod("ANoVoidMehotdWithoutParameters", new Type[0]));
			Assert.That(Reflector.MethodInfo<MyClass>(x => x.ANoVoidMehotdWithoutParameters()), Is.EqualTo(mb));
		}

		[Test]
		public void ShouldWorkWithListOfMethods()
		{
			var mb = new[]
			         	{
			         		(typeof(MyClass).GetMethod("ANoVoidMehotdWithoutParameters", new[] { typeof(int) })),
									(typeof (MyClass).GetMethod("ANoVoidMehotdWithoutParameters", new Type[0]))
			         	};
			var methods = new Expression<Action<MyClass>>[]
			              	{
			              		c => c.ANoVoidMehotdWithoutParameters(5), 
												c => c.ANoVoidMehotdWithoutParameters()
			              	};

			var actual = Reflector.MethodsInfos(methods);
			for (int i = 0; i < 2; i++)
			{
				Assert.That(actual[i], Is.EqualTo(mb[i]));
			}
		}
	}
}