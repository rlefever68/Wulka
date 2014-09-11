using System;
using System.Linq;
using System.Reflection;
using NLog;
using Wulka.Exceptions;

namespace Wulka.Attributes
{
    /// <summary>
    /// Exposes a Property that will return the minimum version of an authentication
    /// agent that is required to communicate with the current version of an 
    /// authentication service 
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RequiredAgentVersionAttribute : Attribute
    {

        private readonly string _agentVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredAgentVersionAttribute"/> class.
        /// </summary>
        /// <param name="version">The required agent version.</param>
        public RequiredAgentVersionAttribute(string version)
        {
            _agentVersion = version;
        }
        protected RequiredAgentVersionAttribute()
        {
            
        }

        /// <summary>
        /// Gets the minimum required agent version.
        /// </summary>
        /// <value>The required agent version.</value>
        public string AgentVersion
        {
            get
            {
                return _agentVersion;
            }
        }

        /// <summary>
        /// Gets the minimum required agent version from the executing assembly
        /// </summary>
        /// <returns>Minimum required agent version</returns>
        public static Version GetVersion()
        {
            return GetVersion(Assembly.GetExecutingAssembly());
        }


        private static Logger _logger = LogManager.GetLogger("RequiredAgentVersionAttribute");

        /// <summary>
        /// Gets the minimum required agent version from the assembly
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Minimum required agent version</returns>
        public static Version GetVersion(Assembly assembly)
        {
            var attribute = assembly
                .GetCustomAttributes(false)
                .OfType<RequiredAgentVersionAttribute>()
                .FirstOrDefault();

            if(attribute != null)
            {
                try
                {
                    return new Version(attribute.AgentVersion);
                }
                catch (ArgumentException ex)
                {
                    _logger.Warn("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.AgentVersion, ex.Message);
                        
                }
                catch (FormatException ex)
                {
                    _logger
                    .Warn("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.AgentVersion, ex.Message);
                }
                catch (OverflowException ex)
                {
                    _logger
                        .Error("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.AgentVersion, ex.Message);
                    _logger.Error(ex.GetCombinedMessages());
                }
            }

            return new Version(int.MaxValue, int.MaxValue);
        }
    }
}