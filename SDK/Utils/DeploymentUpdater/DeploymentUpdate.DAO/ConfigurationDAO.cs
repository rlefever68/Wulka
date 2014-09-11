using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net.Mail;
using DeploymentUpdate.DTO;

namespace DeploymentUpdate.DAO
{
    public class ConfigurationDAO
    {
        private static Configuration _Config;
        
        private static Configuration Config
        {
            get
            {
                if(_Config==null)
                    _Config=ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                return _Config;
            }
        }

        private static ConfigurationServiceSection Section
        {
            get
            {
                if (Config == null)
                    throw new ConfigurationErrorsException();
                ConfigurationServiceSection sec = (ConfigurationServiceSection)Config.GetSection("DeploymentUpdateConfiguration");
                if (sec == null)
                {
                    Config.Sections.Add("DeploymentUpdateConfiguration", new ConfigurationServiceSection());
                    Config.Save(ConfigurationSaveMode.Minimal, true);

                }
                return sec;
            }
        }

        public static List<MailAddressSurrogate> GetRecipients()
        {
            return Section.Recipients.Cast<RecipientElement>().Select(i => (MailAddressSurrogate)i).ToList();
        }

        public static void UpdateRecipients(IEnumerable<MailAddressSurrogate> recipients)
        {
            Section.Recipients = new RecipientCollection(recipients);
            Config.Save(ConfigurationSaveMode.Minimal, true);
        }

        public static String GetWatcherPath()
        {
            return Section.WatcherPath;
        }

        public static void UpdateWatcherPath(String path)
        {
            Section.WatcherPath = path;
            Config.Save(ConfigurationSaveMode.Minimal, true);
        }

        public static String GetUriToWatchPath()
        {
            return Section.UriToWatchPath;
        }

        public static void UpdateUriToWatchPath(String path)
        {
            Section.UriToWatchPath = path;
            Config.Save(ConfigurationSaveMode.Minimal, true);
        }
    }
}
