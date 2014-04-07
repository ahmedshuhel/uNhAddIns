using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests.Integration;
using uNhAddIns.SessionEasier;

namespace uNhAddIns.CastleAdapters.Tests
{
	[TestFixture]
	public class SessionWrapperFixture : SessionWrapperFixtureBase
	{
		#region Overrides of SessionWrapperFixtureBase

		protected override IServiceLocator NewServiceLocator()
		{
			var container = new WindsorContainer();
			var sl = new WindsorServiceLocator(container);
			container.Register(Component.For<ISessionWrapper>().ImplementedBy<SessionWrapper>());
			return sl;
		}

		#endregion
	}
}