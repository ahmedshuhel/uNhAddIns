namespace uNhAddIns.SessionEasier.Conversations
{
	/// <summary>
	/// Factory for conversations.
	/// </summary>
	public interface IConversationFactory
	{
		IConversation CreateConversation();
		IConversation CreateConversation(string conversationId);
	}
}