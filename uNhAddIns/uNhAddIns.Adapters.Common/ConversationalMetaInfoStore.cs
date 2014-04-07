using System;
using System.Collections.Generic;

namespace uNhAddIns.Adapters.Common
{
	public class ConversationalMetaInfoStore : IConversationalMetaInfoStore
	{
		private readonly IDictionary<Type, IConversationalMetaInfoHolder> _typeInfo =
			new Dictionary<Type, IConversationalMetaInfoHolder>(100);

		private readonly object _locker = new object();


		public IConversationalMetaInfoHolder GetMetadataFor(Type conversationalClass)
		{
			IConversationalMetaInfoHolder result;
			_typeInfo.TryGetValue(conversationalClass, out result);
			return result;
		}

		public IEnumerable<IConversationalMetaInfoHolder> MetaData
		{
			get { return _typeInfo.Values; }
		}

		public void AddMetadata(IConversationalMetaInfoHolder classMetadata)
		{
			if (classMetadata == null)
			{
				throw new ArgumentNullException("classMetadata");
			}
			lock (_locker)
			{
				_typeInfo.Add(classMetadata.ConversationalClass, classMetadata);				
			}
		}

	}
}