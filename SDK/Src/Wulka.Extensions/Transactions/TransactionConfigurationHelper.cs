using System;
using System.Configuration;

namespace Wulka.Transactions
{


    public class TransactionConfigurationKey
    {
        public const string InboxPath = "Transactions.InboxPath";
        public const string TemporaryPath = "Transactions.TemporaryPath";
        public const string QueueHost = "Transactions.QueueHost";
        public const string RouterPluginAssemblyLocation = "Router.PluginAssemblyLocation";

    }



    public class TransactionConfigurationHelper
    {
        public static string InboxPath
        {
            get
            {
                return ConfigurationManager.AppSettings[TransactionConfigurationKey.InboxPath];
            }
        }

        public static string RouterPluginAssemblyLocation
        {
            get
            {
                return ConfigurationManager.AppSettings[TransactionConfigurationKey.RouterPluginAssemblyLocation];
            }
        }

            
        public static string TemporaryPath
        {
            get
            {
                return ConfigurationManager.AppSettings[TransactionConfigurationKey.TemporaryPath];
            }
        }


        public static string QueueHost
        {
            get
            {
                return String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[TransactionConfigurationKey.QueueHost]) ? "localhost" : ConfigurationManager.AppSettings[TransactionConfigurationKey.QueueHost];
            }
        }
            
    }
}
