using System.Configuration;

namespace Wulka.Configuration
{
    public class WulkaAServiceCodeSection: ConfigurationSection
    {
        /// <summary>
        /// Gets or sets a value that names the section.
        /// </summary>
        [ConfigurationProperty("name", DefaultValue = "serviceCodes", IsRequired = true, IsKey = false)]
        [StringValidator(InvalidCharacters = " ~!@#$%^&*()[]{}/;'\"|\\", MinLength = 1, MaxLength = 60)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        /// <summary>
        /// Gets the <see cref="WulkaAServiceCodeCollection"/> that holds all the configured <see cref="WulkaAServiceCodeElement"/> elements.
        /// </summary>
        /// <seealso cref="WulkaAServiceCodeCollection"/>
        [ConfigurationProperty("codes", IsDefaultCollection = false)]
        public WulkaAServiceCodeCollection ServiceCodes
        {
            get { return (WulkaAServiceCodeCollection)base["codes"]; }
        }
    }
}
