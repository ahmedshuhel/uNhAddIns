using System;
using System.Reflection;
using uNhAddIns.Extensions;
using uNhAddIns.SessionEasier.Conversations;

namespace uNhAddIns.Adapters.Common
{
	public abstract class AbstractConversationInterceptor
	{
		protected static readonly Type BaseConventionType = typeof(IConversationCreationInterceptorConvention<>);

		protected string ConversationId;
		protected IConversationalMetaInfoHolder metadata;

		protected AbstractConversationInterceptor(IConversationalMetaInfoStore metadataStore,
			IConversationsContainerAccessor conversationsContainerAccessor,
			IConversationFactory conversationFactory)
		{
			ConversationsContainerAccessor = conversationsContainerAccessor;
			ConversationFactory = conversationFactory;
			MetadataStore = metadataStore;
		}

		protected IConversationalMetaInfoStore MetadataStore { get; set; }

		protected IConversationsContainerAccessor ConversationsContainerAccessor { get; private set; }

		protected IConversationFactory ConversationFactory { get; private set; }

		protected virtual IConversationalMetaInfoHolder Metadata
		{
			get
			{
				if (metadata == null)
				{
					metadata = MetadataStore.GetMetadataFor(GetConversationalImplementor());
				}
				return metadata;
			}
		}

		protected abstract Type GetConversationalImplementor();
		protected abstract IConversationCreationInterceptor GetConversationCreationInterceptor(Type configuredConcreteType);

		protected virtual void BeforeMethodExecution(MethodInfo methodInfo)
		{
			IPersistenceConversationInfo att = Metadata.GetConversationInfoFor(methodInfo);
			var cca = ConversationsContainerAccessor;
			if (att == null || cca == null)
			{
				return;
			}
			string convId = GetConvesationId(Metadata.Setting);
			IConversation c = cca.Container.Get(convId);
			if (c == null)
			{
				var cf = ConversationFactory;
				if (cf == null)
				{
					return;
				}
				c = cf.CreateConversation(convId);
				// we are using the event because a custom eventHandler can prevent the rethrow
				// but we must Unbind the conversation from the container
				// and we must dispose the conversation itself (high probability UoW inconsistence).
				c.OnException += ((conversation, args) => cca.Container.Unbind(c.Id).Dispose());
				ConfigureConversation(c);
				cca.Container.SetAsCurrent(c);
				c.Start();
			}
			else
			{
				cca.Container.SetAsCurrent(c);
				c.Resume();
			}
		}

		protected virtual void AfterMethodExecution(MethodInfo methodInfo)
		{
			IPersistenceConversationInfo att = Metadata.GetConversationInfoFor(methodInfo);
			var cca = ConversationsContainerAccessor;
			if (att == null || cca == null)
			{
				return;
			}
			IConversation c = cca.Container.Get(ConversationId);
			switch (att.ConversationEndMode)
			{
				case EndMode.End:
					c.End();
					c.Dispose();
					break;
				case EndMode.Abort:
					c.Abort();
					c.Dispose();
					break;
				case EndMode.CommitAndContinue:
					c.FlushAndPause();
					break;
				default:
					c.Pause();
					break;
			}
		}

		protected bool ShouldBeIntercepted(MethodInfo methodInfo)
		{
			return methodInfo != null && Metadata.Contains(methodInfo);
		}

		protected virtual string GetConvesationId(IPersistenceConversationalInfo config)
		{
			if (ConversationId == null)
			{
				if (!string.IsNullOrEmpty(config.ConversationId))
				{
					ConversationId = config.ConversationId;
				}
				else if (!string.IsNullOrEmpty(config.IdPrefix))
				{
					ConversationId = config.IdPrefix + Guid.NewGuid();
				}
				else
				{
					ConversationId = Guid.NewGuid().ToString();
				}
			}
			return ConversationId;
		}

		protected virtual void ConfigureConversation(IConversation conversation)
		{
			IConversationCreationInterceptor cci = null;
			Type creationInterceptorType = Metadata.Setting.ConversationCreationInterceptor;
			if (creationInterceptorType != null)
			{
				cci = creationInterceptorType.IsInterface ? GetConversationCreationInterceptor(creationInterceptorType) : creationInterceptorType.Instantiate<IConversationCreationInterceptor>();
			}
			else
			{
				if (Metadata.Setting.UseConversationCreationInterceptorConvention)
				{
					Type concreteImplementationType = BaseConventionType.MakeGenericType(Metadata.ConversationalClass);
					cci = GetConversationCreationInterceptor(concreteImplementationType);
				}
			}
			if (cci != null)
			{
				cci.Configure(conversation);
			}
			if(Metadata.Setting.AllowOutsidePersistentCall)
			{
				var conversationWithOpc = conversation as ISupportOutsidePersistentCall;
				if(conversationWithOpc == null)
				{
					throw new InvalidOperationException("The conversation doesn't support outside persistent call");
				}
				conversationWithOpc.UseSupportForOutsidePersistentCall = true;
			}
		}

		protected void DisposeConversationOnException()
		{
			var cca = ConversationsContainerAccessor;
			var conversation = cca.Container.Unbind(ConversationId);
			if (!ReferenceEquals(null, conversation))
			{
				conversation.Dispose();
			}
		}
	}
}