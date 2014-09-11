using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using DeploymentUpdate.DAO;
using System.ServiceModel;
using DeploymentUpdate.Service.UI.Service;
using DeploymentUpdate.DTO;
namespace DeploymentUpdate.NotificationService
{

    public partial class NotificationService : ServiceBase
    {

        ServiceHost host;
        public NotificationService()
        {

            try
            {
                InitializeComponent();                
            }
            catch (Exception ex)
            {

                this.EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        void NotificationDAO_WarningRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (e.ExceptionObject as Exception);
            this.EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Warning);
        }
        void NotificationDAO_ErrorRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (e.ExceptionObject as Exception);
            this.EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
        }
        void NotificationDAO_UpdateRegistered(object sender, UpdateNotificationEventArgs e)
        {
            this.EventLog.WriteEntry(String.Format("Following update was found on {1}{0}{2}", Environment.NewLine, e.UpdatePath, e.Message), System.Diagnostics.EventLogEntryType.Information);
        }

        protected override void OnStart(string[] args)
        {
            OnCustomCommand(1);
        }
        protected override void OnStop()
        {
            OnCustomCommand(0);
        }
        protected override void OnPause()
        {
            OnCustomCommand(0);
        }
        protected override void OnContinue()
        {
            OnCustomCommand(1);
        }
        protected override void OnShutdown()
        {
            OnCustomCommand(0);
        }
        protected override void OnCustomCommand(int command)
        {
                    EventLog.Source = "Deployment Update Windows Service";
            try
            {
                if (command == 0)
                {
                    this.EventLog.WriteEntry("Service stopping ...");
                    NotificationDAO.Stop();
                    host.Close();
                    host = null;
                    NotificationDAO.UpdateRegistered -= new EventHandler<UpdateNotificationEventArgs>(NotificationDAO_UpdateRegistered);
                    NotificationDAO.WarningRegistered -= new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_WarningRegistered);
                    NotificationDAO.ErrorRegistered -= new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_ErrorRegistered);
                }
                else if (command == 1)
                {
                    this.EventLog.WriteEntry("Service starting ...");
                    NotificationDAO.UpdateRegistered += new EventHandler<UpdateNotificationEventArgs>(NotificationDAO_UpdateRegistered);
                    NotificationDAO.WarningRegistered += new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_WarningRegistered);
                    NotificationDAO.ErrorRegistered += new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_ErrorRegistered);

                    if (host != null)
                    {
                        host.Close();
                    }
                    host = new ServiceHost(typeof(ConfigurationService));
                    host.Open();
                    
                    EventLog.WriteEntry(String.Format("WCF ConfigurationService hosted at {0} ({1})", host.BaseAddresses.FirstOrDefault().AbsoluteUri, host.State));
                    NotificationDAO.Start();
                }
                else if (command == 2)
                {
                    this.EventLog.WriteEntry("Service restarting ...");
                    NotificationDAO.Stop();
                    NotificationDAO.Start();
                }
            }
            catch (Exception ex)
            {
                this.EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException!=null?ex.InnerException.Message:String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
            }
        }        
    }
}
