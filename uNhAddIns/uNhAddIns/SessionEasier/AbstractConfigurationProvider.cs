using System;
using System.Collections.Generic;
using NHibernate.Cfg;

namespace uNhAddIns.SessionEasier
{
	public abstract class AbstractConfigurationProvider : IConfigurationProvider
	{
		#region Implementation of IConfigurationProvider

		public abstract IEnumerable<Configuration> Configure();
		public event EventHandler<ConfiguringEventArgs> BeforeConfigure;
		public event EventHandler<ConfigurationEventArgs> AfterConfigure;

		#endregion

		protected void DoAfterConfigure(Configuration cfg)
		{
			if (AfterConfigure != null)
			{
				AfterConfigure(this, new ConfigurationEventArgs(cfg));
			}
		}

		protected void DoBeforeConfigure(Configuration cfg, out bool configured)
		{
			configured = false;
			if (BeforeConfigure == null)
			{
				return;
			}
			var args = new ConfiguringEventArgs(cfg);
			BeforeConfigure(this, args);
			configured = args.Configured;
		}

		protected virtual Configuration CreateConfiguration()
		{
			return new Configuration();
		}
	}
}