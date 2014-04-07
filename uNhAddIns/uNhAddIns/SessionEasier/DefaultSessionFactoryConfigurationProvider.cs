using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Util;

namespace uNhAddIns.SessionEasier
{
	public class DefaultSessionFactoryConfigurationProvider : AbstractConfigurationProvider
	{
		public override IEnumerable<Configuration> Configure()
		{
			var cfg = CreateConfiguration();
			bool configured;
			DoBeforeConfigure(cfg, out configured);
			if (!configured)
			{
				cfg.Configure();
			}
			DoAfterConfigure(cfg);
			return new SingletonEnumerable<Configuration>(cfg);
		}
	}
}