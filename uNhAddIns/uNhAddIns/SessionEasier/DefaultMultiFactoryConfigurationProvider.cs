using System.Collections.Generic;
using System.Configuration;
using Configuration=NHibernate.Cfg.Configuration;

namespace uNhAddIns.SessionEasier
{
	public class DefaultMultiFactoryConfigurationProvider : AbstractConfigurationProvider
	{
		private const string factoriesStart = "nhfactory";

		public override IEnumerable<Configuration> Configure()
		{
			var result = new List<Configuration>(4);
			foreach (string setting in ConfigurationManager.AppSettings.Keys)
			{
				if (setting.StartsWith(factoriesStart))
				{
					string nhConfigFilePath = ConfigurationManager.AppSettings[setting];
					var configuration = CreateConfiguration();

					bool configured;
					DoBeforeConfigure(configuration, out configured);
					if (!configured)
					{
						configuration.Configure(nhConfigFilePath);
					}
					DoAfterConfigure(configuration);

					result.Add(configuration);
				}
			}
			return result.ToArray();
		}
	}
}