// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="ConfigurationHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Configuration;

namespace Wulka.BigD.Configuration
{
    /// <summary>
    /// Class ConfigurationHelper.
    /// </summary>
    public class ConfigurationHelper
    {

        /// <summary>
        /// Class AppSettingsKey.
        /// </summary>
        public class AppSettingsKey
        {
            /// <summary>
            /// The couch database host
            /// </summary>
            public const string CouchDbHost = "CouchDB.Host";

            /// <summary>
            /// The couch database port
            /// </summary>
            public const string CouchDbPort = "CouchDB.Port";

            /// <summary>
            /// The couch database database
            /// </summary>
            public const string CouchDbDatabase = "CouchDB.Database";

            /// <summary>
            /// The couch database user
            /// </summary>
            public const string CouchDbUser = "CouchDB.User";

            /// <summary>
            /// The couch database password
            /// </summary>
            public const string CouchDbPassword = "CouchDB.Password";

            /// <summary>
            /// The couch database protocol
            /// </summary>
            public const string CouchDbProtocol = "CouchDB.Protocol";

        }


        /// <summary>
        /// Gets the couch database database.
        /// </summary>
        /// <value>The couch database database.</value>
        public static string CouchDbDatabase
        {
            get
            {
                return (ConfigurationManager.AppSettings[AppSettingsKey.CouchDbDatabase]);
            }
        }



        /// <summary>
        /// Gets the couch database host.
        /// </summary>
        /// <value>The couch database host.</value>
        public static string CouchDbHost
        {
            get
            {
                return ConfigurationManager.AppSettings[AppSettingsKey.CouchDbHost] == null
                    ? "localhost"
                    : (ConfigurationManager.AppSettings[AppSettingsKey.CouchDbHost]);
            }
        }

        /// <summary>
        /// Gets the couch database port.
        /// </summary>
        /// <value>The couch database port.</value>
        public static int CouchDbPort
        {
            get
            {
                return ConfigurationManager.AppSettings[AppSettingsKey.CouchDbPort] == null
                    ? 5984
                    : int.Parse(ConfigurationManager.AppSettings[AppSettingsKey.CouchDbPort]);
            }
        }




        /// <summary>
        /// Gets the couch database user.
        /// </summary>
        /// <value>The couch database user.</value>
        public static string CouchDbUser
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.CouchDbUser] == null)
                    return "user";
                return ConfigurationManager.AppSettings[AppSettingsKey.CouchDbUser];
            }

        }

        /// <summary>
        /// Gets the couch database password.
        /// </summary>
        /// <value>The couch database password.</value>
        public static string CouchDbPassword
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.CouchDbUser] == null)
                    return "password";
                return ConfigurationManager.AppSettings[AppSettingsKey.CouchDbPassword];
            }

        }

        /// <summary>
        /// Gets the couch database protocol.
        /// </summary>
        /// <value>The couch database protocol.</value>
        public static string CouchDbProtocol
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.CouchDbProtocol] == null)
                    return "http";
                return ConfigurationManager.AppSettings[AppSettingsKey.CouchDbProtocol];

            }
        }


    }
}
