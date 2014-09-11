// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 11-30-2013
//
// Last Modified By : ON8RL
// Last Modified On : 11-30-2013
// ***********************************************************************
// <copyright file="BindingHelper.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using Wulka.Domain;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class BindingHelper.
    /// </summary>
    public static class BindingHelper
    {

        /// <summary>
        /// Class BindingConst.
        /// </summary>
        public class BindingConst
        {
            /// <summary>
            /// The maximum received message size
            /// </summary>
            public const int MaxReceivedMessageSize = 2147483647;
            /// <summary>
            /// The receive timeout
            /// </summary>
            public static TimeSpan ReceiveTimeout = TimeSpan.FromMinutes(5);
            /// <summary>
            /// The send timeout
            /// </summary>
            public static TimeSpan SendTimeout = TimeSpan.FromMinutes(5);
            /// <summary>
            /// The has transaction flow
            /// </summary>
            public const bool HasTransactionFlow = true;
        }


        /// <summary>
        /// Gets the maximum size of the message.
        /// </summary>
        /// <value>The maximum size of the message.</value>
        private static int MaxMessageSize
        {
            get
            {
                var s = ConfigurationManager.AppSettings[BindingAppSettingsKey.MaxReceivedMessageSize];
                return String.IsNullOrEmpty(s) ? BindingConst.MaxReceivedMessageSize : Convert.ToInt32(s);
            }
        }


        /// <summary>
        /// Gets a value indicating whether this instance has transaction flow.
        /// </summary>
        /// <value><c>true</c> if this instance has transaction flow; otherwise, <c>false</c>.</value>
        private static bool HasTransactionFlow
        {
            get
            {
                var s = ConfigurationManager.AppSettings[BindingAppSettingsKey.TransactionFlow];
                return String.IsNullOrEmpty(s) ? BindingConst.HasTransactionFlow : Convert.ToBoolean(s);
            }
        }

        /// <summary>
        /// Gets the send timeout.
        /// </summary>
        /// <value>The send timeout.</value>
        private static TimeSpan SendTimeout 
        {
            get
            {
                var s = ConfigurationManager.AppSettings[BindingAppSettingsKey.SendTimeout];
                return String.IsNullOrEmpty(s) ? BindingConst.SendTimeout : TimeSpan.Parse(s);
            }
        }

        /// <summary>
        /// Gets the receive timeout.
        /// </summary>
        /// <value>The receive timeout.</value>
        private static TimeSpan ReceiveTimeout 
        {
            get
            {
                var s = ConfigurationManager.AppSettings[BindingAppSettingsKey.ReceiveTimeout];
                return String.IsNullOrEmpty(s) ? BindingConst.ReceiveTimeout : TimeSpan.Parse(s);
            }
        }


        /// <summary>
        /// Upscales the specified binding.
        /// </summary>
        /// <param name="binding">The binding.</param>
        public static void Upscale(this Binding binding)
        {
            binding.SendTimeout = SendTimeout;
            binding.ReceiveTimeout = ReceiveTimeout;
            if (binding is NetNamedPipeBinding)
            {
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxDepth = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((NetNamedPipeBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((NetNamedPipeBinding)binding).MaxBufferSize = MaxMessageSize;
                ((NetNamedPipeBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((NetNamedPipeBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((NetNamedPipeBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
            if (binding is BasicHttpBinding)
            {
                ((BasicHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((BasicHttpBinding)binding).MaxBufferSize = MaxMessageSize;
                ((BasicHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((BasicHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((BasicHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((BasicHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((BasicHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                return;
            };
            if (binding is WSHttpBinding)
            {
                ((WSHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((WSHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((WSHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((WSHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((WSHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((WSHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((WSHttpBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
            if (binding is NetTcpBinding)
            {
                ((NetTcpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((NetTcpBinding)binding).MaxBufferSize = MaxMessageSize;
                ((NetTcpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((NetTcpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((NetTcpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((NetTcpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((NetTcpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((NetTcpBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
            if (binding is MsmqIntegrationBinding)
            {
                ((MsmqIntegrationBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                return;
            };
            if (binding is NetMsmqBinding)
            {
                ((NetMsmqBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((NetMsmqBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((NetMsmqBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((NetMsmqBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((NetMsmqBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((NetMsmqBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                return;
            };
            if (binding is WebHttpBinding)
            {
                ((WebHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((WebHttpBinding)binding).MaxBufferSize = MaxMessageSize;
                ((WebHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((WebHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((WebHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((WebHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((WebHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                return;
            };
            if (binding is WS2007FederationHttpBinding)
            {
                ((WS2007FederationHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((WS2007FederationHttpBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
            if (binding is WSDualHttpBinding)
            {
                ((WSDualHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((WSDualHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((WSDualHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((WSDualHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((WSDualHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((WSDualHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((WSDualHttpBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
            if (binding is WSFederationHttpBinding)
            {
                ((WSFederationHttpBinding)binding).MaxReceivedMessageSize = MaxMessageSize;
                ((WSFederationHttpBinding)binding).MaxBufferPoolSize = MaxMessageSize;
                ((WSFederationHttpBinding)binding).ReaderQuotas.MaxArrayLength = MaxMessageSize;
                ((WSFederationHttpBinding)binding).ReaderQuotas.MaxBytesPerRead = MaxMessageSize;
                ((WSFederationHttpBinding)binding).ReaderQuotas.MaxStringContentLength = MaxMessageSize;
                ((WSFederationHttpBinding)binding).ReaderQuotas.MaxNameTableCharCount = MaxMessageSize;
                ((WSFederationHttpBinding)binding).TransactionFlow = HasTransactionFlow;
                return;
            };
        }

   
    }
}
