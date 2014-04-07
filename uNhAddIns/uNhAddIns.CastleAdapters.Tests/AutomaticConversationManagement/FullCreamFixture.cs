using Castle.Facilities.FactorySupport;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NHibernate;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests;
using uNhAddIns.Adapters.CommonTests.Integration;
using uNhAddIns.CastleAdapters.AutomaticConversationManagement;
using uNhAddIns.SessionEasier;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.CastleAdapters.Tests.AutomaticConversationManagement
{
    [TestFixture]
    public class FullCreamFixture : FullCreamFixtureBase
    {
        protected override void InitializeServiceLocator()
        {
            //
            //        <components>
            //			<component id="uNhAddIns.sessionFactory" type="NHibernate.ISessionFactory, NHibernate" factoryId="sessionFactoryProvider" factoryCreate="GetFactory">
            //				<parameters>
            //					<factoryId>uNhAddIns</factoryId>
            //				</parameters>
            //			</component>
            //
            //			<component id="uNhAddIns.dao.factory" service="uNhAddIns.Adapters.CommonTests.IDaoFactory, uNhAddIns.Adapters.CommonTests" type="uNhAddIns.Adapters.CommonTests.Integration.DaoFactory, uNhAddIns.Adapters.CommonTests" />
            //			<component id="uNhAddIns.dao.Silly" service="uNhAddIns.Adapters.CommonTests.ISillyDao, uNhAddIns.Adapters.CommonTests" type="uNhAddIns.Adapters.CommonTests.Integration.SillyDao, uNhAddIns.Adapters.CommonTests" />
            //			<component id="uNhAddIns.model.SillyCrud" service="uNhAddIns.Adapters.CommonTests.ISillyCrudModel, uNhAddIns.Adapters.CommonTests" type="uNhAddIns.Adapters.CommonTests.Integration.SillyCrudModel, uNhAddIns.Adapters.CommonTests" lifestyle="transient" />
            //			<component id="uNhAddIns.model.ISillyReport" service="uNhAddIns.Adapters.CommonTests.ISillyReportModel, uNhAddIns.Adapters.CommonTests" type="uNhAddIns.Adapters.CommonTests.Integration.SillyReportModel, uNhAddIns.Adapters.CommonTests" lifestyle="transient" />
            //		</components>


//
//	<components>
//		<component id="sessionFactoryProvider"
//			service='uNhAddIns.SessionEasier.ISessionFactoryProvider, uNhAddIns'
//			type='uNhAddIns.SessionEasier.SessionFactoryProvider, uNhAddIns'/>
//
//		<component id="sessionWrapper"
//			service='uNhAddIns.SessionEasier.ISessionWrapper, uNhAddIns'
//			type='uNhAddIns.CastleAdapters.SessionWrapper, uNhAddIns.Adapters.Castle'/>
//
//		<component id="conversationFactory"
//			service='uNhAddIns.SessionEasier.Conversations.IConversationFactory, uNhAddIns'
//			type='uNhAddIns.SessionEasier.Conversations.DefaultConversationFactory, uNhAddIns'/>
//
//		<component id="conversationContainerAccessor"
//			service='uNhAddIns.SessionEasier.Conversations.IConversationsContainerAccessor, uNhAddIns'
//			type='uNhAddIns.SessionEasier.Conversations.NhConversationsContainerAccessor, uNhAddIns'/>
//	</components>



            var container = new WindsorContainer();
            var sl = new WindsorServiceLocator(container);
            ServiceLocator.SetLocatorProvider(() => sl);

         
            container.Register(Component.For<IServiceLocator>().Instance(sl));



           container.AddFacility<FactorySupportFacility>();
            container.AddFacility<PersistenceConversationFacility>();
            container.Register(Component.For<ISessionFactoryProvider>().ImplementedBy<SessionFactoryProvider>());

            container.Register(Component.For<ISessionWrapper>().ImplementedBy<SessionWrapper>());
            container.Register(Component.For<IConversationFactory>().ImplementedBy<DefaultConversationFactory>());
            container.Register(Component.For<IConversationsContainerAccessor>().ImplementedBy<NhConversationsContainerAccessor>());
   
            container.Register(
                Component.For<ISessionFactory>().UsingFactoryMethod(
                    () =>
                    container.Resolve<ISessionFactoryProvider>(
                        new
                        {
                            IConfigurationProvider = new DefaultSessionFactoryConfigurationProvider()
                        }).GetFactory(null)));









            container.Register(
                Component.For<ISillyReportModel>()
                         .ImplementedBy<SillyReportModel>()
                         .LifeStyle.Transient)
                     .Register(
                         Component.For<ISillyCrudModel>()
                                  .ImplementedBy<SillyCrudModel>()
                                  .LifeStyle.Transient)
                     .Register(
                         Component.For<ISillyDao>()
                                  .ImplementedBy<SillyDao>())
                     .Register(
                         Component.For<IDaoFactory>()
                                  .ImplementedBy<DaoFactory>());


        }
    }
}