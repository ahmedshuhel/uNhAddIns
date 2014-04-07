using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Engine;
using NHibernate.Tool.hbm2ddl;

namespace uNhAddIns.TestUtils.NhIntegration
{
	public class NoOpMappingsLoader : IMappingsLoader
	{
		#region Implementation of IMappingLoader

		public void LoadMappings(Configuration configuration) {}

		#endregion
	}

	public class AssemblyMappingsLoader : IMappingsLoader
	{
		private readonly Assembly assembly;

		public AssemblyMappingsLoader(Assembly assembly)
		{
			this.assembly = assembly;
		}

		#region Implementation of IMappingLoader

		public void LoadMappings(Configuration configuration)
		{
			configuration.AddAssembly(assembly);
		}

		#endregion
	}

	public class NamespaceMappingsLoader : IMappingsLoader
	{
		private readonly Assembly assembly;
		private readonly string mappingNamespace;

		public NamespaceMappingsLoader(Assembly assembly, string mappingNamespace)
		{
			this.assembly = assembly;
			this.mappingNamespace = mappingNamespace;
		}

		#region Implementation of IMappingLoader

		public void LoadMappings(Configuration configuration)
		{
			foreach (var resource in assembly.GetManifestResourceNames())
			{
				if (resource.StartsWith(mappingNamespace) && resource.EndsWith(".hbm.xml"))
				{
					configuration.AddResource(resource, assembly);
				}
			}
		}

		#endregion
	}

	public class ResourceWithRelativeNameMappingsLoader : IMappingsLoader
	{
		private readonly Assembly assembly;
		private readonly string baseName;
		private readonly string[] mappingsName;

		public ResourceWithRelativeNameMappingsLoader(Assembly assembly, string baseName, string[] mappingsName)
		{
			this.assembly = assembly;
			this.baseName = baseName;
			this.mappingsName = mappingsName;
		}

		#region Implementation of IMappingLoader

		public void LoadMappings(Configuration configuration)
		{
			foreach (var file in mappingsName)
			{
				configuration.AddResource(baseName + "." + file, assembly);
			}
		}

		#endregion
	}

	public class DefaultFunctionalTestSettings : IFunctionalTestSettings
	{
		private readonly IMappingsLoader mappingLoader;

		public DefaultFunctionalTestSettings(IMappingsLoader mappingLoader)
		{
			this.mappingLoader = mappingLoader;
		}

		#region Implementation of IFunctionalTestSettings

		public bool AssertAllDataRemoved { get; set; }

		public virtual void Configure(Configuration configuration)
		{
			configuration.Configure();
		}

		public virtual void SchemaSetup(Configuration configuration)
		{
			new SchemaExport(configuration).Create(false, true);
		}

		public virtual void SchemaShutdown(Configuration configuration)
		{
			new SchemaExport(configuration).Drop(false, true);
		}

		public bool SchemaShutdownAfterFailure { get; set; }

		public void LoadMappings(Configuration configuration)
		{
			mappingLoader.LoadMappings(configuration);
		}

		public virtual void BeforeSessionFactoryBuilt(Configuration configuration) {}

		public virtual void AfterSessionFactoryBuilt(ISessionFactoryImplementor sessionFactory) {}
		public virtual void BeforeSchemaShutdown(ISessionFactoryImplementor factory) {}

		#endregion
	}
}