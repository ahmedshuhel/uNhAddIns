using Castle.Windsor;
using NHibernate.Bytecode;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider;
using uNhAddIns.CastleAdapters.EnhancedBytecodeProvider;

namespace uNhAddIns.CastleAdapters.Tests.EnhancedBytecodeProvider
{
	[TestFixture]
	public class InjectableUserTypeFixture : AbstractInjectableUserTypeFixture
	{
		private WindsorContainer container;

		protected override void InitializeServiceLocator()
		{
			container = new WindsorContainer();
			container.AddComponent<IDelimiter, ParenDelimiter>();
			container.AddComponent<InjectableStringUserType>();
		}

		protected override IBytecodeProvider GetBytecodeProvider()
		{
			return new EnhancedBytecode(container);
		}
	}
}