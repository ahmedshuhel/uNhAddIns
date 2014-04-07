using System;

namespace uNhAddIns.SessionEasier.Conversations
{
	public class DefaultConversationFactory : IConversationFactory
	{
		private readonly ISessionFactoryProvider _factoriesProvider;
		private readonly ISessionWrapper _wrapper;

		public DefaultConversationFactory(ISessionFactoryProvider factoriesProvider, ISessionWrapper wrapper)
		{
			if (factoriesProvider == null)
			{
				throw new ArgumentNullException("factoriesProvider");
			}
			if (wrapper == null)
			{
				throw new ArgumentNullException("wrapper");
			}
			_factoriesProvider = factoriesProvider;
			_wrapper = wrapper;
		}


		public IConversation CreateConversation()
		{
			return new NhConversation(_factoriesProvider, _wrapper);
		}

		public IConversation CreateConversation(string conversationId)
		{
			return new NhConversation(_factoriesProvider, _wrapper, conversationId);
		}

	}
}