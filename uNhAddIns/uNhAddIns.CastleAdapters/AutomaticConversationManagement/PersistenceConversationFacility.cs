using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using uNhAddIns.Adapters.Common;

namespace uNhAddIns.CastleAdapters.AutomaticConversationManagement
{
	public class PersistenceConversationFacility : AbstractFacility
	{
		protected override void Init()
		{
		    
            Kernel.Register(Component.For<ConversationInterceptor>().Named("uNhAddIns.conversation.interceptor"));
            Kernel.Register(Component.For<IConversationalMetaInfoStore>().ImplementedBy<ReflectionConversationalMetaInfoStore>().Named("uNhAddIns.conversation.MetaInfoStore"));
			Kernel.ComponentModelBuilder.AddContributor(new PersistenceConversationalComponentInspector());
		}
	}
}