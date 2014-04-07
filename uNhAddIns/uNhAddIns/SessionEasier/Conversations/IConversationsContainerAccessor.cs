namespace uNhAddIns.SessionEasier.Conversations
{
	/// <summary>
	/// This interface should be implemented by classes that are available in a bigger context, exposing
	/// the container to different areas in the same application.
	/// </summary>
	public interface IConversationsContainerAccessor
	{
		IConversationContainer Container { get; }
	}
}