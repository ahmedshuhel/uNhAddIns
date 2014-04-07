using System;
using NHibernate;
using NUnit.Framework;
using uNhAddIns.Extensions;

namespace uNhAddIns.Test.Extensions
{
	[TestFixture]
	public class ReflectionExtensionsFixture
	{
		private interface IMyInterface { }
		private class MyClass : IMyInterface { }
		private class MyClassNoDef : IMyInterface
		{
			public MyClassNoDef(int p) {}
		}

		private class MyClassCtorException : IMyInterface
		{
			public MyClassCtorException()
			{
				throw new NotImplementedException();
			}
		}

		[Test]
		public void Instantiate()
		{
			// work fine
			Assert.That(ReflectionExtensions.Instantiate<IMyInterface>(typeof (MyClass)), Is.Not.Null);

			Assert.Throws<InstantiationException>(() => ReflectionExtensions.Instantiate<IMyInterface>(typeof (MyClassNoDef)));
			Assert.Throws<InstantiationException>(() => ReflectionExtensions.Instantiate<IMyInterface>(typeof(object)));
			Assert.Throws<InstantiationException>(() => ReflectionExtensions.Instantiate<IMyInterface>(typeof(MyClassCtorException)));
		}

		[Test]
		public void InstantiateWithArguments()
		{
			typeof (MyClassNoDef).Instantiate<MyClassNoDef>(5).Should().Not.Be.Null();
		}
	}
}