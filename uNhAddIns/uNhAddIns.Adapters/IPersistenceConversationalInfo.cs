using System;

namespace uNhAddIns.Adapters
{
	/// <summary>
	/// Conversational class meta-data.
	/// </summary>
	/// <remarks>
	/// Implemented by meta-info holder (in general an attribute).
	/// </remarks>
	public interface IPersistenceConversationalInfo
	{
		/// <summary>
		/// Fixed Conversation's Id for the target class.
		/// </summary>
		/// <remarks>
		/// Optional.
		/// <para>
		/// Only use it when multiple instances of the target class must work in the same conversation.
		/// </para>
		/// </remarks>
		string ConversationId { get; }

		/// <summary>
		/// Conversation's Id prefix.
		/// </summary>
		/// <remarks>
		/// Optional.
		/// <para>
		/// The result conversation's Id will be composed by IdPrefix + UniqueId
		/// </para>
		/// </remarks>
		string IdPrefix { get; }

		/// <summary>
		/// Define the way each method, of the target class, will be included in a persistent conversation.
		/// </summary>
		/// <remarks>
		/// Optional, default <see cref="uNhAddIns.Adapters.MethodsIncludeMode.Implicit"/>
		/// </remarks>
		MethodsIncludeMode MethodsIncludeMode { get; }

		/// <summary>
		/// Define the <see cref="EndMode"/> of each method where not explicity declared.
		/// </summary>
		/// <remarks>
		/// Optional, default <see cref="EndMode.Continue"/>
		/// </remarks>
		EndMode DefaultEndMode { get; }

		///<summary>
		/// Define the class where conversation's events handlers are implemented.
		///</summary>
		/// <remarks>
		/// The class must implements IConversationCreationInterceptor.
		/// </remarks>
		Type ConversationCreationInterceptor { get; }

		/// <summary>
		/// Use the IoC container to discover the implementor of IConversationCreationInterceptorConvention{T}
		/// where T is the class indicated by <seealso cref="IPersistenceConversationalInfo"/>.
		/// </summary>
		bool UseConversationCreationInterceptorConvention { get; }

		/// <summary>
		/// Allow persistent call outside of the service scope.	Usefull in combination with linq queries.
		/// </summary>
		bool AllowOutsidePersistentCall { get; set; }
	}
}