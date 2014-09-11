using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Wulka.Configuration
{
    public static class ConfigurationSectionExtension
    {
        /// <summary>
        /// Serializes this instance returning an xml representation of
        /// the ConfigurationSection.
        /// </summary>
        /// <returns>xml string</returns>
        public static string Serialize(this ConfigurationSection section, string sectionName)
        {
            var serializeSection = typeof(ConfigurationSection).GetMethod("SerializeSection", 
                                                                          BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (serializeSection != null)
            {
                var xml = serializeSection.Invoke(section, new[] { new SettingElement(sectionName, SettingsSerializeAs.Xml), sectionName, (object)ConfigurationSaveMode.Full });
                return xml as string;
            }

            return "";
        }

        /// <summary>
        /// Deserializes the specified XML string, filling up the public
        /// properties of this instance
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="xml">The XML string.</param>
        public static void Deserialize(this ConfigurationSection section, string xml)
        {
            var deserializeSection = typeof(ConfigurationSection).GetMethod("DeserializeSection", 
                                                                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);

            if (deserializeSection != null)
            {
                XmlReader xmlReader = XmlReader.Create(new StringReader(xml));
                deserializeSection.Invoke(section, new[] { xmlReader });
            }
        }

        /// <summary>
        /// Serializes this instance returning an xml representation of
        /// the ConfigurationSection.
        /// </summary>
        /// <returns>xml object</returns>
        public static XElement XSerialize(this ConfigurationSection section, string sectionName)
        {
            var xml = section.Serialize(sectionName);

            if (!string.IsNullOrEmpty(xml))
            {
                return XElement.Parse(xml);
            }

            return new XElement(sectionName);
        }

        /// <summary>
        /// Deserializes the specified XML object, filling up the public
        /// properties of this instance
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="xml">The XML object.</param>
        public static void XDeserialize(this ConfigurationSection section, XElement xml)
        {
            section.Deserialize(xml.ToString(SaveOptions.None));
        }
    }
}