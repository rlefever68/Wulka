using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.ComponentModel;
using System.Collections;

namespace DeploymentUpdate.DTO
{
    public class RecipientCollection : ConfigurationElementCollection, INotifyPropertyChanged, ICollection<ConfigurationElement>
    {
        public RecipientCollection(IEnumerable<MailAddressSurrogate> recipients)
            : this()
        {
            foreach (var item in recipients)
            {
                BaseAdd(((ConfigurationElement)((RecipientElement)item)));
            }
        }

        public RecipientCollection()
            : base()
        {

        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new RecipientElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RecipientElement)element).Name;
        }

        public event CollectionChangeEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnCollectionChanged(CollectionChangeAction action, ConfigurationElement element)
        {
            if (PropertyChanged != null)
            {
                CollectionChanged(this, new CollectionChangeEventArgs(action, element));
            }
        }

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void Add(ConfigurationElement item)
        {
            base.BaseAdd(item);
            OnCollectionChanged(CollectionChangeAction.Add, item);
        }

        public void Clear()
        {
            base.BaseClear();
            OnCollectionChanged(CollectionChangeAction.Refresh, null);
        }

        public bool Contains(ConfigurationElement item)
        {
            foreach (var element in this)
            {
                if (((RecipientElement)item).Name == ((RecipientElement)element).Name && ((RecipientElement)item).Email==((RecipientElement)element).Email)
                    return true;
            }
            return false;
        }

        public new bool IsReadOnly
        {
            get { return base.IsReadOnly(); }
        }

        public bool Remove(ConfigurationElement item)
        {
            if (this.Contains(item))
            {
                base.BaseRemove(item);
                OnCollectionChanged(CollectionChangeAction.Remove, item);
                return true;
            }
            return false;
        }

        public new IEnumerator<ConfigurationElement> GetEnumerator()
        {
            return (IEnumerator<ConfigurationElement>)base.GetEnumerator();
        }
    }
}
