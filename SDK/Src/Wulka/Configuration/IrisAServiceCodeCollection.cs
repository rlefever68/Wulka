using System.Configuration;

namespace Wulka.Configuration
{
    public class WulkaAServiceCodeCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new WulkaAServiceCodeElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((WulkaAServiceCodeElement)element).Name;
        }

        public WulkaAServiceCodeElement this[int index]
        {
            get { return (WulkaAServiceCodeElement)BaseGet(index); }
            set
            {
                if (BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public new WulkaAServiceCodeElement this[string name]
        {
            get { return (WulkaAServiceCodeElement)BaseGet(name); }
        }
    }
}
