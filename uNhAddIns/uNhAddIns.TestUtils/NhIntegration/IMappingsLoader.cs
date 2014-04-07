using NHibernate.Cfg;

namespace uNhAddIns.TestUtils.NhIntegration
{
	public interface IMappingsLoader
	{
		void LoadMappings(Configuration configuration);
	}
}