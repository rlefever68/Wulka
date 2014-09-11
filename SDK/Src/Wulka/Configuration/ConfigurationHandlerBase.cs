using Wulka.Core;

namespace Wulka.Configuration
{
    /// <summary>
    /// Exposes the abstract interface that needs to be implemented 
    /// by each ConfigurationHandlerBase.
    /// </summary>
    public abstract class ConfigurationHandlerBase : IHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationHandlerBase"/> class.
        /// 
        /// A unique ConfigurationHandlerKey must be given to identify the Handler.
        /// </summary>
        /// <param name="handlerKey">The unique Handler key.</param>
        protected ConfigurationHandlerBase(string handlerKey)
        {
            HandlerKey = handlerKey;
        }

        /// <summary>
        /// Gets the configurion from the Handler.
        /// </summary>
        /// <typeparam name="T">Type of the configuration</typeparam>
        /// <param name="key">The key of the configuration to retrieve.</param>
        /// <returns>The configuration as T</returns>
        public abstract T GetConfigurationSection<T>(string key) where T : class;

        /// <summary>
        /// Determines whether the specified key has configuration stored
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// 	<c>true</c> if the specified key has config; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool HasConfigurationSection(string key);

        /// <summary>
        /// Determines whether this instance can store the specified configurationSection.
        /// </summary>
        /// <param name="key">The key of the configurationSection.</param>
        /// <param name="configurationSection">The configuration.</param>
        /// <returns>
        /// 	<c>true</c> if this instance can store the specified key; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanSaveConfigurationSection(string key, object configurationSection);

        /// <summary>
        /// Stores the configurationSection in the Handler under the given key
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="configurationSection">The configuration to store.</param>
        /// <returns>
        /// 	<c>true</c> if this instance has stored the specified key; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool SaveConfigurationSection(string key, object configurationSection);

        /// <summary>
        /// Removes the configuration from the provider
        /// </summary>
        /// <param name="key">The key of the configuration to remove.</param>
        /// <returns><c>true</c> when the configuration was removed; otherwise <c>false</c></returns>
        public abstract bool RemoveConfigurationSection(string key);

        /// <summary>
        /// Gets the fallback priority.
        /// 
        /// When the a given ConfigurationHandlerBase is not available,
        /// the configuration will be stored in the first available
        /// ConfigurationHandlerBase that CanSaveConfigurationSection the configuration.
        /// 
        /// The ConfigurationProviders will be searched in ascending FallbackPriority order.
        /// </summary>
        /// <value>The fallback priority.</value>
        public virtual int FallbackPriority { get { return int.MaxValue; } }

        /// <summary>
        /// Gets the Handler key that will uniquely
        /// identify a provider in a ProviderManagerBase.
        /// </summary>
        /// <value>The Handler key.</value>
        public string HandlerKey
        {
            get; private set;
        }
    }
}