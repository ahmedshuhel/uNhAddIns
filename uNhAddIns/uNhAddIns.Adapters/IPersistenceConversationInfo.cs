namespace uNhAddIns.Adapters
{
	/// <summary>
	/// Conversation method meta-data.
	/// </summary>
	public interface IPersistenceConversationInfo
	{
		/// <summary>
		/// The action to take after finishing this part of the conversation.
		/// </summary>
		/// <remarks>
		/// Default <see cref="PersistenceConversationalAttribute.DefaultEndMode"/>
		/// </remarks>
		EndMode ConversationEndMode { get; set; }

		///<summary>
		/// <see langword="true"/> if you want to explicitly exclude a method from a persistence-conversation.
		///</summary>
		/// <remarks>
		/// Default <see langword="false"/>.
		/// </remarks>
		bool Exclude { get; }
	}
}