using System;
using System.Reflection;

namespace uNhAddIns.Adapters.Common
{
	public class ReflectionConversationalMetaInfoStore: ConversationalMetaInfoStore
	{
		private readonly IConversationalMetaInfoInspector mii = new ReflectionConversationalMetaInfoInspector();
		public bool Add(Type conversationalClass)
		{
			if (conversationalClass == null)
			{
				throw new ArgumentNullException("conversationalClass");
			}
			var typeDef = mii.GetInfo(conversationalClass);
			if(typeDef == null)
			{
				return false;
			}
			var metaInfo = new ConversationalMetaInfoHolder(conversationalClass, typeDef);
			BuildMetaInfoFromType(metaInfo, metaInfo.ConversationalClass);
			AddMetadata(metaInfo);
			return true;
		}

		protected IConversationalMetaInfoInspector MetaInfoInspector
		{
			get { return mii; }
		}

		protected virtual void BuildMetaInfoFromType(ConversationalMetaInfoHolder metaInfo, Type implementation)
		{
			if (implementation == typeof(object) || implementation == typeof(MarshalByRefObject)) return;

			MethodInfo[] methods = implementation.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);

			foreach (MethodInfo method in methods)
			{
				var mi = mii.GetMethodInfo(method);
				AddMethodIfNecessary(metaInfo, method, mi);
			}

			BuildMetaInfoFromType(metaInfo, implementation.BaseType);
		}

		protected void AddMethodIfNecessary(ConversationalMetaInfoHolder holder, MethodInfo method, IPersistenceConversationInfo info)
		{
			IPersistenceConversationInfo toAdd = null;
			if (info != null)
			{
				if (!info.Exclude)
				{
					toAdd = info;
				}
			}
			else
			{
				if (holder.Setting.MethodsIncludeMode == MethodsIncludeMode.Implicit)
				{
					toAdd = new PersistenceConversationAttribute();
				}
			}
			if (toAdd != null && toAdd.ConversationEndMode == EndMode.Unspecified)
			{
				toAdd.ConversationEndMode = holder.Setting.DefaultEndMode;
			}
			if(toAdd != null)
			{
				holder.AddMethodInfo(method, toAdd);
			}
		}
	}
}