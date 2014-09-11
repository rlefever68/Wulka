using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DeploymentUpdate.DTO;
using DeploymentUpdate.Common.UI;

namespace DeploymentUpdate.Client.UI
{
    /// <summary>
    /// Interaction logic for DeploymentControl.xaml
    /// </summary>
    public partial class DeploymentControl : UserControl, INotifyPropertyChanged
    {
        public DeploymentControl()
        {
            InitializeComponent();
        }

        private DeploymentSettings _Settings;
        //private bool _IsDirty;

        public DeploymentSettings Settings
        {
            get { return _Settings; }
            set
            {
                if (_Settings == value) return;
                if (_Settings != null)
                    _Settings.PropertyChanged -= new PropertyChangedEventHandler(_Settings_PropertyChanged);
                _Settings = value;
                if (_Settings != null)
                    _Settings.PropertyChanged += new PropertyChangedEventHandler(_Settings_PropertyChanged);

                OnPropertyChanged("Settings");
            }
        }

        void _Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DeploymentSettings.DirtyIndicatorColumns.Contains(e.PropertyName))
                OnPropertyChanged("IsDirty");
        }
        public bool IsDirty
        {
            get
            {
                if (Settings == null)
                    return false;

                foreach (var dirtyIndicator in DeploymentSettings.DirtyIndicatorColumns)
                {
                    var result = typeof(DeploymentSettings).GetProperty(dirtyIndicator).GetValue(Settings, null) as bool?;
                    if (result != null && result.HasValue && result.Value)
                        return true;
                }
                return false;
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".application";
            dlg.Filter = "Application manifest (.application)|*.application";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                Settings = DeploymentSettings.Load(dlg.FileName);
                OnPropertyChanged("IsDirty");
                if (Settings != null)
                    MainDockPanel.IsEnabled = true;
                else
                    MainDockPanel.IsEnabled = false;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == true)
            {
                if (Settings.SignDeployment && !String.IsNullOrEmpty(KeyPassword.Password))
                    Settings.Save(fbd.SelectedImagePath, KeyPassword.Password);
                else if (!Settings.SignDeployment)
                    Settings.Save(fbd.SelectedImagePath);
            }
        }

        private void SigningKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".pfx";
            dlg.Filter = "Key files (.pfx)|*.pfx";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                Settings.SigningKeyLocation = dlg.FileName;
            }
        }
    }
}
