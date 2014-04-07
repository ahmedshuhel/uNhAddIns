namespace uNhAddIns.Adapters
{
	///<summary>
	/// Enum types to decide what to do with the conversation after executing the current action
	///</summary>
	public enum EndMode
	{
		/// <summary>
		/// Use <see cref="PersistenceConversationalAttribute.DefaultEndMode"/>.
		/// </summary>
		Unspecified,

		///<summary>
		/// Continue the conversation
		///</summary>
		Continue,

		///<summary>
		/// Flushes the results and continues the conversation
		///</summary>
		CommitAndContinue,

		///<summary>
		/// end and accept the changes
		///</summary>
		End,

		///<summary>
		/// end and abort the changes
		///</summary>
		Abort
	}
}