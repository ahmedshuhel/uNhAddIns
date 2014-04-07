using System;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Proxy.DynamicProxy;
using NUnit.Framework;
using uNhAddIns.SessionEasier.Contexts;
using uNhAddIns.SessionEasier.Conversations;
using IInterceptor = NHibernate.Proxy.DynamicProxy.IInterceptor;

namespace uNhAddIns.Test.Conversations
{
    [TestFixture]
    public class NhConversationsContainerAccessorFixture
    {
        private static readonly ProxyFactory ProxyGenerator = new ProxyFactory();

        private class InvalidSessionFactory : IInterceptor
        {
            #region IInterceptor Members

            public object Intercept(InvocationInfo info)
            {
                return null;
            }

            #endregion
        }

        private class SessionFactoryMock : IInterceptor
        {
            public object SessionContext { get; set; }

            #region IInterceptor Members

            public object Intercept(InvocationInfo info)
            {
                return "get_CurrentSessionContext".Equals(info.TargetMethod.Name) ? SessionContext : null;
            }

            #endregion
        }

//        [Test]
//        public void Ctor()
//        {
//            Assert.Throws<ArgumentNullException>(() => new NhConversationsContainerAccessor(null));
//            Assert.Throws<ConversationException>(() => new NhConversationsContainerAccessor(new ISessionFactory[0]));
//
//
//            var invalidSFmock =
//                (ISessionFactory) ProxyGenerator.CreateProxy(typeof (ISessionFactory), new InvalidSessionFactory());
//
//
//            Assert.Throws<ConversationException>(() => new NhConversationsContainerAccessor(new[] {invalidSFmock}));
//
//            var notvalidSFmock =
//                (ISessionFactory)
//                ProxyGenerator.CreateProxy(typeof (ISessionFactory), new SessionFactoryMock(),
//                                           new[] {typeof (ISessionFactory)});
//
//            Assert.Throws<ConversationException>(() => new NhConversationsContainerAccessor(new[] {notvalidSFmock}));
//
//            var container = new ThreadLocalConversationalSessionContext(null);
//
//            object validSFmock =
//                ProxyGenerator.CreateProxy(typeof(ISessionFactoryImplementor),
//                                           new SessionFactoryMock {SessionContext = container},
//                                           new[]
//                                               {
//                                                   typeof (ISessionFactory)
//                                               });
//            var ca = new NhConversationsContainerAccessor(new[] {validSFmock});
//            Assert.That(ca.Container, Is.SameAs(container));
//
//            var sessionFactoryMock = new SessionFactoryMock();
//            object contexMock = ProxyGenerator.CreateProxy<ISessionFactoryImplementor>(sessionFactoryMock,
//                                                                                       new[] {typeof (ISessionFactory)});
//            sessionFactoryMock.SessionContext = new ThreadLocalSessionContext(contexMock);
//            try
//            {
//                new NhConversationsContainerAccessor(new[] {contexMock});
//            }
//            catch (ConversationException e)
//            {
//                Assert.That(e.Message,
//                            Text.Contains("Current session context does not implement IConversationContainer"));
//            }
//        }
    }
}