using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using uNhAddIns.CastleAdapters.EnhancedBytecodeProvider;

namespace uNhAddIns.CastleAdapters.Tests.EnhancedBytecodeProvider
{
    [TestFixture]
    public class ProxyFactoryFactoryInjectionFixture
    {
        [Test]
        public void can_inject_through_fully_qualified_name()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);
            Type proxyFactory = typeof(ProxyFactoryFactory2);

            bytecode.SetProxyFactoryFactory(proxyFactory.AssemblyQualifiedName);

            bytecode.ProxyFactoryFactory.GetType()
                    .Should().Be.EqualTo(proxyFactory);

        }


        [Test]
        public void can_inject_and_resolve_from_container()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);
            Type proxyFactoryType = typeof(ProxyFactoryFactory2);
            var proxyFactory = new ProxyFactoryFactory2();

            container.Register(Component.For<ProxyFactoryFactory2>()
                                        .Instance(proxyFactory));

            bytecode.SetProxyFactoryFactory(proxyFactoryType.AssemblyQualifiedName);

            bytecode.ProxyFactoryFactory
                    .Should().Be.SameInstanceAs(proxyFactory);

        }

        [Test]
        public void throw_exception_when_passing_wrong_qualified_name()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);

            const string qualifiedName = "kkdlkdsa";

            new Action(() => bytecode.SetProxyFactoryFactory(qualifiedName))
                .Should().Throw<TypeLoadException>();
        }
    }

    public class ProxyFactoryFactory2 : NHibernate.Proxy.DefaultProxyFactory
    { }
}