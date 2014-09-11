using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeploymentUpdate.DAO;
using DeploymentUpdate.Service.UI.Interface;
using System.IO;
using System.ServiceProcess;
using System.ComponentModel;
using System.Diagnostics;
using DeploymentUpdate.DTO;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;

namespace DeploymentUpdate.Test
{
    class Program
    {
        static EventLog EventLog = new EventLog("Application");
        static void Main(string[] args)
        {
            //EventLog.Source = "DUN Test";
            //NotificationDAO.UpdateRegistered += new EventHandler<UpdateNotificationEventArgs>(NotificationDAO_UpdateRegistered);
            //NotificationDAO.WarningRegistered += new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_WarningRegistered);
            //NotificationDAO.ErrorRegistered += new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_ErrorRegistered);

            //NotificationDAO.Start();
            //Console.ReadLine();
            //NotificationDAO.Stop();
            //NotificationDAO.UpdateRegistered -= new EventHandler<UpdateNotificationEventArgs>(NotificationDAO_UpdateRegistered);
            //NotificationDAO.WarningRegistered -= new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_WarningRegistered);
            //NotificationDAO.ErrorRegistered -= new EventHandler<UnhandledExceptionEventArgs>(NotificationDAO_ErrorRegistered);
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.To.Add(new MailAddress("bruno.de.wilde@prodatamobility.com", "Bruno De Wilde"));
                mailMessage.From = new MailAddress("BOGEN@prodatamobility.com","Deployment Update Service");
                mailMessage.Subject = "test";
                mailMessage.Body = "test";
                mailMessage.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("PSBEMAIL.psbe.site");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential("BoGen", "RaLe110105", "PSBE");
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        static void NotificationDAO_WarningRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (e.ExceptionObject as Exception);
            EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Warning);
        }
        static void NotificationDAO_ErrorRegistered(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (e.ExceptionObject as Exception);
            EventLog.WriteEntry(String.Format("Following error was encountered {0}{1}{0}{2}{0}{3}", Environment.NewLine, ex.Message, ex.InnerException != null ? ex.InnerException.Message : String.Empty, ex.StackTrace), System.Diagnostics.EventLogEntryType.Error);
        }
        static void NotificationDAO_UpdateRegistered(object sender, UpdateNotificationEventArgs e)
        {
            EventLog.WriteEntry(String.Format("Following update was found on {1}{0}{2}", Environment.NewLine, e.UpdatePath, e.Message), System.Diagnostics.EventLogEntryType.Information);
        }

    }
}
