using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using DeploymentUpdate.DTO;
using System.Diagnostics;

namespace DeploymentUpdate.DAO
{
    public class NotificationDAO
    {
        private static Notifier _Instance;

        public static event EventHandler<UpdateNotificationEventArgs> UpdateRegistered;
        public static event EventHandler<UnhandledExceptionEventArgs> WarningRegistered;
        public static event EventHandler<UnhandledExceptionEventArgs> ErrorRegistered;


        public static Notifier Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = Notifier.Instance;
                return _Instance;
            }
        }



        public static void Start()
        {
            try
            {
                Notifier.UpdateRegistered += new EventHandler<UpdateNotificationEventArgs>(Notifier_UpdateRegistered);
                Notifier.WarningRegistered += new EventHandler<UnhandledExceptionEventArgs>(Notifier_WarningRegistered);
                Notifier.ErrorRegistered += new EventHandler<UnhandledExceptionEventArgs>(Notifier_ErrorRegistered);
                Instance.Start();


            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(Instance, new UnhandledExceptionEventArgs(ex, false));
            }
        }

        static void Notifier_WarningRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            if (WarningRegistered != null)
                WarningRegistered(sender, e);
        }

        static void Notifier_ErrorRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            if (ErrorRegistered != null)
                ErrorRegistered(sender, e);
        }

        static void Notifier_UpdateRegistered(object sender, UpdateNotificationEventArgs e)
        {
            if (UpdateRegistered != null)
                UpdateRegistered(sender, e);
        }

        public static void Stop()
        {
            try
            {
                if (Instance.IsStarted)
                    Instance.Stop();
                Notifier.UpdateRegistered -= new EventHandler<UpdateNotificationEventArgs>(Notifier_UpdateRegistered);
                Notifier.ErrorRegistered -= new EventHandler<UnhandledExceptionEventArgs>(Notifier_ErrorRegistered);
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(Instance, new UnhandledExceptionEventArgs(ex, false));
            }
        }
    }
}
