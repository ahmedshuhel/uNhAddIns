using System;
using Castle.Core;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using uNhAddIns.Adapters.Common;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.CastleAdapters.AutomaticConversationManagement
{
	[Transient]
	public class ConversationInterceptor : AbstractConversationInterceptor, IInterceptor, IOnBehalfAware
	{
		private readonly IKernel _kernel;
		private Type _targetImplementation;

		public ConversationInterceptor(IKernel kernel,
			IConversationalMetaInfoStore metadataStore,
			IConversationsContainerAccessor conversationsContainerAccessor,
			IConversationFactory conversationFactory)
			: base(metadataStore, conversationsContainerAccessor, conversationFactory)
		{
			_kernel = kernel;
		}

		public void Intercept(IInvocation invocation)
		{
			var methodInfo = invocation.MethodInvocationTarget;
			if (!ShouldBeIntercepted(methodInfo))
			{
				invocation.Proceed();
			}
			else
			{
				BeforeMethodExecution(methodInfo);
				try
				{
					invocation.Proceed();
					AfterMethodExecution(methodInfo);
				}
				catch (Exception)
				{
					DisposeConversationOnException();
					throw;
				}
			}
		}

		public void SetInterceptedComponentModel(ComponentModel target)
		{
			_targetImplementation = target.Implementation;
		}

		protected override Type GetConversationalImplementor()
		{
			return _targetImplementation;
		}

		protected override IConversationCreationInterceptor GetConversationCreationInterceptor(Type configuredConcreteType)
		{
			return _kernel.HasComponent(configuredConcreteType)
                    ? (IConversationCreationInterceptor)_kernel.Resolve(configuredConcreteType)
			       	: null;
		}
	}
}