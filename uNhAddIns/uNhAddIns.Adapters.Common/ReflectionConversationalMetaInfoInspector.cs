using System;
using System.Reflection;

namespace uNhAddIns.Adapters.Common
{
	public class ReflectionConversationalMetaInfoInspector : IConversationalMetaInfoInspector
	{
		#region IConversationalMetaInfoInspector Members

		public IPersistenceConversationalInfo GetInfo(Type type)
		{
			if (!type.IsDefined(typeof (PersistenceConversationalAttribute), true))
			{
				return null;
			}
			object[] atts = type.GetCustomAttributes(typeof (PersistenceConversationalAttribute), true);
			return atts[0] as PersistenceConversationalAttribute;
		}

		public IPersistenceConversationInfo GetMethodInfo(MethodInfo methodInfo)
		{
			object[] atts = methodInfo.GetCustomAttributes(typeof (PersistenceConversationAttribute), true);
			if (atts == null || atts.Length == 0)
			{
				return null;
			}

			return (PersistenceConversationAttribute) atts[0];
		}

		#endregion
	}
}