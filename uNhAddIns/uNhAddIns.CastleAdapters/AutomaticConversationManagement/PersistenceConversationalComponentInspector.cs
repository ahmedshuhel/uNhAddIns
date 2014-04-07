using System.Collections.Generic;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder.Inspectors;
using uNhAddIns.Adapters.Common;
using System.Linq;

namespace uNhAddIns.CastleAdapters.AutomaticConversationManagement
{
	public class PersistenceConversationalComponentInspector : MethodMetaInspector
	{
		private ReflectionConversationalMetaInfoStore _metaStore;

		public override void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (_metaStore == null)
			{
			    _metaStore = (ReflectionConversationalMetaInfoStore)kernel.Resolve<IConversationalMetaInfoStore>();				
			}

			if (!ConfigureBasedOnAttributes(model))
			{
				return;
			}

			Validate(model, _metaStore);
			AddConversationInterceptorIfIsConversational(model, _metaStore);
		}

		private static void AddConversationInterceptorIfIsConversational(ComponentModel model, IConversationalMetaInfoStore store)
		{
			var meta = store.GetMetadataFor(model.Implementation);

			if (meta == null)
			{
				return;
			}

            
			model.Dependencies.Add(new DependencyModel(null, typeof (ConversationInterceptor), false));
			model.Interceptors.Add(new InterceptorReference(typeof (ConversationInterceptor)));
		}

		private static void Validate(ComponentModel model, IConversationalMetaInfoStore store)
		{		    
		    if (!model.HasClassServices || model.Services.Any(s=> s.IsInterface))
			{
				return;
			}

			var meta = store.GetMetadataFor(model.Implementation);

			if (meta == null)
			{
				return;
			}

			var problematicMethods = new List<string>(10);

			foreach (MethodInfo method in meta.Methods)
			{
				if (!method.IsVirtual)
				{
					problematicMethods.Add(method.Name);
				}
			}

			if (problematicMethods.Count != 0)
			{
				string[] methodNames = problematicMethods.ToArray();

				string message =
					string.Format(
						"The class {0} wants to use persistence-conversation interception, "
						+ "however the methods must be marked as virtual in order to do so. Please correct "
						+ "the following methods: {1}", model.Implementation.FullName, string.Join(", ", methodNames));

				throw new FacilityException(message);
			}
		}

		private bool ConfigureBasedOnAttributes(ComponentModel model)
		{
			return _metaStore.Add(model.Implementation);
		}


		protected override string ObtainNodeName()
		{
			return "NotSupportedYet";
		}

	}
}