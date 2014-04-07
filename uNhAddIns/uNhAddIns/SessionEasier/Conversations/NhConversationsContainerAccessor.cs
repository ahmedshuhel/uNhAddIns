using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Engine;

namespace uNhAddIns.SessionEasier.Conversations
{
	public class NhConversationsContainerAccessor : IConversationsContainerAccessor
	{
		private readonly IConversationContainer container;

		public NhConversationsContainerAccessor(IEnumerable<ISessionFactory> sessionFactoryProvider)
		{
			if (sessionFactoryProvider == null)
			{
				throw new ArgumentNullException("sessionFactoryProvider");
			}
			IEnumerator<ISessionFactory> sfe = sessionFactoryProvider.GetEnumerator();
			if (!sfe.MoveNext())
			{
				throw new ConversationException("SessionFactoryProvider was not initialized.");
			}

			var factoryImpl = sfe.Current as ISessionFactoryImplementor;
			if (factoryImpl == null)
			{
				throw new ConversationException("Session factory does not implement ISessionFactoryImplementor.");
			}

			if (factoryImpl.CurrentSessionContext == null)
			{
				throw new ConversationException(
					"NhConversationsContainerAccessor extract the container from the CurrentSessionContext of the SessionFactory. No current session context configured.");
			}

			container = factoryImpl.CurrentSessionContext as IConversationContainer;
			if (container == null)
			{
				throw new ConversationException("Current session context does not implement IConversationContainer.\n Check your session-factory-configuration.");
			}
		}

		public NhConversationsContainerAccessor(ISessionFactoryProvider sessionFactoryProvider)
			: this((IEnumerable<ISessionFactory>) sessionFactoryProvider) {}

		#region Implementation of IConversationsContainerAccessor

		public IConversationContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}