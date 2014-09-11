using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DeploymentUpdate.Service.UI.Interface.Configuration;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace DeploymentUpdate.Service.UI.Interface
{
    /// <summary>
    /// Interaction logic for AddRecipient.xaml
    /// </summary>
    public partial class AddRecipient : UserControl, INotifyPropertyChanged
    {


        private String _DisplayName = String.Empty;
        private String _EmailAddress = String.Empty;
        private bool _IsValid = false;
        private MailAddressSurrogate _Recipient;

        private static EventLog _EventLog = new EventLog("Application", ".", "Deployment Update Windows Service Configuration Client");

        public event PropertyChangedEventHandler PropertyChanged;
        
        public MailAddressSurrogate Recipient { get { return _Recipient; } }
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
                string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
                Regex regEmail = new Regex(strRegex);
                IsValid = regEmail.IsMatch(value);
                _EmailAddress = value;
                OnPropertyChanged("EmailAddress");
            }
        }
        public bool IsValid
        {
            get { return _IsValid; }
            set
            {
                if (value == _IsValid) return;
                _IsValid = value;
                OnPropertyChanged("IsValid");
            }
        }

        public AddRecipient()
        {
            InitializeComponent();
        }

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _Recipient = new MailAddressSurrogate();
                _Recipient.DisplayName = this.DisplayName;
                _Recipient.EmailAddress = this.EmailAddress ;
                var parentWindow = this.GetParentWindow();
                parentWindow.DialogResult = true;
                parentWindow.Close();
            }
            catch (Exception ex)
            {
                _EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this._Recipient = null;
                var parentWindow = this.GetParentWindow();
                parentWindow.DialogResult = false;
                parentWindow.Close();
            }
            catch (Exception ex)
            {
                _EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private Window GetParentWindow()
        {
            try
            {
                DependencyObject dpParent = this.Parent as DependencyObject;
                while (!(dpParent.GetType()== typeof(Window) ||dpParent.GetType().BaseType == typeof(Window)))
                {
                    dpParent = LogicalTreeHelper.GetParent(dpParent);
                }
                return dpParent as Window;
            }
            catch (Exception ex)
            {
                _EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }
    }
}
