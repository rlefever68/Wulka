using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.MsmqIntegration;
using NLog;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Helper class to create WCF bindings
    /// </summary>
    public static class BindingFactory
    {



        private static Logger Logger
        {
            get { return LogManager.GetCurrentClassLogger(); }
        }

        public class Key
        {
            public const string BasicHttpBindingNoSecurity = "BasicHttpBindingNoSecurity";
            public const string BasicHttpBindingMessageSecurity = "BasicHttpBindingMessageSecurity";
            public const string BasicHttpBindingTransportSecurity = "BasicHttpBindingTransportSecurity";
            public const string BasicHttpBindingTransportOnlySecurity = "BasicHttpBindingTransportOnlySecurity";
            public const string BasicHttpBindingTransportWithMessageSecurity = "BasicHttpBindingTransportWithMessageSecurity";



            public const string BasicWinHttpBinding = "BasicWinHttpBinding";
            public const string WsHttpBindingNoSecurity = "WsHttpBindingNoSecurity";
            public const string WsHttpBindingMessageSecurity = "WsHttpBindingMessageSecurity";
            public const string WsHttpBindingTransportSecurity = "WsHttpBindingTransportSecurity";
            public const string WsHttpBindingTransportWithMessageSecurity = "WsHttpBindingTransportWithMessageSecurity";
            public const string WsWinHttpBinding = "WsWinHttpBinding";
            public const string Ws2007HttpBinding = "Ws2007HttpBinding";
            public const string WsDualHttpBinding = "WsDualHttpBinding";
            public const string WsFederationHttpBinding = "WsFederationHttpBinding";
            public const string Ws2007FederationHttpBinding = "Ws2007FederationHttpBinding";
            public const string NetTcpBinding = "NetTcpBinding";
            public const string NetNamedPipeBinding = "NetNamedPipeBinding";
            public const string NetMsmqBinding = "NetMsmqBinding";
            public const string NetPeerTcpBinding = "NetPeerTcpBinding";
            public const string WebHttpBinding = "WebHttpBinding";
            public const string MsmqIntegrationBinding = "MsmqIntegrationBinding";


            public const string UnsecureNetMsmqBinding = "UnsecureNetMsmqBinding";

            public const string CustomValidationBasicHttpBindingTransportAndMessage = "CustomValidationBasicHttpBindingTransportAndMessage";
            public const string CustomValidationBasicHttpBindingMessage = "CustomValidationBasicHttpBindingMessage";

            public const string CustomValidationWsHttpBindingMessage = "CustomValidationWsHttpBindingMessage";
            public const string CustomValidationWsHttpBindingTransportAndMessage = "CustomValidationWsHttpBindingTransportAndMessage";
        }

        public static IEnumerable<string> KnownBindings
        {
            get
            {
                return new List<string>
                          {
                              Key.BasicHttpBindingNoSecurity, 
                              Key.BasicHttpBindingMessageSecurity,
                              Key.BasicHttpBindingTransportSecurity,
                              Key.BasicHttpBindingTransportOnlySecurity,
                              Key.BasicHttpBindingTransportWithMessageSecurity,
                              Key.WsHttpBindingNoSecurity,
                              Key.WsHttpBindingMessageSecurity,
                              Key.WsHttpBindingTransportSecurity,
                              Key.WsHttpBindingTransportWithMessageSecurity,
                              Key.WsWinHttpBinding,
                              Key.WsFederationHttpBinding,
                              Key.WsDualHttpBinding,
                              Key.Ws2007HttpBinding,
                              Key.Ws2007FederationHttpBinding,
                              Key.WebHttpBinding,
                              Key.NetTcpBinding,
                              Key.NetPeerTcpBinding,
                              Key.NetNamedPipeBinding,
                              Key.NetMsmqBinding,
                              Key.MsmqIntegrationBinding,
                              Key.CustomValidationWsHttpBindingTransportAndMessage,
                              Key.CustomValidationWsHttpBindingMessage,
                              Key.CustomValidationBasicHttpBindingTransportAndMessage,
                              Key.CustomValidationBasicHttpBindingMessage,
                              Key.UnsecureNetMsmqBinding
                          };
            }
        }


        /// <summary>
        /// Creates a binding for the specified hostName (an application or a repository).
        /// If no key with name '[hostName]BindingInfo' is provided in the AppSettings, a default
        /// binding of type WSHttpBinding is created and returned.
        /// If the key '[hostName]BindingInfo' is provided, its value must be formatted as
        /// '[BindingTypeName];[BindingConfigurationName]'.
        /// BindingTypeName must be an assembly-qualified name of a binding type.  An assembly-qualified name
        /// of a type consists of the type name, including its namespace, followed by a comma, followed by the
        ///  display name of the assembly.
        /// BindingConfigurationName must refer to the name of a binding in the section 
        /// system.serviceModel/Bindings in the config file
        /// </summary>
        /// <param name="hostName">A logical name associated with the service host (an application, a repository, etc.).</param>
        /// <returns></returns>
        public static Binding CreateBindingFromConfiguration(string hostName)
        {
            Binding binding;
            string keyName = hostName + "BindingInfo";

            string bindingTypeName;
            string bindingConfigurationName;
            if (GetBindingInfo(hostName, keyName, out bindingTypeName, out bindingConfigurationName))
            {
                try
                {
                    Type bindingType = Type.GetType(bindingTypeName, true /* throw error if not found */);
                    binding = (Binding)Activator.CreateInstance(bindingType, new object[] { bindingConfigurationName });
                }
                catch (Exception ex)
                {
                    string errorMessage = string.Format(
                        "Failed to create service binding from binding info in configuration file for service host {0} (AppSettings key {1})"
                        + ", bindingTypeName = {2}, bindingConfigurationName = {3}",
                        hostName, keyName, bindingTypeName, bindingConfigurationName);
                    throw new ApplicationException(errorMessage, ex);
                }
                Logger.Debug("Created service binding for service host {0} (AppSettings key {1})"
                    + ", bindingTypeName = {2}, bindingConfigurationName = {3}",
                    hostName, keyName, bindingTypeName, bindingConfigurationName);
            }
            else // Return default binding
            {
                binding = new WSHttpBinding { TransactionFlow = true, MaxReceivedMessageSize = 2147483647 };
            }
            return binding;
        }

        /// <summary>
        /// Gets the binding info.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="keyName">Name of the key.</param>
        /// <param name="bindingTypeName">Name of the binding type.</param>
        /// <param name="bindingConfigurationName">Name of the binding configuration.</param>
        /// <returns></returns>
        private static bool GetBindingInfo(string hostName, string keyName,
            out string bindingTypeName, out string bindingConfigurationName)
        {
            bindingTypeName = null;
            bindingConfigurationName = null;

            string bindingInfo = ConfigurationManager.AppSettings[keyName];
            if (string.IsNullOrEmpty(bindingInfo))
            {
                string message = string.Format(
                    "No service binding info found in configuration file to specify binding for service host {0} (AppSettings key {1})"
                    + ", default binding will be used",
                    hostName, keyName);
                Logger.Debug(message);
                return false;
            }
            string[] bindingInfos = bindingInfo.Split(new[] { ';' });
            if (bindingInfos.Length != 2)
            {
                string errorMessage = string.Format(
                    "Unexpected service binding info found in configuration file to specify service binding for service host {0} (AppSettings key {1} = '{2}')"
                    + ", please provide a value with format '<BindingTypeName>;<BindingConfigurationName>'"
                    + " where <BindingTypeName> is an assembly qualified name formatted as <FullTypeName>[,<AssemblyDisplayName>]"
                    + " and <BindingConfigurationName> refers to the name of a binding in the section system.serviceModel/Bindings of the config file",
                    hostName, keyName, bindingInfo);
                throw new ApplicationException(errorMessage);
            }
            bindingTypeName = bindingInfos[0];
            bindingConfigurationName = bindingInfos[1];
            return true;
        }


                


        /// <summary>
        /// Creates the binding from key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static Binding CreateBindingFromKey(string key)
        {
            switch (key)
            {
                case Key.BasicWinHttpBinding:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                        bnd.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                        bnd.Upscale();
                        return bnd;
                    };


                case Key.WsWinHttpBinding:
                    {
                        var bnd = new WSHttpBinding(SecurityMode.Transport);
                        bnd.Security.Transport.ClientCredentialType = HttpClientCredentialType.Windows;
                        bnd.Upscale();
                        return bnd;
                    }


                case Key.BasicHttpBindingNoSecurity:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.None);
                        bnd.Upscale();
                        return bnd;
                    }

                case Key.BasicHttpBindingMessageSecurity:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.Message);
                        bnd.Upscale();
                        return bnd;
                    }

                case Key.BasicHttpBindingTransportSecurity:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
                        bnd.Upscale();
                        return bnd;
                    }

                case Key.BasicHttpBindingTransportOnlySecurity:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
                        bnd.Upscale();
                        return bnd;
                    }

                case Key.BasicHttpBindingTransportWithMessageSecurity:
                    {
                        var bnd = new BasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential);
                        bnd.Upscale();
                        return bnd;
                    }


                case Key.MsmqIntegrationBinding:
                    {
                        var bnd = new MsmqIntegrationBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.NetMsmqBinding:
                    {
                        var bnd = new NetMsmqBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.NetNamedPipeBinding:
                    {
                        //     var bnd = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
                        var bnd = new NetNamedPipeBinding();

                        bnd.Upscale();
                        return bnd;
                    }
                case Key.UnsecureNetMsmqBinding:
                    {
                        var bnd = new NetMsmqBinding(NetMsmqSecurityMode.None);
                        bnd.Security.Transport.MsmqAuthenticationMode = MsmqAuthenticationMode.None;
                        bnd.Security.Transport.MsmqProtectionLevel = ProtectionLevel.None;
                        bnd.Security.Message.ClientCredentialType = MessageCredentialType.None;
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.NetTcpBinding:
                    {
                        var bnd = new NetTcpBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.WebHttpBinding:
                    {
                        var bnd = new WebHttpBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.Ws2007FederationHttpBinding:
                    {
                        var bnd = new WS2007FederationHttpBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.Ws2007HttpBinding:
                    {
                        var bnd = new WS2007HttpBinding();
                        bnd.Upscale();
                        return bnd;
                    };
                case Key.WsDualHttpBinding:
                    {
                        var bnd = new WSDualHttpBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.WsFederationHttpBinding:
                    {
                        var bnd = new WSFederationHttpBinding();
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.CustomValidationWsHttpBindingTransportAndMessage:
                    {
                        //http://www.codeproject.com/Articles/59927/WCF-Service-over-HTTPS-with-custom-username-and-pa
                        var bnd = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
                        bnd.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.CustomValidationWsHttpBindingMessage:
                    {
                        //http://www.codeproject.com/Articles/59927/WCF-Service-over-HTTPS-with-custom-username-and-pa
                        var bnd = new WSHttpBinding(SecurityMode.Message);
                        bnd.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.CustomValidationBasicHttpBindingTransportAndMessage:
                {
                    var bnd = new BasicHttpBinding((BasicHttpSecurityMode.TransportWithMessageCredential));
                    bnd.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                    bnd.Upscale();
                    return bnd;
                }
                case Key.CustomValidationBasicHttpBindingMessage:
                {
                    var bnd = new BasicHttpBinding((BasicHttpSecurityMode.Message));
                    bnd.Security.Message.ClientCredentialType = BasicHttpMessageCredentialType.UserName;
                    bnd.Upscale();
                    return bnd;
                }
                case Key.WsHttpBindingMessageSecurity:
                    {
                        var bnd = new WSHttpBinding(SecurityMode.Message);
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.WsHttpBindingTransportSecurity:
                    {
                        var bnd = new WSHttpBinding(SecurityMode.Transport);
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.WsHttpBindingTransportWithMessageSecurity:
                    {
                        var bnd = new WSHttpBinding(SecurityMode.TransportWithMessageCredential);
                        bnd.Upscale();
                        return bnd;
                    }
                case Key.WsHttpBindingNoSecurity:
                    {
                        var bnd = new WSHttpBinding(SecurityMode.None);
                        bnd.Upscale();
                        return bnd;
                    }
                default:
                {
                    var bnd = new BasicHttpBinding(BasicHttpSecurityMode.None);
                    bnd.Upscale();
                    return bnd;

                }

            }
        }
    }
}
