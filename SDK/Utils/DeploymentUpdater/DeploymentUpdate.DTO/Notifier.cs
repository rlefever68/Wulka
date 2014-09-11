using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Xml;
using System.Configuration;
using System.ComponentModel;
using System.Text;

namespace DeploymentUpdate.DTO
{
    public class Notifier : INotifyPropertyChanged
    {
        private static List<Tuple<string, long, long, WatcherChangeTypes>> History;
        private bool _IsStarted;
        private bool TriggerActivated;
        private FileSystemWatcher _Watcher;

        public static event EventHandler<UpdateNotificationEventArgs> UpdateRegistered;
        public static event EventHandler<UnhandledExceptionEventArgs> ErrorRegistered;
        public static event EventHandler<UnhandledExceptionEventArgs> WarningRegistered;

        private Notifier()
        { }
        public bool IsStarted
        {
            get { return _IsStarted; }
            set
            {
                if (_IsStarted = value)
                    return;
                _IsStarted = value;
                OnPropertyChanged("IsStarted");
            }
        }

        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }
        private ConfigurationServiceSection _ServiceConfiguration;

        private ConfigurationServiceSection ServiceConfiguration
        {
            get
            {
                if (_ServiceConfiguration == null)
                {
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (config == null)
                        if (WarningRegistered != null)
                            WarningRegistered(this, new UnhandledExceptionEventArgs(new ConfigurationErrorsException("Configuration file missing"), false));
                    ConfigurationServiceSection sec = (ConfigurationServiceSection)config.GetSection("DeploymentUpdateConfiguration");
                    if (sec == null)
                    {
                        sec = new ConfigurationServiceSection();
                        config.Sections.Add("DeploymentUpdateConfiguration", sec);
                        config.Save();
                    }
                    _ServiceConfiguration = sec;
                }
                return _ServiceConfiguration;


            }
        }

        private FileSystemWatcher Watcher
        {
            get
            {
                if (_Watcher == null)
                {
                    if (String.IsNullOrEmpty(ServiceConfiguration.WatcherPath))
                    {
                        if (WarningRegistered != null)
                            WarningRegistered(this, new UnhandledExceptionEventArgs(new ConfigurationErrorsException("Watchpath not set"), false));

                        return null;
                    }
                    else
                    {
                        _Watcher = new FileSystemWatcher(ServiceConfiguration.WatcherPath);
                        History = new List<Tuple<string, long, long, WatcherChangeTypes>>();
                        //_Watcher.Filter = "*.application";
                        _Watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.Security | NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.Attributes;
                        _Watcher.IncludeSubdirectories = true;
                        _Watcher.EnableRaisingEvents = true;

                    }

                }
                return _Watcher;
            }
        }

        private static Notifier _Instance;

        public static Notifier Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new Notifier();
                return _Instance;
            }
        }


        public void Start()
        {
            try
            {
                if (_IsStarted)
                {
                    if (WarningRegistered != null)
                        WarningRegistered(this, new UnhandledExceptionEventArgs(new Exception(String.Format("Notifier already started ({0})", ServiceConfiguration.WatcherPath)), false));
                    return;
                }

                if (!Directory.Exists(ServiceConfiguration.WatcherPath))
                {
                    if (WarningRegistered != null)
                        WarningRegistered(this, new UnhandledExceptionEventArgs(new Exception(String.Format("Path ({0}) to monitor is invalid", ServiceConfiguration.WatcherPath)), false));

                    return;
                }

                if (ServiceConfiguration.Recipients == null || ServiceConfiguration.Recipients.Count <= 0)
                {
                    if (ErrorRegistered != null)
                        WarningRegistered(this, new UnhandledExceptionEventArgs(new Exception(String.Format("No recipients found")), false));
                    return;
                }

                Watcher.Created += new FileSystemEventHandler(Watcher_Created);
                Watcher.Changed += new FileSystemEventHandler(Watcher_Changed);
                Watcher.Deleted += new FileSystemEventHandler(Watcher_Deleted);
                _IsStarted = true;
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error while starting notifier", ex), false));
            }
        }

        public void Stop()
        {

            try
            {
                while (TriggerActivated)
                { }

                if (!_IsStarted)
                {
                    if (WarningRegistered != null)
                        WarningRegistered(this, new UnhandledExceptionEventArgs(new Exception("Notifier not started"), false));
                    return;
                }

            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error while stopping Notifier", ex), false));
            }
            finally
            {
                Watcher.Created -= new FileSystemEventHandler(Watcher_Created);
                Watcher.Changed -= new FileSystemEventHandler(Watcher_Changed);
                Watcher.Deleted -= new FileSystemEventHandler(Watcher_Deleted);
                _IsStarted = false;
            }
        }

        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            while (TriggerActivated)
            { }

            NotifyUpdate(e);
        }

        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            while (TriggerActivated)
            { }

            NotifyUpdate(e);
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            while (TriggerActivated)
            { }

            NotifyUpdate(e);
        }

        private void NotifyUpdate(FileSystemEventArgs e)
        {

            StringBuilder sb = new StringBuilder();
            try
            {
                sb.AppendLine("0");
                FileInfo changeFI = new FileInfo(e.FullPath);
                sb.AppendLine("1");
                if (changeFI.Extension.ToLower() != ".application" && changeFI.Extension.ToLower() != ".svc")
                    return;
                sb.AppendLine("2");
                while (!RecentChangesForDirectory(changeFI.Directory))
                { }
                sb.AppendLine("3");
                changeFI.Refresh();
                sb.AppendLine("4");
                switch (e.ChangeType)
                {
                    case WatcherChangeTypes.Changed:
                        sb.AppendLine("5a");
                        if (History.Any
                                    (
                                        x => x.Item1 == e.FullPath &&
                                             x.Item2 >= changeFI.CreationTimeUtc.AddSeconds(-30).Ticks &&
                                             x.Item2 <= changeFI.CreationTimeUtc.AddSeconds(30).Ticks &&
                                             x.Item3 >= changeFI.LastWriteTimeUtc.AddSeconds(-30).Ticks &&
                                             x.Item3 <= changeFI.LastWriteTimeUtc.AddSeconds(30).Ticks
                                    )
                            )
                            return;
                        sb.AppendLine("6a");
                        History.Add(new Tuple<string, long, long, WatcherChangeTypes>(e.FullPath, changeFI.CreationTimeUtc.Ticks, changeFI.LastWriteTimeUtc.Ticks, e.ChangeType));
                        break;
                    case WatcherChangeTypes.Renamed:
                    case WatcherChangeTypes.Created:
                    case WatcherChangeTypes.Deleted:
                        sb.AppendLine("5b");
                        History.RemoveAll(x => x.Item1 == e.FullPath);
                        sb.AppendLine("6b");
                        History.Add(new Tuple<string, long, long, WatcherChangeTypes>(e.FullPath, changeFI.CreationTimeUtc.Ticks, changeFI.LastWriteTimeUtc.Ticks, e.ChangeType));
                        break;
                    default:
                        break;
                }
                sb.AppendLine("7");
                TriggerActivated = true;
                sb.AppendLine("8");
                if (e.ChangeType == WatcherChangeTypes.Renamed || e.ChangeType == WatcherChangeTypes.Created || e.ChangeType == WatcherChangeTypes.Changed)
                {
                    sb.AppendLine("9");
                    Tuple<String, String> message = null;
                    sb.AppendLine("10");
                    if (changeFI.Extension.ToLower() == ".application")
                    {
                        sb.AppendLine("11a");
                        message = GetMessageManifestUpdate(changeFI);
                    }
                    else if (changeFI.Extension.ToLower() == ".svc")
                    {
                        sb.AppendLine("11b");
                        message = GetMessageServiceUpdate(changeFI);
                    }
                    if (message == null || String.IsNullOrEmpty(message.Item1))
                        return;
                    sb.AppendLine("12");
                    if (UpdateRegistered != null)
                        UpdateRegistered(this, new UpdateNotificationEventArgs(changeFI.FullName, message.Item2));
                    sb.AppendLine("13");
                    List<MailAddress> recipients = ServiceConfiguration.Recipients.Cast<RecipientElement>().Select(r => new MailAddress(r.Email, r.Name)).ToList();
                    sb.AppendLine("14");
                    if (message != null && recipients != null)
                        SendNotification(recipients, message.Item1, message.Item2);
                }
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error while performing update notification" + Environment.NewLine + sb.ToString(), ex), false));
            }
            finally
            {
                TriggerActivated = false;
            }
        }

        private List<MailAddress> GetRecipientList(String xmlRecipientsFilePath)
        {
            try
            {
                List<MailAddress> list = new List<MailAddress>();
                FileInfo xmlRecipientsFI = new FileInfo(xmlRecipientsFilePath);
                if (xmlRecipientsFI == null)
                    return null;

                XmlDocument document;
                XmlNamespaceManager namespaceManager;
                namespaceManager = new XmlNamespaceManager(new NameTable());

                document = new XmlDocument();
                document.Load(xmlRecipientsFI.FullName);

                string recipientXPath = "/Recipients/Recipient";
                XmlNodeList recipientNodes = document.SelectNodes(recipientXPath, namespaceManager);
                //recipientNodes.Cast<XmlNode>().First().ChildNodes.Cast<XmlNode>().First().InnerText
                list.AddRange
                    (
                        recipientNodes.Cast<XmlNode>()
                                      .Select
                                      (
                                        recipient => new MailAddress
                                                     (
                                                        recipient.ChildNodes.Cast<XmlNode>()
                                                                            .Where(recipientInfo => recipientInfo.Name.ToLower() == "email")
                                                                            .Select(recipientInfo => recipientInfo.InnerText)
                                                                            .FirstOrDefault(),
                                                        recipient.ChildNodes.Cast<XmlNode>()
                                                                            .Where(recipientInfo => recipientInfo.Name.ToLower() == "name")
                                                                            .Select(recipientInfo => recipientInfo.InnerText)
                                                                            .FirstOrDefault()
                                                     )
                                      )
                    );

                return list;
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error retrieving recipients", ex), false));
                return new List<MailAddress>();
            }
        }

        private Tuple<string, string> GetMessageManifestUpdate(FileInfo changeFI)
        {
            try
            {

                DeploymentSettings settings = DeploymentSettings.Load(changeFI.FullName);
                if (settings == null)
                    return null;

                return new Tuple<string, string>
                       (
                            String.Format
                            (
                                !String.IsNullOrEmpty(settings.SuiteName) ? "Deployment update for {0} ({1})" : "Deployment update for {1}",
                                settings.SuiteName,
                                !String.IsNullOrEmpty(settings.ProductName) ? settings.ProductName : String.Empty
                            ),
                            String.Format
                            (
                                @"<html><head /><body><p>There is an update found for following deployment</p><p>{0}{1}{2}{3}</p></body></html>",
                                !String.IsNullOrEmpty(settings.SuiteName) ? String.Format("Suite name: {0}<br/>", settings.SuiteName) : String.Empty,
                                !String.IsNullOrEmpty(settings.ProductName) ? String.Format("Product name: {0}<br/>", settings.ProductName) : String.Empty,
                                !String.IsNullOrEmpty(settings.Version) ? String.Format("Version: {0}<br/>", settings.Version) : String.Empty,
                                String.IsNullOrEmpty(ServiceConfiguration.UriToWatchPath) ?
                                    String.Format
                                    (
                                        @"Update location: {0} on the BackOffice deploymentserver<br/>",
                                        settings.DeploymentManifestFile.Directory.FullName
                                    ) :
                                    String.Format
                                    (
                                        @"Update location: <a href=""{0}"">{0}</a><br/>",
                                        String.Concat
                                        (
                                            ServiceConfiguration.UriToWatchPath,
                                            settings.DeploymentManifestFile.Directory.FullName.Replace
                                                                                              (
                                                                                                ServiceConfiguration.WatcherPath,
                                                                                                ServiceConfiguration.WatcherPath.Substring(ServiceConfiguration.WatcherPath.Length - 1, 1) == @"\" ? @"\" : ""
                                                                                              )
                                        )
                                    )
                            )
                        );
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error while getting update information", ex), false));
                return null;
            }
        }

        private Tuple<string, string> GetMessageServiceUpdate(FileInfo changeFI)
        {
            try
            {

                WCFServiceSettings settings = WCFServiceSettings.Load(changeFI.FullName);

                if (settings == null)
                    return null;

                return new Tuple<string, string>
                       (
                            String.Format
                            (
                                "Service update for {0}",
                                settings.ServiceName
                            ),
                            String.Format
                            (
                                @"<html><head /><body><p>There is an update found for following Service</p><p>{0}{1}{2}{3}</p></body></html>",
                                !String.IsNullOrEmpty(settings.ServiceName) ? String.Format("Service name: {0}<br/>", settings.ServiceName) : String.Empty,
                                !String.IsNullOrEmpty(settings.Language) ? String.Format("Language: {0}<br/>", settings.Language) : String.Empty,
                                !String.IsNullOrEmpty(settings.Factory) ? String.Format("Factory: {0}<br/>", settings.Factory) : String.Empty,
                                String.IsNullOrEmpty(ServiceConfiguration.UriToWatchPath) ?
                                    String.Format
                                    (
                                        @"Update location: {0} on the BackOffice deploymentserver<br/>",
                                        changeFI.Directory.FullName
                                    ) :
                                    String.Format
                                    (
                                        @"Update location: <a href=""{0}"">{0}</a><br/>",
                                        String.Concat
                                        (
                                            ServiceConfiguration.UriToWatchPath,
                                            changeFI.Directory.FullName.Replace
                                                                                              (
                                                                                                ServiceConfiguration.WatcherPath,
                                                                                                ServiceConfiguration.WatcherPath.Substring(ServiceConfiguration.WatcherPath.Length - 1, 1) == @"\" ? @"\" : ""
                                                                                              )
                                        )
                                    )
                            )
                        );

            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error while getting update information", ex), false));
                return null;
            }
        }

        private void SendNotification(List<MailAddress> recipients, String subject, String content)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                foreach (var recipient in recipients)
                {
                    mailMessage.To.Add(recipient);
                }
                mailMessage.From = new MailAddress("BOGEN@prodatamobility.com", "Deployment Update Service");
                mailMessage.Subject = subject;
                mailMessage.Body = content;
                mailMessage.IsBodyHtml = true;
                SmtpClient client = new SmtpClient("PSBEMAIL.psbe.site");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Credentials = new NetworkCredential("BoGen", "RaLe110105", "PSBE");
                client.Send(mailMessage);               
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception("Error sending update notification", ex), false));
            }

        }

        private bool RecentChangesForDirectory(DirectoryInfo info)
        {

            bool result = false;
            try
            {
                if (info.GetFiles().Any(x => x.LastWriteTime <= DateTime.Now.AddSeconds(-15)))
                    result = true;
                if (info.GetDirectories().Any(d => RecentChangesForDirectory(d)))
                    result = true;
            }
            catch (Exception ex)
            {
                if (ErrorRegistered != null)
                    ErrorRegistered(this, new UnhandledExceptionEventArgs(new Exception(String.Format("Error while monitoring update of files in directory {0}", info.FullName), ex), false));
            }
            return result;

        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
