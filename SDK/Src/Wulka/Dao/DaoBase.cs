using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;

namespace Wulka.Dao
{
    /// <summary>
    /// Provides basic functionality needed by all data access objects
    /// </summary>
    public class DaoBase
    {
        /// <summary>
        /// Gets or sets the connection key.
        /// </summary>
        /// <value>The connection key.</value>
        public string ConnectionKey { get; private set; }

        #region Protected Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="DaoBase"/> class.
        /// </summary>
        /// <param name="connectionKey">The connection key.</param>
        protected DaoBase(string connectionKey)
        {
            ConnectionKey = connectionKey;
        }

		/// <summary>
        /// Gets the configured command Timeout value from the configuration file.
        /// Returns a default value of 30 seconds if no or empty entry with the name 'CommandTimeoutInSeconds' found in the configuration file.
        /// </summary>
        protected int ConfiguredCommandTimeout
        {
            get
            {
                const string commandTimeoutAppSettingsKey = "CommandTimeoutInSeconds";
                int? iCommandTimeout = null;
                string commandTimeout = ConfigurationManager.AppSettings[commandTimeoutAppSettingsKey];
                if (!string.IsNullOrEmpty(commandTimeout))
                {
                    try
                    {
                        iCommandTimeout = Convert.ToInt32(commandTimeout);
                    }
                    catch(Exception ex)
                    {
                        // Get the application configuration file.
                        System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        string errorMessage = string.Format(
                            "Invalid CommandTimeout: the configuration file {0} does not contain an appropriate value for key '{1}' in the section AppSettings",
                            config.FilePath, commandTimeoutAppSettingsKey);
                        throw new ApplicationException(errorMessage, ex);
                    }
                }
                return iCommandTimeout ?? 30;
            }
        }
			
        protected SqlConnection GetConnection(string connectionStringsKey)
        {
            string connectionString = null;
            ConnectionStringSettings connectionStringSettings =
                ConfigurationManager.ConnectionStrings[connectionStringsKey];
            if (connectionStringSettings != null)
            {
                connectionString = ConfigurationManager.ConnectionStrings[connectionStringsKey].ConnectionString;
            }
            if (string.IsNullOrEmpty(connectionString))
            {
                // Get the application configuration file.
                System.Configuration.Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                string errorMessage = string.Format(
                    "No connection string found for target database"
                    + ", please provide a key '{0}' in the section connectionStrings of the configuration file {1} and set its value to the appropriate connection string for the target database.",
                    connectionStringsKey, config.FilePath);
                throw new ApplicationException(errorMessage);
            }
            return new SqlConnection(connectionString);
        }

        protected static string Serialize<TTable>(TTable value) where TTable : class
        {
            using(MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer dataContractSerializer = new DataContractSerializer(typeof (TTable));
                dataContractSerializer.WriteObject(memoryStream, value);
                memoryStream.Position = 0; // Position to begin of stream
                using (StreamReader streamReader = new StreamReader(memoryStream))
                {
                    return streamReader.ReadToEnd();
                }
            }

            ////Stopwatch timer = Stopwatch.StartNew();
            //XmlSerializer s = new XmlSerializer(typeof(TTable));
            //StringWriter w = new StringWriter();
            //s.Serialize(w, value);
            //w.Close();
            //string result = w.ToString();
            ////timer.Stop();
            ////Logger.LogDebug(typeof(Auditor), "Value serialized ({0}): {1}", timer.ToStringInSeconds(), result);
            //return result;
        }

        #endregion
    }
}
