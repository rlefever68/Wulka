using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using System.ComponentModel;

namespace DeploymentUpdate.DTO
{
    public class RecipientElement : ConfigurationElement, INotifyPropertyChanged
    {

        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)base["name"]; }
            set 
            {
                if (base["name"].ToString() == value) 
                    return;
                
                base["name"] = value;
                OnPropertyChanged("Name");
            }
        }

        [ConfigurationProperty("email", IsRequired = true)]
        public string Email
        {
            get { return (string)base["email"]; }
            set {
                if (base["email"].ToString() == value)
                    return;
                
                base["email"] = value;
                OnPropertyChanged("Email");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public static explicit operator RecipientElement(MailAddressSurrogate value)
        {
            if (value == null)
                return null;
            return new RecipientElement
            {
                Name = value.DisplayName,
                Email = value.EmailAddress
            };
        }
        public static explicit operator MailAddressSurrogate(RecipientElement value)
        {
            if (value == null)
                return null;
            return new MailAddressSurrogate { EmailAddress = value.Email, DisplayName = value.Name };
        }

    }
}
