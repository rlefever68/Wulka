using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DeploymentUpdate.DTO
{
    public class MailAddressSurrogate : INotifyPropertyChanged
    {
        private String _DisplayName=String.Empty;
        private String _EmailAddress = String.Empty;

        public String DisplayName
        {
            get { return _DisplayName; }
            set
            {
                if (value == _DisplayName) return;
                _DisplayName = value;
                OnPropertyChanged("DisplayName");
            }
        }
        public String EmailAddress
        {
            get { return _EmailAddress; }
            set
            {
                if (value == _EmailAddress) return;
                _EmailAddress = value;
                OnPropertyChanged("EmailAddress");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MailAddressSurrogate()
            :base()
        {}

        private void OnPropertyChanged(String propertyname)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
        }
    }
}
