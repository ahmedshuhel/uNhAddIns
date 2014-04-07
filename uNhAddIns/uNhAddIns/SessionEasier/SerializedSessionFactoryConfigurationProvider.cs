using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using NHibernate.Cfg;

namespace uNhAddIns.SessionEasier
{
	/// <summary>
	/// Serialize the first time the configuration to a file. 
	/// Use the serialized version until the assembly or configuration change.
	/// This class was extracted from the Effectus sample of Ayende:
	/// http://msdn.microsoft.com/en-us/magazine/ee819139.aspx
	/// </summary>
	public class SerializedSessionFactoryConfigurationProvider : AbstractConfigurationProvider
	{
		private readonly string configFile = "hibernate.cfg.xml";
		private readonly string serializedConfiguration = "configuration.serialized";

		public SerializedSessionFactoryConfigurationProvider()
		{
		}

		public SerializedSessionFactoryConfigurationProvider(
			string serializedConfiguration,
			string configFile)
		{
			this.serializedConfiguration = serializedConfiguration;
			this.configFile = configFile;
		}


		private bool IsConfigurationFileValid
		{
			get
			{
				Assembly ass = Assembly.GetCallingAssembly();
				if (ass.Location == null)
					return false;
				var configInfo = new FileInfo(serializedConfiguration);
				var assInfo = new FileInfo(ass.Location);
				var configFileInfo = new FileInfo(configFile);
				if (configInfo.LastWriteTime < assInfo.LastWriteTime)
					return false;
				if (configInfo.LastWriteTime < configFileInfo.LastWriteTime)
					return false;
				return true;
			}
		}

		public override IEnumerable<Configuration> Configure()
		{
			var configuration = new Configuration();

			bool configured;
			DoBeforeConfigure(configuration, out configured);

			if (!configured)
			{
				configuration = LoadConfigurationFromFile();

				if (configuration == null)
				{
					configuration = CreateConfiguration();
					configuration.Configure(configFile);

					DoAfterConfigure(configuration);
					SaveConfigurationToFile(configuration);
				}
			}
			return new List<Configuration> {configuration};
		}

		private void SaveConfigurationToFile(Configuration configuration)
		{
			using (FileStream file = File.Open(serializedConfiguration, FileMode.Create))
			{
				var bf = new BinaryFormatter();
				bf.Serialize(file, configuration);
			}
		}

		private Configuration LoadConfigurationFromFile()
		{
			if (IsConfigurationFileValid == false)
				return null;
			try
			{
				using (var file = File.Open(serializedConfiguration, FileMode.Open))
				{
					var bf = new BinaryFormatter();
					return bf.Deserialize(file) as Configuration;
				}
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}