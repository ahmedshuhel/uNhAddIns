using System;
using System.Collections.Generic;

namespace uNhAddIns.Adapters.Common
{
	/// <summary>
	/// Metadata store for conversational classes
	/// </summary>
	public interface IConversationalMetaInfoStore
	{
		void AddMetadata(IConversationalMetaInfoHolder classMetadata);
		IConversationalMetaInfoHolder GetMetadataFor(Type conversationalClass);
		IEnumerable<IConversationalMetaInfoHolder> MetaData { get; }
	}
}