using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using NLog;

namespace Wulka.Configuration
{
    /// <summary>
    /// Exposes methods to store and retrieve data to and from a specified
    /// Microsoft Application Configuration File.
    /// </summary>
    public class ExeConfigurationHandler : MicrosoftConfigurationHandlerBase
    {


        private readonly object _sync = new object();
        private System.Configuration.Configuration _configuration;
        private readonly string _file = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExeConfigurationHandler"/> class.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="handlerKey">The handler key.</param>
        public ExeConfigurationHandler(string file, string handlerKey)
            : base(handlerKey)
        {
            _file = file;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExeConfigurationHandler"/> class.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="handlerKey">The handler key.</param>
        public ExeConfigurationHandler(Assembly assembly, string handlerKey)
            : base(handlerKey)
        {
            if (assembly != null)
            {
                _file = Path.GetFileName(assembly.Location);
            }
            else
            {
                _logger.Warn("No assembly specified, using default Configuration");
            }
        }

        private static Logger _logger = LogManager.GetLogger("ExeConfigurationHandler");

        /// <summary>
        /// Initializes a new instance of the <see cref="ExeConfigurationHandler"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="handlerKey">The handler key.</param>
        public ExeConfigurationHandler(Type type, string handlerKey)
            : base(handlerKey)
        {
            if (type != null)
            {
                _file = Path.GetFileName(Assembly.GetAssembly(type).Location);
            }
            else
            {
               _logger.Warn("No type specified, using default Configuration");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExeConfigurationHandler"/> class.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="handlerKey">The handler key.</param>
        public ExeConfigurationHandler(object obj, string handlerKey)
            : base(handlerKey)
        {
            if (obj != null)
            {
                _file = Path.GetFileName(Assembly.GetAssembly(obj.GetType()).Location);
            }
            else
            {
                _logger.Warn("No object specified, using default Configuration");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExeConfigurationHandler"/> class.
        /// </summary>
        public ExeConfigurationHandler()
            : base(ConfigurationHandlerKey.Instance.Exe())
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
                    _configuration = string.IsNullOrEmpty(_file)
                                         ? ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None)
                                         : ConfigurationManager.OpenExeConfiguration(_file);
                }
            }
            return _configuration;
        }

        /// <summary>
        /// Closes the configuration.
        /// </summary>
        protected override void CloseConfiguration()
        {
            lock (_sync)
            {
                if (_configuration != null)
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
            get { return 14000; }
        }

        #endregion
    }
}