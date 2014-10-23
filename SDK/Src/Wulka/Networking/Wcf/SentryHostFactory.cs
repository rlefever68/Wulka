// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-11-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-15-2013
// ***********************************************************************
// <copyright file="SentryHostFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading;
using NLog;
using Wulka.Configuration;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class SentryHostFactory.
    /// </summary>
    public class SentryHostFactory : ServiceHostFactory
    {




        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Class RetryConst.
        /// </summary>
        private class RetryConst
        {
            /// <summary>
            /// The retry pause
            /// </summary>
            public static readonly int RetryPause = ConfigurationHelper.RetryPause;
            /// <summary>
            /// The retry treshold
            /// </summary>
            public static readonly int RetryTreshold = ConfigurationHelper.RetryTreshold;
        }




        /// <summary>
        /// The _t
        /// </summary>
        private static Type _t;
        /// <summary>
        /// The _host name
        /// </summary>
        private string _hostName;
        /// <summary>
        /// The _sync
        /// </summary>
        readonly object _sync = new object();
        /// <summary>
        /// The _retry counter
        /// </summary>
        private static int _retryCounter = 0;



        /// <summary>
        /// Override servicehost creation.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        /// <returns>ServiceHost.</returns>
        protected override ServiceHost CreateServiceHost(Type t, Uri[] baseAddresses)
        {
            _t = t;
            _hostName = t.ToString();
            return CreateServiceHostInternal(t, baseAddresses); 
        }

        /// <summary>
        /// Creates the service host internal.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        /// <returns>ServiceHost.</returns>
        private ServiceHost CreateServiceHostInternal(Type t, Uri[] baseAddresses)
        {
            var host = new ServiceHost(t, baseAddresses);
            host.Opened += _host_Opened;
            host.Opening += _host_Opening;
            host.Closed += _host_Closed;
            host.MakeAnnouncingService();
            return host;
        }

        public static ServiceHost CreateAnnouncingHost(Type t, Uri[] baseAddresses)
        {
            var host = new ServiceHost(t, baseAddresses);
            host.Opening += (sender, args) => {
                logger.Info("Delaying announcement for {0} seconds.", ConfigurationHelper.AnnounceDelay);
                Thread.Sleep(ConfigurationHelper.AnnounceDelay);
            };
            host.Opened += (sender, args) => {
                ((ServiceHost)sender).PrintHostInfo();
                ((ServiceHost)sender).PrintListeningEndpoints();
                if (ConfigurationHelper.HasRegisterDomainObjects)
                {
                    Retry(RegisterDomainObjects, RetryConst.RetryPause, RetryConst.RetryTreshold);
                }
            };
            host.Closed += (sender, args) => ((ServiceHost)sender).PrintTermination();
            host.MakeAnnouncingService();
            return host;
        }

    

        /// <summary>
        /// Handles the Closed event of the _host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void _host_Closed(object sender, EventArgs e)
        {
            ((ServiceHost)sender).PrintTermination();
        }

        /// <summary>
        /// Occurs before opening the host
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void _host_Opening(object sender, EventArgs e)
        {
            logger.Info("Delaying announcement for {0} seconds.", ConfigurationHelper.AnnounceDelay);
            Thread.Sleep(ConfigurationHelper.AnnounceDelay);
        }

        /// <summary>
        /// Handles the Opened event of the _host control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs" /> instance containing the event data.</param>
        void _host_Opened(object sender, EventArgs e)
        {
            ((ServiceHost)sender).PrintHostInfo();
            ((ServiceHost)sender).PrintListeningEndpoints();
            if (ConfigurationHelper.HasRegisterDomainObjects)
            {
                Retry(RegisterDomainObjects, RetryConst.RetryPause, RetryConst.RetryTreshold);
            }
        }


        /// <summary>
        /// Registers the domain objects async.
        /// </summary>
        private static void RegisterDomainObjects()
        {
            var hst = Activator.CreateInstance(_t);
            try
            {
                if (!(hst is SentryBase)) return;
                ((SentryBase)hst).RegisterDomainObjects();
                Print("Domain object registration => success.");
            }
            catch (Exception ex)
            {
                Error(ex.GetCombinedMessages());
                throw;
            }
            finally
            {
                hst = null;
            }
        }


        /// <summary>
        /// Registers the domain objects async.
        /// </summary>
        private void RegisterDomainObjectsAsync()
        {
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) =>
                {
                    try
                    {
                        RegisterDomainObjects();
                    }
                    catch (Exception ex)
                    {
                        logger.Debug("Error registering domain objects (Async): {0}",ex.GetCombinedMessages());
                        wrk.Dispose();
                        throw;
                    }
                };
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    wrk.Dispose();
                };
                wrk.RunWorkerAsync();
            }
        }



        /// <summary>
        /// Retries the specified act.
        /// </summary>
        /// <param name="act">The act.</param>
        /// <param name="retryInSeconds">The retry in seconds.</param>
        /// <param name="retryTreshold">The retry treshold.</param>
        private static void Retry(Action act, int retryInSeconds, int retryTreshold)
        {
            try
            {
                if ((retryTreshold == 0) || (_retryCounter < retryTreshold))
                {
                    _retryCounter++;
                    Print("** Trying Method {0}...attempt #{1}", act.Method.ToString(), _retryCounter);
                    act();
                }
                else
                {
                    Print("** Retry treshold exceeded for {0}.", act.Method);
                }
            }
            catch 
            {
                Thread.Sleep(TimeSpan.FromSeconds(retryInSeconds));
                Retry(act, retryInSeconds, retryTreshold);
            }
        }



        /// <summary>
        /// Prints the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="vals">The vals.</param>
        private static void Print(string message, params object[] vals)
        {
            logger.Debug(message, vals);
        }


        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="vals">The vals.</param>
        private static void Error(string message, params object[] vals)
        {
            logger.Error(message);
        }

    }
}
