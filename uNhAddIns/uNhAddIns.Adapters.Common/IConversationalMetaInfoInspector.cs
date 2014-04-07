using System;
using System.Reflection;

namespace uNhAddIns.Adapters.Common
{
	/// <summary>
	/// Metadata conversational metadata inspector
	/// </summary>
	public interface IConversationalMetaInfoInspector
	{
		/// <summary>
		/// Inspect a given class in order to know its persistence conversation settings.
		/// </summary>
		/// <param name="type">The class to inspect.</param>
		/// <returns>
		/// The <see cref="IPersistenceConversationalInfo"/> 
		/// or <see langword="null"/> where the class is not involved in a persistence conversations. 
		/// </returns>
		IPersistenceConversationalInfo GetInfo(Type type);

		/// <summary>
		/// Inspect a given method in order to know its persistence conversation settings.
		/// </summary>
		/// <param name="methodInfo">The method to inspect</param>
		/// <returns>
		/// The <see cref="IPersistenceConversationInfo"/> 
		/// or <see langword="null"/> where the method is not marked as "persistence conversational". 
		/// </returns>
		IPersistenceConversationInfo GetMethodInfo(MethodInfo methodInfo);
	}
}