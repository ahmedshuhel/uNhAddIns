namespace uNhAddIns.SessionEasier.Conversations
{
	/// <summary>
	/// A contract for a persistent conversation configurator.
	/// </summary>
	/// <remarks>
	/// In a implementor you can use the <see cref="Configure"/> method to subscribe your class
	/// to the conversation events (see also uNhAddIns.Adapters.PersistenceConversationAttribute).
	/// </remarks>
	/// <seealso cref="IConversation"/>
	public interface IConversationCreationInterceptor
	{
		/// <summary>
		/// Configure a new fresh conversation.
		/// </summary>
		/// <param name="conversation">The new conversation.</param>
		void Configure(IConversation conversation);
	}

	/// <summary>
	/// A contract for a persistent conversation configurator.
	/// </summary>
	/// <typeparam name="T">The type of the class involved to the persistent-conversation</typeparam>
	/// <remarks>
	/// Implementor of this class should be injected trough IoC and managed by IoC/AOP adapters.
	/// </remarks>
	/// <seealso cref="IConversationCreationInterceptor"/>
	public interface IConversationCreationInterceptorConvention<T> : IConversationCreationInterceptor where T: class
	{

	}
}