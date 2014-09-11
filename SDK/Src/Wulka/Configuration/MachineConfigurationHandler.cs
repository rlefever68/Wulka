namespace Wulka.Configuration
{
    public class MachineConfigurationHandler : MicrosoftConfigurationHandlerBase
    {
        private readonly object _sync = new object();
        private System.Configuration.Configuration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="MachineConfigurationHandler"/> class.
        /// </summary>
        public MachineConfigurationHandler()
            : base(ConfigurationHandlerKey.Instance.Machine())
        {
        }

        #region Overrides of MicrosoftConfigurationHandlerBase

        /// <summary>
        /// Opens the configuration.
        /// </summary>
        /// <returns>Configuration object</returns>
        protected override System.Configuration.Configuration OpenConfiguration()
        {
            lock (_sync)
            {
                if (_configuration == null)
                {
                    _configuration = System.Configuration.ConfigurationManager.OpenMachineConfiguration();
                }
            }
            return _configuration;
        }

        /// <summary>
        /// Closes the configuration.
        /// </summary>
        protected override void CloseConfiguration()
        {
            lock(_sync)
            {
                if(_configuration != null)
                {
                    _configuration = null;
                }
            }
        }

        /// <summary>
        /// Gets the fallback priority.
        /// When the a given ConfigurationHandlerBase is not available,
        /// the configuration will be stored in the first available
        /// ConfigurationHandlerBase that CanSaveConfigurationSection the configuration.
        /// The ConfigurationProviders will be searched in ascending FallbackPriority order.
        /// </summary>
        /// <value>The fallback priority.</value>
        public override int FallbackPriority
        {
            get { return 15000; }
        }

        #endregion
    }
}