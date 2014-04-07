using System;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NHibernate.Bytecode;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests.EnhancedBytecodeProvider;
using uNhAddIns.CastleAdapters.EnhancedBytecodeProvider;

namespace uNhAddIns.CastleAdapters.Tests.EnhancedBytecodeProvider
{
	[TestFixture]
	public class EntityInjectionFixture : AbstractEntityInjectionFixture
	{
		private WindsorContainer container;

		protected override void InitializeServiceLocator()
		{
			container = new WindsorContainer();
			var sl = new WindsorServiceLocator(container);
			container.Register(Component.For<IServiceLocator>().Instance(sl));
			ServiceLocator.SetLocatorProvider(() => sl);
			container.AddComponent<IInvoiceTotalCalculator, SumAndTaxTotalCalculator>();
			container.AddComponentLifeStyle(typeof(Invoice).FullName,
				typeof(IInvoice), typeof(Invoice), LifestyleType.Transient);
		}

		protected override IBytecodeProvider GetBytecodeProvider()
		{
			return new EnhancedBytecode(container);
		}
	}
}