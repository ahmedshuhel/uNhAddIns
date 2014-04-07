namespace uNhAddIns.Adapters
{
	///<summary>
	/// Define the way each method will be included in a persistent conversation.
	///</summary>
	public enum MethodsIncludeMode
	{
		///<summary>
		/// Methods involved must be explicitly declared.
		///</summary>
		Explicit,
		
		///<summary>
		/// Each method is involved in a persistence-conversation if not explicitly excluded.
		///</summary>
		Implicit
	}
}