using System;
using System.Collections.Generic;
using System.Reflection;

namespace uNhAddIns.Adapters.Common
{
	/// <summary>
	/// Conversational metadata holder.
	/// </summary>
	public interface IConversationalMetaInfoHolder
	{
		/// <summary>
		/// The conversational class (owner of metadata)
		/// </summary>
		Type ConversationalClass { get; }

		/// <summary>
		/// The conversational class settings.
		/// </summary>
		IPersistenceConversationalInfo Setting { get; }

		/// <summary>
		/// Conversation methods.
		/// </summary>
		IEnumerable<MethodInfo> Methods { get; }

		/// <summary>
		/// Determines whether the <see cref="IConversationalMetaInfoHolder"/> object contains an element with the specified MethodInfo.
		/// </summary>
		/// <param name="methodInfo">The method to find.</param>
		/// <returns><see langword="true"/> if the method was added; <see langword="false"/> otherwise.</returns>
		bool Contains(MethodInfo methodInfo);

		/// <summary>
		/// Get metadata for a given MethodInfo.
		/// </summary>
		/// <param name="methodInfo">The method to find.</param>
		/// <returns>The metadata or <see langword="null"/> if the method is unknow.</returns>
		IPersistenceConversationInfo GetConversationInfoFor(MethodInfo methodInfo);
	}
}