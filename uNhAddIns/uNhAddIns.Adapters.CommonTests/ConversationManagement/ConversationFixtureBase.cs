using System;
using System.Linq;
using log4net.Config;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using uNhAddIns.Adapters.CommonTests.Integration;
using uNhAddIns.SessionEasier.Conversations;
using uNhAddIns.TestUtils.Logging;

namespace uNhAddIns.Adapters.CommonTests.ConversationManagement
{
	public abstract class ConversationFixtureBase
	{
		protected ConversationFixtureBase()
		{
			XmlConfigurator.Configure();
		}

		/// <summary>
		/// Initialize a new ServiceLocator registering base services needed by this test.
		/// </summary>
		/// <remarks>
		/// Services needed, in this test, are:
		/// 
		/// - Microsoft.Practices.ServiceLocation.IServiceLocator
		///		The service locator itself is used by the implementation of DaoFactory
		/// 
		/// - uNhAddIns.SessionEasier.Conversations.IConversationContainer
		///		Implementation: uNhAddIns.Adapters.CommonTests.ConversationManagement.ThreadLocalConversationContainerStub
		/// 
		/// - uNhAddIns.SessionEasier.Conversations.IConversationsContainerAccessor
		///		Implementation: uNhAddIns.Adapters.CommonTests.ConversationManagement.ConversationsContainerAccessorStub
		/// 
		/// - uNhAddIns.Adapters.CommonTests.IDaoFactory
		///		Implementation: uNhAddIns.Adapters.CommonTests.ConversationManagement.DaoFactoryStub
		/// 
		/// - uNhAddIns.Adapters.CommonTests.ISillyDao
		///		Implementation : uNhAddIns.Adapters.CommonTests.ConversationManagement.SillyDaoStub
		/// 
		/// </remarks>
		protected abstract IServiceLocator NewServiceLocator();

		/// <summary>
		/// Register a new service and its implementor as Transient (new instance per service get)
		/// </summary>
		/// <typeparam name="TService">The service type.</typeparam>
		/// <typeparam name="TImplementor">The type of the implementor.</typeparam>
		/// <param name="serviceLocator">The ServiceLocator instance</param>
		protected abstract void RegisterAsTransient<TService, TImplementor>(IServiceLocator serviceLocator)
			where TService : class where TImplementor : TService;

		/// <summary>
		/// Enlist an instance of a service.
		/// </summary>
		/// <typeparam name="T">The type of the service.</typeparam>
		/// <param name="serviceLocator">The ServiceLocator instance</param>
		/// <param name="instance">The instance of the given service type.</param>
		protected abstract void RegisterInstanceForService<T>(IServiceLocator serviceLocator, T instance);

		[Test]
		public void ShouldUnbindOnFlushException()
		{
			IServiceLocator serviceLocator = NewServiceLocator();
			RegisterAsTransient<ISillyCrudModel, SillyCrudModel>(serviceLocator);
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new ExceptionOnFlushConversationStub(id);
			                                              		result.OnException += ((sender, args) => args.ReThrow = false);
			                                              		return result;
			                                              	});
			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);

			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			Silly e = scm.GetIfAvailable(Guid.NewGuid());
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Don't start and bind the conversation inmediately");
			scm.ImmediateDelete(e);
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(0),
			            "Don't unbind the conversation with exception catch by custom event handler");

			conversationContainer.Reset();
		}

		[Test]
		public void ShouldUnbindOnEndException()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModel, SillyCrudModel>(serviceLocator);
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new ExceptionOnFlushConversationStub(id);
			                                              		result.OnException += ((sender, args) => args.ReThrow = false);
			                                              		return result;
			                                              	});
			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);

			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			scm.GetIfAvailable(Guid.NewGuid());
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Don't start and bind the conversation inmediately");
			scm.AcceptAll();
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(0),
			            "Don't unbind the conversation with exception catch by custom event handler");

			conversationContainer.Reset();
		}

		[Test]
		public void ShouldWorkWithConcreteCtorInterceptor()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModel, InheritedSillyCrudModelWithConcreteConversationCreationInterceptor>(
				serviceLocator);
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new NoOpConversationStub(id);
			                                              		return result;
			                                              	});
			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);

			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			Assert.That(
				Spying.Logger<ConversationCreationInterceptor>().Execute(() => scm.GetIfAvailable(Guid.NewGuid())).MessageSequence,
				Is.EqualTo(new[] {ConversationCreationInterceptor.StartingMessage, ConversationCreationInterceptor.StartedMessage}));
			// cleanup
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			conversationContainer.Reset();
		}

		[Test]
		public virtual void ShouldWorkWithServiceCtorInterceptor()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModel, InheritedSillyCrudModelWithServiceConversationCreationInterceptor>(
				serviceLocator);
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new NoOpConversationStub(id);
			                                              		return result;
			                                              	});
			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);

			// Registr the IConversationCreationInterceptor implementation
			RegisterAsTransient<IMyServiceConversationCreationInterceptor, ConversationCreationInterceptor>(serviceLocator);

			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			Assert.That(
				Spying.Logger<ConversationCreationInterceptor>().Execute(() => scm.GetIfAvailable(Guid.NewGuid())).MessageSequence,
				Is.EqualTo(new[] {ConversationCreationInterceptor.StartingMessage, ConversationCreationInterceptor.StartedMessage}));

			// cleanup
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			conversationContainer.Reset();
		}

		[Test]
		public virtual void ShouldWorkWithConventionCtorInterceptor()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModel, InheritedSillyCrudModelWithConvetionConversationCreationInterceptor>(
				serviceLocator);
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new NoOpConversationStub(id);
			                                              		return result;
			                                              	});
			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);

			// Registr the IConversationCreationInterceptor implementation
			RegisterAsTransient
				<IConversationCreationInterceptorConvention<InheritedSillyCrudModelWithConvetionConversationCreationInterceptor>,
					ConvetionConversationCreationInterceptor>(serviceLocator);

			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			Assert.That(
				Spying.Logger<ConvetionConversationCreationInterceptor>().Execute(() => scm.GetIfAvailable(Guid.NewGuid())).
					MessageSequence,
				Is.EqualTo(new[]
				           	{
				           		ConvetionConversationCreationInterceptor.StartingMessage,
				           		ConvetionConversationCreationInterceptor.StartedMessage
				           	}));

			// cleanup
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			conversationContainer.Reset();
		}

		[Test]
		public void ShouldSupportsDefaultEndMode()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModel, SillyCrudModelDefaultEnd>(serviceLocator);

			bool endedCalled = false;
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new NoOpConversationStub(id);
			                                              		result.Ended += ((s, a) => endedCalled = true);
			                                              		return result;
			                                              	});

			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);
			var scm = serviceLocator.GetInstance<ISillyCrudModel>();
			scm.GetIfAvailable(Guid.NewGuid());
			Assert.That(endedCalled);
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(0),
			            "Don't unbind the conversation after end. The Adapter are changing the conversation AutoUnbindAfterEndConversation");

			// cleanup
			conversationContainer.Reset();
		}

		[Test]
		public void ShouldSupportsImplicitMethodsInclusion()
		{
			IServiceLocator serviceLocator = NewServiceLocator();

			RegisterAsTransient<ISillyCrudModelExtended, SillyCrudModelWithImplicit>(serviceLocator);

			bool resumedCalled = false;
			bool startedCalled = false;
			var convFactory = new ConversationFactoryStub(delegate(string id)
			                                              	{
			                                              		IConversation result = new NoOpConversationStub(id);
			                                              		result.Resumed += ((s, a) => resumedCalled = true);
			                                              		result.Started += ((s, a) => startedCalled = true);
			                                              		return result;
			                                              	});

			RegisterInstanceForService<IConversationFactory>(serviceLocator, convFactory);
			var conversationContainer =
				(ThreadLocalConversationContainerStub) serviceLocator.GetInstance<IConversationContainer>();

			var scm = serviceLocator.GetInstance<ISillyCrudModelExtended>();

			scm.GetEntirelyList();
			Assert.That(startedCalled, "An implicit method inclusion don't start the conversation.");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Should have one active conversation because the default mode is continue.");
			resumedCalled = false;
			startedCalled = false;

			scm.GetIfAvailable(Guid.NewGuid());
			Assert.That(resumedCalled, "An implicit method inclusion don't resume the conversation.");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Should have one active conversation because the default mode is continue.");
			resumedCalled = false;

			scm.DoSomethingNoPersistent();
			Assert.That(!resumedCalled, "An explicit method exclusion resume the conversation; shouldn't");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Should have one active conversation because the default mode is continue.");

			string value = scm.PropertyOutConversation;
			Assert.That(!resumedCalled, "An explicit method exclusion resume the conversation; shouldn't");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Should have one active conversation because the default mode is continue.");

			value = scm.PropertyInConversation;
			Assert.That(resumedCalled, "An implicit method inclusion don't resume the conversation.");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(1),
			            "Should have one active conversation because the default mode is continue.");
			resumedCalled = false;

			scm.AcceptAll();
			Assert.That(resumedCalled, "An explicit method inclusion should resume the conversation");
			Assert.That(conversationContainer.BindedConversationCount, Is.EqualTo(0),
			            "Should have NO active conversation because the method AcceptAll end the conversation.");
		}

	}
}