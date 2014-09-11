using System.Configuration;
using NLog;
using Wulka.Exceptions;

namespace Wulka.Configuration
{
	public abstract class MicrosoftConfigurationHandlerBase : ConfigurationHandlerBase
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="MicrosoftConfigurationHandlerBase"/> class.
		/// </summary>
		/// <param name="handlerKey">The unique Handler key.</param>
		protected MicrosoftConfigurationHandlerBase(string handlerKey)
			: base(handlerKey)
		{
		}

		/// <summary>
		/// Opens the configuration.
		/// </summary>
		/// <returns>Configuration object</returns>
		protected abstract System.Configuration.Configuration OpenConfiguration();

		/// <summary>
		/// Closes the configuration.
		/// </summary>
		protected abstract void CloseConfiguration();

		#region Overrides of ConfigurationHandlerBase

		/// <summary>
		/// Gets the configurion from the Handler.
		/// </summary>
		/// <typeparam name="T">Type of the configuration</typeparam>
		/// <param name="key">The key of the configuration to retrieve.</param>
		/// <returns>The configuration as T</returns>
		public override T GetConfigurationSection<T>(string key)
		{
			var configuration = OpenConfiguration();
			if(configuration != null)
			{
				try
				{
					var section = configuration.GetSection(key) as T;
					return section;
				}
				catch (ConfigurationErrorsException e)
				{
					_logger.Error("Unable to load {0} : {1}", key, e.GetCombinedMessages());
					configuration.Sections.Remove(key);
					configuration.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(key);
				}
				finally
				{
					CloseConfiguration();
				}
			}

			return default(T);
		}

        private readonly Logger _logger = LogManager.GetLogger("MicrosoftConfigurationHandler");

		/// <summary>
		/// Determines whether the specified key has configuration stored
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the specified key has config; otherwise, <c>false</c>.
		/// </returns>
		public override bool HasConfigurationSection(string key)
		{
			try
			{
				return HasConfigurationSection(key, OpenConfiguration());
			}
			finally
			{
				CloseConfiguration();
			}
		}

		/// <summary>
		/// Determines whether the specified key has configuration stored
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="configuration">The config.</param>
		/// <returns>
		/// 	<c>true</c> if the specified key has configuration; otherwise, <c>false</c>.
		/// </returns>
		public bool HasConfigurationSection(string key, System.Configuration.Configuration configuration)
		{
			if (configuration != null)
			{
				try
				{
					var section = configuration.GetSection(key);
					return section != null;
				}
				catch (ConfigurationErrorsException e)
				{
					_logger.Error("Unable to load {0} : {1}", key, e.GetCombinedMessages());
					configuration.Sections.Remove(key);
					configuration.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(key);
				}
			}

			return false;
		}

		/// <summary>
		/// Determines whether this instance can store the specified configurationSection.
		/// </summary>
		/// <param name="key">The key of the configurationSection.</param>
		/// <param name="configurationSection">The configuration.</param>
		/// <returns>
		/// 	<c>true</c> if this instance can store the specified key; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanSaveConfigurationSection(string key, object configurationSection)
		{
			return configurationSection as ConfigurationSection != null;
		}

		/// <summary>
		/// Stores the configurationSection in the Handler under the given key
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="configurationSection">The configuration to store.</param>
		/// <returns>
		/// 	<c>true</c> if this instance has stored the specified key; otherwise, <c>false</c>.
		/// </returns>
		public override bool SaveConfigurationSection(string key, object configurationSection)
		{
			var data = configurationSection as ConfigurationSection;
			if (data != null)
			{
				try
				{
					var configuration = OpenConfiguration();
					if (HasConfigurationSection(key, configuration))
					{
						var xml = data.Serialize(key);
						configuration.Sections[key].SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
						configuration.Sections[key].SectionInformation.SetRawXml(xml);
						configuration.Save(ConfigurationSaveMode.Minimal);
					}
					else
					{
						data.SectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
						configuration.Sections.Add(key, data);
						configuration.Save(ConfigurationSaveMode.Modified);
					}

					ConfigurationManager.RefreshSection(key);
					return true;
				}
				finally
				{
					CloseConfiguration();
				}
			}

			return false;
		}

		/// <summary>
		/// Removes the configuration from the provider
		/// </summary>
		/// <param name="key">The key of the configuration to remove.</param>
		/// <returns><c>true</c> when the configuration was removed; otherwise <c>false</c></returns>
		public override bool RemoveConfigurationSection(string key)
		{
			try
			{
				var configuration = OpenConfiguration();
				if (HasConfigurationSection(key, configuration))
				{
					configuration.Sections.Remove(key);
					configuration.Save(ConfigurationSaveMode.Modified);
					ConfigurationManager.RefreshSection(key);
				}

				return true;
			}
			finally
			{
				CloseConfiguration();
			}
		}

		#endregion
	}
}