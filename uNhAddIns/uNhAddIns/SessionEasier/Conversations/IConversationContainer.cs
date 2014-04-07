namespace uNhAddIns.SessionEasier.Conversations
{
	/// <summary>
	/// The context adapter to hold conversations.
	/// </summary>
	public interface IConversationContainer
	{
		/// <summary>
		/// Get a <see cref="IConversation"/> instance for a given ID.
		/// </summary>
		/// <param name="conversationId">the id of the conversation.</param>
		/// <returns>The conversation.</returns>
		IConversation Get(string conversationId);

		/// <summary>
		/// Currente conversation.
		/// </summary>
		IConversation CurrentConversation { get; }

		/// <summary>
		/// Remove a conversation from the container.
		/// </summary>
		/// <param name="conversationId">The conversation id.</param>
		/// <returns>The conversation removed.</returns>
		IConversation Unbind(string conversationId);

		/// <summary>
		/// Hold a conversation in the container.
		/// </summary>
		/// <param name="conversation">The conversation.</param>
		void Bind(IConversation conversation);

		/// <summary>
		/// Set a conversation as current.
		/// </summary>
		/// <param name="conversation">The conversation to set as current.</param>
		/// <remarks>
		/// If the conversation is not contained it will be binded too.
		/// </remarks>
		void SetAsCurrent(IConversation conversation);

		/// <summary>
		/// Set as current the conversation with the given id.
		/// </summary>
		/// <param name="conversationId">The given id of the conversation.</param>
		void SetAsCurrent(string conversationId);
	}
}