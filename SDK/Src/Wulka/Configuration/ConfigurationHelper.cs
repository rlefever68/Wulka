// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-31-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-13-2014
// ***********************************************************************
// <copyright file="ConfigurationHelper.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Configuration;
using Wulka.Authentication;
using Wulka.Crypto;
using Wulka.Domain;

namespace Wulka.Configuration
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
            /// The probe
            /// </summary>
            public const string CloudProbe = "Cloud.Probe";

            /// <summary>
            /// The announce
            /// </summary>
            public const string CloudAnnounce = "Cloud.Announce";

            /// <summary>
            /// The disco endpoint
            /// </summary>
            public const string DiscoEndpoint = "Disco.Endpoint";

            /// <summary>
            /// The announce wave
            /// </summary>
            public const string AnnounceWave = "Announce.Wave";


            /// <summary>
            /// The allow SSL
            /// </summary>
            public const string AllowSSL = "Client.AllowSSL";

            /// <summary>
            /// The has required objects
            /// </summary>
            public const string HasRequiredObjects = "HasRequiredObjects";

            /// <summary>
            /// The data mode
            /// </summary>
            public const string DataMode = "DataMode";

            /// <summary>
            /// The is initializing
            /// </summary>
            public const string IsInitializing = "IsInitializing";

            /// <summary>
            /// The retry treshold
            /// </summary>
            public const string RetryTreshold = "Retry.Treshold";

            /// <summary>
            /// The retry pause
            /// </summary>
            public const string RetryPause = "Retry.Pause";

            /// <summary>
            /// The has register domain objects
            /// </summary>
            public const string HasRegisterDomainObjects = "Service.AutoRegisterDomain";

            /// <summary>
            /// The upscale bindings
            /// </summary>
            public const string UpscaleBindings = "Service.UpscaleBindings";

            /// <summary>
            /// The enable mex
            /// </summary>
            public const string EnableMex = "Service.EnableMex";

            /// <summary>
            /// The enable discovery
            /// </summary>
            public const string EnableDiscovery = "Service.EnableDiscovery";

            /// <summary>
            /// The desktop mode
            /// </summary>
            public const string DesktopMode = "Desktop.Mode";

            /// <summary>
            /// The applet authenticate
            /// </summary>
            public const string AppletAuthenticate = "Applet.Authenticate";

            /// <summary>
            /// The root enc password
            /// </summary>
            public const string RootEncPwd = "Root.EncPwd";

            /// <summary>
            /// The is session validation active
            /// </summary>
            public const string IsSessionValidationActive = "Session.Validate";

            /// <summary>
            /// The session timeout
            /// </summary>
            public const string SessionTimeout = "Session.Timeout";

            /// <summary>
            /// The announce delay
            /// </summary>
            public const string AnnounceDelay = "Announce.Delay";


            /// <summary>
            /// The secure mex
            /// </summary>
            public const string SecureMex = "Service.SercureMex";

            /// <summary>
            /// The add default endpoints
            /// </summary>
            public const string AddDefaultEndpoints = "Service.AddDefaultEndpoints";

            /// <summary>
            /// The accept all certificates
            /// </summary>
            public const string AcceptAllCertificates = "DebugOnly.AcceptAllCertificates";

            /// <summary>
            /// The service common name
            /// </summary>
            public const string ServiceCommonName = "Service.CommonName";

            /// <summary>
            /// The extensions path
            /// </summary>
            public const string ExtensionsPath = "Extensions.Path";

            /// <summary>
            /// The service do register
            /// </summary>
            public const string ServiceDoRegister = "Service.Register";

            /// <summary>
            /// The service help URI
            /// </summary>
            public const string ServiceHelpUri = "Service.HelpUri";

            /// <summary>
            /// The log composition
            /// </summary>
            public const string LogComposition = "Composition.Logging";

            public const string CacheSyncInterval = "CacheSync.Interval";

            public const string IsRestful = "Service.IsRestful";
            public static string SmtpHost = "Smtp.Host";
        }


        /// <summary>
        /// Gets a value indicating whether [upscale bindings].
        /// </summary>
        /// <value><c>true</c> if [upscale bindings]; otherwise, <c>false</c>.</value>
        public static bool UpscaleBindings
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.UpscaleBindings] == null) return true;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.UpscaleBindings]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [allow SSL].
        /// </summary>
        /// <value><c>true</c> if [allow SSL]; otherwise, <c>false</c>.</value>
        public static bool AllowSSL
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.AllowSSL] == null) return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.AllowSSL]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [enable mex].
        /// </summary>
        /// <value><c>true</c> if [enable mex]; otherwise, <c>false</c>.</value>
        public static bool EnableMex
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.EnableMex] == null) return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.EnableMex]);
            }
        }


        /// <summary>
        /// Gets a value indicating whether [enable discovery].
        /// </summary>
        /// <value><c>true</c> if [enable discovery]; otherwise, <c>false</c>.</value>
        public static bool EnableDiscovery
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.EnableDiscovery] == null) return true;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.EnableDiscovery]);
            }
        }











        /// <summary>
        /// Gets or sets a value indicating whether this instance is initializing.
        /// </summary>
        /// <value><c>true</c> if this instance is initializing; otherwise, <c>false</c>.</value>
        public static bool IsInitializing
        {
            get { return (ConfigurationManager.AppSettings[AppSettingsKey.IsInitializing] == "T"); }
            set { ConfigurationManager.AppSettings[AppSettingsKey.IsInitializing] = value ? "T" : "F"; }
        }


        /// <summary>
        /// Gets the data mode.
        /// </summary>
        /// <value>The data mode.</value>
        public static string DataMode
        {
            get { return ConfigurationManager.AppSettings[AppSettingsKey.DataMode]; }
        }





        /// <summary>
        /// Gets the retry treshold.
        /// </summary>
        /// <value>The retry treshold.</value>
        public static int RetryTreshold
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.RetryTreshold] == null)
                    return 10;
                return Convert.ToInt32(ConfigurationManager.AppSettings[AppSettingsKey.RetryTreshold]);
            }
        }

        /// <summary>
        /// Gets the retry pause.
        /// </summary>
        /// <value>The retry pause.</value>
        public static int RetryPause
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.RetryPause] == null)
                    return 5;
                return Convert.ToInt32(ConfigurationManager.AppSettings[AppSettingsKey.RetryPause]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has register domain objects.
        /// </summary>
        /// <value><c>true</c> if this instance has register domain objects; otherwise, <c>false</c>.</value>
        public static bool HasRegisterDomainObjects
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.HasRegisterDomainObjects] == null)
                    return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.HasRegisterDomainObjects]);

            }
        }

        /// <summary>
        /// Gets the launcher mode.
        /// </summary>
        /// <value>The launcher mode.</value>
        public static string LauncherMode
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.DesktopMode] == null)
                    return DesktopMode.Ribbon;
                return Convert.ToString(ConfigurationManager.AppSettings[AppSettingsKey.DesktopMode]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [authenticate applet].
        /// </summary>
        /// <value><c>true</c> if [authenticate applet]; otherwise, <c>false</c>.</value>
        public static bool AuthenticateApplet
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.AppletAuthenticate] == null)
                    return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.AppletAuthenticate]);
            }
        }

        /// <summary>
        /// Gets the root enc PWD.
        /// </summary>
        /// <value>The root enc PWD.</value>
        public static string RootEncPwd
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.RootEncPwd] == null)
                    return CryptoEngine.Encrypt(AuthenticationDefaults.RootPwd);
                return ConfigurationManager.AppSettings[AppSettingsKey.RootEncPwd];
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is session validation active.
        /// </summary>
        /// <value><c>true</c> if this instance is session validation active; otherwise, <c>false</c>.</value>
        public static bool IsSessionValidationActive
        {
            get
            {
                return (ConfigurationManager.AppSettings[AppSettingsKey.IsSessionValidationActive] != null) &&
                       Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.IsSessionValidationActive]);
            }
        }

        /// <summary>
        /// Gets the session timeout.
        /// </summary>
        /// <value>The session timeout.</value>
        public static double SessionTimeout
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.SessionTimeout] == null)
                    return 0;
                return Convert.ToDouble(ConfigurationManager.AppSettings[AppSettingsKey.SessionTimeout]);
            }
        }

        /// <summary>
        /// Gets the announce delay.
        /// </summary>
        /// <value>The announce delay.</value>
        public static TimeSpan AnnounceDelay
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.AnnounceDelay] == null)
                    return TimeSpan.FromSeconds(1);
                return
                    TimeSpan.FromSeconds(Convert.ToDouble(ConfigurationManager.AppSettings[AppSettingsKey.AnnounceDelay]));
            }
        }






        /// <summary>
        /// Gets a value indicating whether [secure mex].
        /// </summary>
        /// <value><c>true</c> if [secure mex]; otherwise, <c>false</c>.</value>
        public static bool SecureMex
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.SecureMex] == null)
                    return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.SecureMex]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [add default endpoints].
        /// </summary>
        /// <value><c>true</c> if [add default endpoints]; otherwise, <c>false</c>.</value>
        public static bool AddDefaultEndpoints
        {
            get
            {
                return ConfigurationManager.AppSettings[AppSettingsKey.AddDefaultEndpoints] == null ||
                       Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.AddDefaultEndpoints]);
            }
        }

        /// <summary>
        /// Gets a value indicating whether [accept all certificates].
        /// </summary>
        /// <value><c>true</c> if [accept all certificates]; otherwise, <c>false</c>.</value>
        public static bool AcceptAllCertificates
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.AcceptAllCertificates] == null)
                    return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.AcceptAllCertificates]);
            }
        }

        /// <summary>
        /// Gets the name of the service common.
        /// </summary>
        /// <value>The name of the service common.</value>
        public static string ServiceCommonName
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.ServiceCommonName] == null)
                    return "Service was not specified in Service.CommonName";
                return ConfigurationManager.AppSettings[AppSettingsKey.ServiceCommonName];
            }
        }

        /// <summary>
        /// Gets the disco endpoint.
        /// </summary>
        /// <value>The disco endpoint.</value>
        /// <exception cref="System.Exception">No discovery Endpoint was specified in app.config</exception>
        public static string DiscoEndpoint
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.DiscoEndpoint] != null)
                    return ConfigurationManager.AppSettings[AppSettingsKey.DiscoEndpoint];
                throw new Exception(String.Format("No Discovery Endpoint was specified in app.config! Please check {0}.", ConfigurationHelper.AppSettingsKey.DiscoEndpoint));
            }
        }

        /// <summary>
        /// Gets the announce endpoint.
        /// </summary>
        /// <value>The announce endpoint.</value>
        /// <exception cref="System.Exception">No announcement endpoint was specified in app.config</exception>
        public static string CloudAnnounce
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.CloudAnnounce] != null)
                    return ConfigurationManager.AppSettings[AppSettingsKey.CloudAnnounce];
                throw new Exception(String.Format("No Announcement Endpoint was specified in app.config! Please check {0}.", ConfigurationHelper.AppSettingsKey.CloudAnnounce));
            }
        }

        /// <summary>
        /// Gets the probe endpoint.
        /// </summary>
        /// <value>The probe endpoint.</value>
        /// <exception cref="System.Exception">No probing endpoint was specified in app.config</exception>
        public static string CloudProbe
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.CloudProbe] != null)
                    return
                        ConfigurationManager.AppSettings[AppSettingsKey.CloudProbe];
                throw new Exception(String.Format("No Probing Endpoint was specified in app.config! Please check {0}.", ConfigurationHelper.AppSettingsKey.CloudProbe));
            }
        }

        /// <summary>
        /// Gets the extensions path.
        /// </summary>
        /// <value>The extensions path.</value>
        public static string ExtensionsPath
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.ExtensionsPath] == null)
                    return String.Empty;
                return
                    ConfigurationManager.AppSettings[AppSettingsKey.ExtensionsPath];
            }
        }


        /// <summary>
        /// Gets a value indicating whether [register service metadata].
        /// </summary>
        /// <value><c>true</c> if [register service metadata]; otherwise, <c>false</c>.</value>
        public static bool RegisterServiceMetadata 
        {
            get
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.ServiceDoRegister] == null)
                    return false;
                else
                    return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.ServiceDoRegister]);
            }
        }

        /// <summary>
        /// Gets the service help page.
        /// </summary>
        /// <value>The service help page.</value>
        public static string ServiceHelpPage 
        {
            get 
            {
                return ConfigurationManager.AppSettings[AppSettingsKey.ServiceHelpUri] 
                    ?? "Not Specified.";
            }
        }


        /// <summary>
        /// Gets a value indicating whether [log composition].
        /// </summary>
        /// <value><c>true</c> if [log composition]; otherwise, <c>false</c>.</value>
        public static bool LogComposition 
        {
            get 
            {
                return (ConfigurationManager.AppSettings[AppSettingsKey.LogComposition] != null)
                    && Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.LogComposition]);
            }
        }

        public static double CacheSyncInterval
        {
            get
            {
                return (ConfigurationManager.AppSettings[AppSettingsKey.CacheSyncInterval]==null) 
                    ? 5 
                    : Convert.ToDouble(ConfigurationManager.AppSettings[AppSettingsKey.CacheSyncInterval]);
            }
        }


        public static bool IsRestful 
        {
            get 
            {
                if (ConfigurationManager.AppSettings[AppSettingsKey.IsRestful] == null)
                    return false;
                return Convert.ToBoolean(ConfigurationManager.AppSettings[AppSettingsKey.IsRestful]);

            }
        }

        public static string SmtpHost 
        {
            get
            {
                if(String.IsNullOrWhiteSpace(ConfigurationManager.AppSettings[AppSettingsKey.SmtpHost]))
                    return "smtp.telenet.be";
                return Convert.ToString(ConfigurationManager.AppSettings[AppSettingsKey.SmtpHost]);
            } 
        }
    }
}
