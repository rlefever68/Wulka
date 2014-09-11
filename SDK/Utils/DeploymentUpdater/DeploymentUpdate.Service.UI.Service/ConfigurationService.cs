using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Net.Mail;
using System.Configuration;
using DeploymentUpdate.DAO;
using DeploymentUpdate.DTO;
using System.Diagnostics;

namespace DeploymentUpdate.Service.UI.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class ConfigurationService : IConfigurationService
    {
        static EventLog EventLog = new EventLog("Application", ".", "Deployment Update Windows Service WCF Configuration Service");
        
        public ConfigurationService()
            :base()
        {            
            EventLog.WriteEntry("Service initialised");
        }               

        public List<MailAddressSurrogate> GetRecipients()
        {
            return ConfigurationDAO.GetRecipients();
        }

        public void UpdateRecipients(List<MailAddressSurrogate> recipients)
        {
            ConfigurationDAO.UpdateRecipients(recipients);
        }

        public string GetWatcherPath()
        {
            return ConfigurationDAO.GetWatcherPath();
        }

        public void UpdateWatcherPath(string watherPath)
        {
            ConfigurationDAO.UpdateWatcherPath(watherPath);
        }

        public string GetUriToWatchPath()
        {
            return ConfigurationDAO.GetUriToWatchPath();
        }

        public void UpdateUriToWatchPath(string watherPath)
        {
            ConfigurationDAO.UpdateUriToWatchPath(watherPath);
        }    
    }
}
