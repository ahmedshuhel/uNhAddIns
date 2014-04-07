using System;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NUnit.Framework;
using uNhAddIns.SessionEasier;
using uNhAddIns.TestUtils.NhIntegration;

namespace uNhAddIns.Adapters.CommonTests.Integration
{
	public abstract class SessionWrapperFixtureBase : FunctionalTestCase
	{
		/// <summary>
		/// Initialize a new ServiceLocator registering base services needed by this test.
		/// </summary>
		/// <remarks>
		/// Services needed, in this test, are:
		/// 
		/// - uNhAddIns.SessionEasier.ISessionWrapper
		///		Implementation: uNhAddIns.YourAdapter.SessionWrapper
		/// 
		/// </remarks>
		protected abstract IServiceLocator NewServiceLocator();

		[Test]
		public void ShouldThrowsExceptionWrappingNullSession()
		{
			IServiceLocator serviceLocator = NewServiceLocator();
			var w = serviceLocator.GetInstance<ISessionWrapper>();
			Assert.Throws<ArgumentNullException>(() => w.Wrap(null, null, null));
		}

		[Test]
		public void ShouldWrapTheSessionWithTheDefaultProxyInterfaceMarker()
		{
			IServiceLocator serviceLocator = NewServiceLocator();
			var w = serviceLocator.GetInstance<ISessionWrapper>();
			using(ISession session = SessionFactory.OpenSession())
			{
				var wrapped = w.Wrap(session, null, null);

				Assert.That(wrapped, Is.Not.SameAs(session));
				Assert.That((wrapped as ISessionProxy), Is.Not.Null);
			}
		}

		[Test]
		public void ShouldNotWrapAnAlredyWrappedSession()
		{
			IServiceLocator serviceLocator = NewServiceLocator();
			var w = serviceLocator.GetInstance<ISessionWrapper>();
			using (ISession session = SessionFactory.OpenSession())
			{
				var wrapped = w.Wrap(session, null, null);

				Assert.That(w.Wrap(wrapped, null, null), Is.SameAs(wrapped));
			}
		}

		[Test]
		public void ShouldRecognizeAWrappedSession()
		{
			IServiceLocator serviceLocator = NewServiceLocator();
			var w = serviceLocator.GetInstance<ISessionWrapper>();
			using (ISession session = SessionFactory.OpenSession())
			{
				var wrapped = w.Wrap(session, null, null);

				Assert.That(w.IsWrapped(wrapped));
				Assert.That(!w.IsWrapped(session));
			}
		}
	}
}