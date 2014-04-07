using System;
using System.Configuration;
using System.Linq;

namespace uNhAddIns.Adapters
{
	/// <summary>
	/// Helper class to get the <see cref="IGuyWire"/> concrete implementation
	/// from application config.
	/// </summary>
	/// <remarks>
	/// The appSetting section should have a key named "GuyWire" (case insensitive)
	/// <example>
	/// <![CDATA[
	/// <appSettings>
	///	<add key='GuyWire' value='YourCompany.Product.Wiring.IoC_Fx.GuyWire, YourCompany.Product.Wiring.IoC_Fx'/>
	/// </appSettings>"
	/// ]]>
	/// </example>
	/// </remarks>
	public static class ApplicationConfiguration
	{
		private const string GuyWireConfKey = "guywire";
		private const string GuyWireConfMessage =
			@"The GuyWire was not configured.
Example
	<appSettings>
		<add key='GuyWire' value='YourCompany.Product.Wiring.IoC_Fx.GuyWire, YourCompany.Product.Wiring.IoC_Fx'/>
	</appSettings>";

		/// <summary>
		/// Read the configuration to instantiate the <see cref="IGuyWire"/>.
		/// </summary>
		/// <returns>The instance of <see cref="IGuyWire"/>.</returns>
		/// <exception cref="ApplicationException">
		/// If the key='GuyWire' was not found or if the <see cref="IGuyWire"/> can't be instancied.
		/// </exception>
		public static IGuyWire GetGuyWire()
		{
			var guyWireClassKey =
				ConfigurationManager.AppSettings.Keys.Cast<string>().FirstOrDefault(k => GuyWireConfKey.Equals(k.ToLowerInvariant()));
			if (string.IsNullOrEmpty(guyWireClassKey))
			{
				throw new ApplicationException(GuyWireConfMessage);
			}
			var guyWireClass = ConfigurationManager.AppSettings[guyWireClassKey];
			var type = Type.GetType(guyWireClass);
			try
			{
				return (IGuyWire)Activator.CreateInstance(type);
			}
			catch (MissingMethodException ex)
			{
				throw new ApplicationException("Public constructor was not found for " + type, ex);
			}
			catch (InvalidCastException ex)
			{
				throw new ApplicationException(type + "Type does not implement " + typeof(IGuyWire), ex);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Unable to instantiate: " + type, ex);
			}
		}
	}
}