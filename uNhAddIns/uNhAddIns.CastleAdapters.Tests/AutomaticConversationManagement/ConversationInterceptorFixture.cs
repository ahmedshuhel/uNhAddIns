using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests;
using uNhAddIns.Adapters.CommonTests.ConversationManagement;
using uNhAddIns.CastleAdapters.AutomaticConversationManagement;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.CastleAdapters.Tests.AutomaticConversationManagement
{
    [TestFixture]
    public class ConversationInterceptorFixture : ConversationFixtureBase
    {
        private class TestWindsorContainerAccessor : IContainerAccessor
        {
            private readonly IWindsorContainer _container;

            public TestWindsorContainerAccessor(IWindsorContainer container)
            {
                _container = container;
            }

            #region Implementation of IContainerAccessor

            public IWindsorContainer Container
            {
                get { return _container; }
            }

            #endregion
        }

        protected override IServiceLocator NewServiceLocator()
        {
            var container = new WindsorContainer();
            container.AddFacility<PersistenceConversationFacility>();
            var wca = new TestWindsorContainerAccessor(container);
            container.Register(Component.For<IContainerAccessor>().Instance(wca));

            // Services for this test
            var sl = new WindsorServiceLocator(container);
            container.Register(Component.For<IServiceLocator>().Instance(sl));

            container.Register(
                Component.For<IConversationContainer>().ImplementedBy<ThreadLocalConversationContainerStub>());
            container.Register(
                Component.For<IConversationsContainerAccessor>().ImplementedBy<ConversationsContainerAccessorStub>());

            container.Register(Component.For<IDaoFactory>().ImplementedBy<DaoFactoryStub>());
            container.Register(Component.For<ISillyDao>().ImplementedBy<SillyDaoStub>());

            return sl;
        }

        protected override void RegisterAsTransient<TService, TImplementor>(IServiceLocator serviceLocator)
        {
            var windsor = serviceLocator.GetInstance<IContainerAccessor>();
            windsor.Container.Register(Component.For<TService>().ImplementedBy<TImplementor>().LifeStyle.Transient);
        }

        protected override void RegisterInstanceForService<T>(IServiceLocator serviceLocator, T instance)
        {
            var windsor = serviceLocator.GetInstance<IContainerAccessor>();
            windsor.Container.Register(Component.For(typeof (T)).Instance(instance));            
        }
    }
}