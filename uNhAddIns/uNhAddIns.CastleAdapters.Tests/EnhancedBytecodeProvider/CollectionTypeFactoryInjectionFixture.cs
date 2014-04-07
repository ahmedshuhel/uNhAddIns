using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate.Type;
using NUnit.Framework;
using uNhAddIns.CastleAdapters.EnhancedBytecodeProvider;

namespace uNhAddIns.CastleAdapters.Tests.EnhancedBytecodeProvider
{
    [TestFixture]
    public class CollectionTypeFactoryInjectionFixture
    {
        [Test]
        public void can_inject_through_fully_qualified_name()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);
            Type collFactoryType = typeof(TestCollTypeFactory);

            bytecode.SetCollectionTypeFactoryClass(collFactoryType.AssemblyQualifiedName);

            bytecode.CollectionTypeFactory.GetType()
                    .Should().Be.EqualTo(collFactoryType);

        }

        [Test]
        public void can_inject_through_type()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);
            Type collFactoryType = typeof(TestCollTypeFactory);

            bytecode.SetCollectionTypeFactoryClass(collFactoryType);

            bytecode.CollectionTypeFactory.GetType()
                    .Should().Be.EqualTo(collFactoryType);

        }

        [Test]
        public void can_inject_through_type_and_resolve_from_container()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);
            var collFactory = new TestCollTypeFactory();

            container.Register(Component.For<TestCollTypeFactory>().Instance(collFactory));

            Type collFactoryType = typeof(TestCollTypeFactory);

            bytecode.SetCollectionTypeFactoryClass(collFactoryType);

            bytecode.CollectionTypeFactory
                    .Should().Be.SameInstanceAs(collFactory);

        }

        [Test]
        public void throw_exception_when_passing_wrong_qualified_name()
        {
            var container = new WindsorContainer();
            var bytecode = new EnhancedBytecode(container);

            const string qualifiedName = "kkdlkdsa";

            new Action(() => bytecode.SetCollectionTypeFactoryClass(qualifiedName))
                .Should().Throw<TypeLoadException>();
        }
    }

    public class TestCollTypeFactory : DefaultCollectionTypeFactory
    {}
}