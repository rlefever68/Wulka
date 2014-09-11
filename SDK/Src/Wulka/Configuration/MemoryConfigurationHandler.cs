using System.Collections.Generic;

namespace Wulka.Configuration
{
    /// <summary>
    /// Implements a ConfigurationHandlerBase for storing ConfigurationSettings
    /// in memory.
    /// </summary>
    public class MemoryConfigurationHandler : ConfigurationHandlerBase
    {
        private readonly Dictionary<string, object> _store = new Dictionary<string, object>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryConfigurationHandler"/> class.
        /// </summary>
        /// <param name="providerId">The provider id.</param>
        public MemoryConfigurationHandler(string providerId)
            : base(providerId)
        {
        }

        public MemoryConfigurationHandler() : base(ConfigurationHandlerKey.Instance.Memory())
        {
        }

        /// <summary>
        /// Gets the configurion from the Handler.
        /// </summary>
        /// <typeparam name="T">Type of the configuration</typeparam>
        /// <param name="key">The key of the configuration to retrieve.</param>
        /// <returns>The configuration as T</returns>
        public override T GetConfigurationSection<T>(string key)
        {
            return _store[key] as T;
        }

        /// <summary>
        /// Determines whether the specified key has configuration stored
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key has config; otherwise, <c>false</c>.
        /// </returns>
        public override bool HasConfigurationSection(string key)
        {
            return _store.ContainsKey(key);
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
            return true;
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
            _store[key] = configurationSection;
            return true;
        }

        /// <summary>
        /// Removes the configuration from the provider
        /// </summary>
        /// <param name="key">The key of the configuration to remove.</param>
        /// <returns><c>true</c> when the configuration was removed; otherwise <c>false</c></returns>
        public override bool RemoveConfigurationSection(string key)
        {
            if (HasConfigurationSection(key))
                _store.Remove(key);

            return true;
        }
    }
}