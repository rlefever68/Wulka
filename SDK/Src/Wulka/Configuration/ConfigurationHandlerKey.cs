namespace Wulka.Configuration
{
    public class ConfigurationHandlerKey
    {
        public static ConfigurationHandlerKey Instance = new ConfigurationHandlerKey();

        /// <summary>
        /// Key name for the Memory Handler.
        /// </summary>
        /// <returns>Memory Handler Key</returns>
        public string Memory()
        {
            return "memory";
        }

        /// <summary>
        /// Key name for the Machine Configuration Handler.
        /// </summary>
        /// <returns>Machine Configuration Handler Key</returns>
        public string Machine()
        {
            return "machine";
        }

        /// <summary>
        /// Key name for the Windows Registry Handler.
        /// </summary>
        /// <returns>Windows Registry Handler Key</returns>
        public string Registry()
        {
            return "registry";
        }

        /// <summary>
        /// Key name for the Microsoft app.config Configuration Handler
        /// </summary>
        /// <returns>Exe Configuration Handler Key</returns>
        public string Exe()
        {
            return "exe";
        }

        /// <summary>
        /// Key name for the Microsoft ConfigurationManager Handler for local user settings
        /// </summary>
        /// <returns>Local Configuration Handler Key</returns>
        public string Local()
        {
            return "local";
        }
    }
}