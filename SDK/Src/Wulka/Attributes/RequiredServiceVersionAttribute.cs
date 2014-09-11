using System;
using System.Linq;
using System.Reflection;
using NLog;

namespace Wulka.Attributes
{
    /// <summary>
    /// Exposes a Property that will return the minimum version of an authentication
    /// service that is required to communicate with the current version of an 
    /// authentication agent 
    /// </summary>    
    [AttributeUsage(AttributeTargets.Assembly)]
    public class RequiredServiceVersionAttribute : Attribute
    {
        private readonly string _serviceVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequiredServiceVersionAttribute"/> class.
        /// </summary>
        /// <param name="version">The service version.</param>
        public RequiredServiceVersionAttribute(string version)
        {
            _serviceVersion = version;
        }


        private static readonly Logger Logger = LogManager.GetLogger("RequiredServiceVersionAttribute");

        /// <summary>
        /// Gets the minimum Service version.
        /// </summary>
        /// <value>The Service version.</value>
        public virtual string ServiceVersion
        {
            get
            {
                return _serviceVersion;
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

        /// <summary>
        /// Gets the minimum required agent version from the assembly
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>Minimum required agent version</returns>
        public static Version GetVersion(Assembly assembly)
        {
            var attribute = assembly
                .GetCustomAttributes(false)
                .OfType<RequiredServiceVersionAttribute>()
                .FirstOrDefault();

            if (attribute != null)
            {
                try
                {
                    return new Version(attribute.ServiceVersion);
                }
                catch (ArgumentException ex)
                {
                    Logger
                    .Warn("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.ServiceVersion, ex.Message);
                }
                catch (FormatException ex)
                {Logger
                    .Error("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.ServiceVersion, ex.Message);
                }
                catch (OverflowException ex)
                {
                    Logger
                        .Error("Unable to parse RequiredAgentVersionAttribute {0} : {1}",
                                      attribute.ServiceVersion, ex.Message);
                }
            }

            return new Version(int.MaxValue, int.MaxValue);
        }
    }
}