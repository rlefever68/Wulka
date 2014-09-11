using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.ComponentModel;

namespace DeploymentUpdate.DTO
{
    public class ConfigurationServiceSection : ConfigurationSection, INotifyPropertyChanged
    {
        public ConfigurationServiceSection()
            : this(String.Empty, new RecipientCollection())
        { }

        public ConfigurationServiceSection(String watcherPath = "", RecipientCollection recipients = null)
        {
            WatcherPath = watcherPath;
            Recipients = recipients != null ? recipients : new RecipientCollection();
        }

        [ConfigurationProperty("watcherPath", DefaultValue = "")]
        public String WatcherPath
        {
            get
            {
                return (String)this["watcherPath"];
            }
            set
            {
                if (this["watcherPath"].ToString() == value)
                    return;

                this["watcherPath"] = value;
                OnPropertyChanged("WatcherPath");
            }
        }

        [ConfigurationProperty("uriToWatchPath", DefaultValue = "")]
        public String UriToWatchPath
        {
            get
            {
                return (String)this["uriToWatchPath"];
            }
            set
            {
                if (this["uriToWatchPath"].ToString() == value)
                    return;

                this["uriToWatchPath"] = value;
                OnPropertyChanged("uriToWatchPath");
            }
        }

        [ConfigurationProperty("recipients", IsDefaultCollection = true)]
        public RecipientCollection Recipients
        {
            get
            {
                return (RecipientCollection)this["recipients"];
            }
            set
            {
                if (this["recipients"] == value)
                    return;

                if (Recipients != null)
                    Recipients.CollectionChanged -= new CollectionChangeEventHandler(Recipients_CollectionChanged);
                this["recipients"] = value;
                if (Recipients != null)
                    Recipients.CollectionChanged += new CollectionChangeEventHandler(Recipients_CollectionChanged);
                OnPropertyChanged("Recipients");
            }
        }

        void Recipients_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            OnPropertyChanged("Recipients");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}