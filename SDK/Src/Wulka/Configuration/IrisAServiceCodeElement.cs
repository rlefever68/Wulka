using System.Configuration;

namespace Wulka.Configuration
{
    public class WulkaAServiceCodeElement: ConfigurationElement
    {
        /// <summary>
        /// Gets or sets a value that identifies the <see cref="WulkaAServiceCodeElement"/> uniquely.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "ServiceCode1", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("serviceType",  IsRequired = true)]
        public string ServiceType
        {
            get { return (string)this["serviceType"]; }
            set { this["serviceType"] = value; }
        }

        [ConfigurationProperty("code", IsRequired = true)]
        public string ServiceCode
        {
            get { return (string)this["code"]; }
            set { this["code"] = value; }
        }

        public WulkaAServiceCodeElement()
        {
            
        }

        public WulkaAServiceCodeElement(string name, string serviceCode, string serviceType)
        {
            Name = name;
            ServiceCode = serviceCode;
            ServiceType = serviceType;
        }
    }
}
