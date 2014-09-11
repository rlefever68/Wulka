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
using DeploymentUpdate.Common.UI;
using System.ComponentModel;
using DeploymentUpdate.DAO;
using DeploymentUpdate.Service.UI.Interface.Configuration;
using System.ServiceProcess;
using System.Collections.ObjectModel;
using DeploymentUpdater.Common.UI.Converters;
using System.Globalization;
using System.Diagnostics;
using log4net;

namespace DeploymentUpdate.Service.UI.Interface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServiceConfiguration : UserControl, INotifyPropertyChanged
    {
        private String _WatcherPath;
        private String _OldWatcherPath;
        private String _UriToWatchPath;
        private String _OldUriWatchPath;
        private ObservableCollection<MailAddressSurrogate> _Recipients = new ObservableCollection<MailAddressSurrogate>();
        private ObservableCollection<MailAddressSurrogate> _OldRecipients = new ObservableCollection<MailAddressSurrogate>();
        private ServiceController service;
        //private static EventLog _EventLog;
        public readonly ILog _log = LogManager.GetLogger(typeof(ServiceConfiguration));

        public event PropertyChangedEventHandler PropertyChanged;

        public String WatcherPath
        {
            get
            {
                if (_WatcherPath == null)
                    _WatcherPath = String.Empty;
                return _WatcherPath;
            }
            set
            {
                if (value == _WatcherPath) return;
                _WatcherPath = value;
                OnPropertyChanged("WatcherPath");
            }
        }
        public String UriToWatchPath
        {
            get
            {
                if (_UriToWatchPath == null)
                    _UriToWatchPath = String.Empty;
                return _UriToWatchPath;
            }
            set
            {
                if (value == _UriToWatchPath) return;
                _UriToWatchPath = value;
                OnPropertyChanged("UriToWatchPath");
            }
        }
        public ObservableCollection<MailAddressSurrogate> Recipients
        {
            get
            {
                return _Recipients;
            }
            set
            {
                if (value == _Recipients) return;
                if (_Recipients != null)
                    _Recipients.CollectionChanged -= new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Recipients_CollectionChanged);
                _Recipients = new ObservableCollection<MailAddressSurrogate>(value);
                if (_Recipients != null)
                    _Recipients.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(_Recipients_CollectionChanged);
                OnPropertyChanged("Recipients");
            }
        }
        public ServiceControllerStatus State
        {
            get
            {
                if (service == null)
                    return ServiceControllerStatus.StartPending;
                return service.Status;
            }
        }
        public bool IsServiceStarted
        {
            get
            {
                switch (State)
                {
                    case ServiceControllerStatus.Running:
                        {
                            return true;
                        }// break;
                    default:
                        {
                            return false;
                        }// break;
                }

            }
        }
        public bool IsDirty
        {
            get
            {
                return (State == ServiceControllerStatus.Running) &&
                       (_OldRecipients.Any(x => !Recipients.Contains(x)) ||
                       Recipients.Any(x => !_OldRecipients.Contains(x)) ||
                       _OldWatcherPath != WatcherPath ||
                       _OldUriWatchPath != UriToWatchPath);
            }
        }

        public ServiceConfiguration()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.Configure();
        }


        void _Recipients_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged("Recipients");
        }

        void RunWorkerCompleted_FindService(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                service = e.Result as ServiceController;
                OnPropertyChanged("State");
                OnPropertyChanged("IsServiceStarted");
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(DoWork_CheckState);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_CheckState);
                bgw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        void RunWorkerCompleted_CheckState(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                OnPropertyChanged("State");
                OnPropertyChanged("IsServiceStarted");
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(DoWork_CheckState);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_CheckState);
                bgw.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        void DoWork_CheckState(object sender, DoWorkEventArgs e)
        {
            try
            {
                String state = service.Status.ToString();
                while (state == service.Status.ToString())
                {
                    service.Refresh();
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        void DoWork_FindService(object sender, DoWorkEventArgs e)
        {
            try
            {
                ServiceController temp = null;
                DateTime startTime = DateTime.Now;
                String serviceName="Deployment Update Windows Service";
                while (e.Result == null)
                {
                    try
                    {
                        temp = new ServiceController(serviceName);

                        if (temp.DisplayName == serviceName)
                            e.Result = temp;
                    }
                    catch (Exception ex)
                    {
                        if (startTime.AddHours(1) > DateTime.Now)
                        {
                            _log.Error("The service 'Deployment Update Windows Service' is not installed", ex);
                            startTime = DateTime.Now;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void WatcherPathButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == true)
                {
                    WatcherPath = fbd.SelectedImagePath;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Window w = new Window();
                AddRecipient ar = new AddRecipient();

                w.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                w.WindowStyle = WindowStyle.ToolWindow;
                w.SizeToContent = SizeToContent.WidthAndHeight;
                w.Content = ar;
                if (w.ShowDialog() == true)
                    Recipients.Add(ar.Recipient);

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Recipients = new ObservableCollection<MailAddressSurrogate>(Recipients.Where(r => !((DevExpress.Xpf.Grid.TableView)RecipientsGrid.View).SelectedRows.Cast<MailAddressSurrogate>().ToList().Contains(r)).ToList());
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        protected internal void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (ConfigurationServiceClient client = new ConfigurationServiceClient())
                {
                    if (_OldRecipients.Any(x => !Recipients.Contains(x)) || Recipients.Any(x => !_OldRecipients.Contains(x)))
                    {
                        client.UpdateRecipients(Recipients.ToList());
                        _OldRecipients = Recipients;
                    }
                    if (_OldWatcherPath != WatcherPath)
                    {
                        client.UpdateWatcherPath(WatcherPath);
                        _OldWatcherPath = WatcherPath;
                    }
                    if (_OldUriWatchPath != UriToWatchPath)
                    {
                        client.UpdateUriToWatchPath(UriToWatchPath);
                        _OldUriWatchPath = UriToWatchPath;
                    }
                }
                if (service != null && !service.ServiceHandle.IsInvalid)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    service.Start();
                }

            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GetParentWindow().Close();
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _log.Info("Client Starting");
            //_EventLog = new EventLog("Application", ".", "Deployment Update Windows Service Configuration Client");
            //_EventLog.WriteEntry("Client starting");
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()) == true)
                return;


            try
            {
                BackgroundWorker bgw = new BackgroundWorker();
                bgw.DoWork += new DoWorkEventHandler(DoWork_FindService);
                bgw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted_FindService);
                bgw.RunWorkerAsync();

                using (ConfigurationServiceClient client = new ConfigurationServiceClient())
                {
                    //State = client.GetServiceStatus();
                    WatcherPath = client.GetWatcherPath();
                    _OldWatcherPath = WatcherPath;
                    UriToWatchPath = client.GetUriToWatchPath();
                    _OldUriWatchPath = UriToWatchPath;
                    Recipients = new ObservableCollection<MailAddressSurrogate>(client.GetRecipients());
                    _OldRecipients = Recipients;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void ServiceActionButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (service != null && !service.ServiceHandle.IsInvalid)
                {
                    switch (State)
                    {
                        case ServiceControllerStatus.Stopped:
                        case ServiceControllerStatus.Paused:
                            service.Start();
                            break;
                        case ServiceControllerStatus.Running:
                            service.Stop();
                            break;
                        case ServiceControllerStatus.StartPending:
                        case ServiceControllerStatus.StopPending:
                        case ServiceControllerStatus.ContinuePending:
                        case ServiceControllerStatus.PausePending:
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }

        private void OnPropertyChanged(String propertyname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyname));
                PropertyChanged(this, new PropertyChangedEventArgs("IsDirty"));
            }
        }

        private Window GetParentWindow()
        {
            try
            {
                DependencyObject dpParent = this.Parent as DependencyObject;
                while (dpParent.GetType().BaseType != typeof(Window))
                {
                    dpParent = LogicalTreeHelper.GetParent(dpParent);
                }
                return dpParent as Window;
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                //_EventLog.WriteEntry(String.Format("Error occurred during initiation {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), EventLogEntryType.Error);
                throw ex;
            }
        }


    }
}
