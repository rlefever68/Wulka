using System;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Wulka.Configuration;
using Wulka.Core;
using Wulka.Networking.Wcf;

namespace Wulka.Agent
{
    public abstract class AgentBase<TClient, TChannel> : IDisposable
        where TClient : ClientBase<TChannel>
        where TChannel : class
    {

        public string Id { get; set; }

        private readonly Dictionary<string, string> _context;

        public ClientCredentials Credentials { get; set; }


        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <returns></returns>
        protected virtual TClient CreateClient()
        {
            return CreateSpecificClient();
        }

        /// <summary>
        /// Creates the specific client.
        /// </summary>
        /// <returns></returns>
        protected virtual TClient CreateSpecificClient()
        {
            return (TClient) Activator.CreateInstance(typeof (TClient));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentBase&lt;TClient, TChannel&gt;"/> class.
        /// </summary>
        protected AgentBase()
        {
            _context = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentBase&lt;TClient, TChannel&gt;"/> class.
        /// </summary>
        /// <param name="credentials">The credentials.</param>
        protected AgentBase(ClientCredentials credentials)
        {
            _context = new Dictionary<string, string>();
            Credentials = credentials;
        }

        /// <summary>
        /// Adds to context.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddToContext(string name, string value)
        {
            if (WulkaContext.Current != null)
                WulkaContext.Current.Add(name, value);
            else
                _context.Add(name, value);

        }

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified name.
        /// </summary>
        /// <value></value>
        public string this[string name]
        {
            get
            {
                return WulkaContext.Current != null ? WulkaContext.Current[name] : _context[name];
            }
        }

        /// <summary>
        /// Removes from context.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RemoveFromContext(string name)
        {
            if (WulkaContext.Current != null)
                WulkaContext.Current.Remove(name);
            else
                _context.Remove(name);
        }

        /// <summary>
        /// Closes the client
        /// </summary>
        /// <param name="client"></param>
        protected void CloseClient(TClient client)
        {
            if (client.State == CommunicationState.Opened)
            {
                client.Close();
            }
            else
            {
                client.Abort();
            }
        }

        /// <summary>
        /// Creates a client for accessing the service specified by the service URL
        /// with the given binding.
        /// </summary>
        /// <param name="serverName">Logical name of the server. This name is used to retrieve configuration information.</param>
        /// <param name="serviceUrlAppSettingsKey">The key in the appSettings section of the configuration file where the service URL is located.</param>
        /// <returns></returns>
        public virtual TClient CreateClient(string serverName, string serviceUrlAppSettingsKey)
        {
            Binding binding = BindingFactory.CreateBindingFromConfiguration(serverName);
            return CreateClient(binding, serviceUrlAppSettingsKey, Credentials);
        }

        /// <summary>
        /// Creates a client for accessing the service specified by the service URL
        /// with the given binding.
        /// </summary>
        /// <param name="binding">The binding to be used to access the service.</param>
        /// <param name="serviceUrlAppSettingsKey">The key in the appSettings section of the configuration file where the service URL is located.</param>
        /// <returns></returns>
        public virtual TClient CreateClient(Binding binding, string serviceUrlAppSettingsKey)
        {
            return CreateClient(binding, serviceUrlAppSettingsKey, Credentials);
        }
        
        /// <summary>
        /// Creates a client for accessing the service specified by the service URL
        /// with the given binding and given credentials.
        /// </summary>
        /// <param name="binding">The binding to be used to access the service.</param>
        /// <param name="serviceUrlAppSettingsKey">The key in the appSettings section of the configuration file where the service URL is located.</param>
        /// <param name="credentials">The credentials to be used to access the service.</param>
        /// <returns></returns>
        public virtual TClient CreateClient(Binding binding, string serviceUrlAppSettingsKey, ClientCredentials credentials)
        {
            string serviceUrl = GetServiceUrl(serviceUrlAppSettingsKey);
            return CreateClient(binding, new EndpointAddress(serviceUrl), credentials);
        }

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        public virtual TClient CreateClient(Binding binding, EndpointAddress address)
        {
            return CreateClient(binding, address, Credentials);
        }

        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns></returns>
        public virtual TClient CreateClient(Binding binding, EndpointAddress address, ClientCredentials credentials)
        {
            if(ConfigurationHelper.AcceptAllCertificates)
                CertificateHelper.SetCertificatePolicy();
            var instance = Activator.CreateInstance(typeof(TClient), binding, address) as TClient;
            if (credentials != null && instance != null)
            {
                //switch (credentials.CredentialType)
                //{
                //    case WulkaAType.Wulkaa:
                //        var WulkaaCred = credentials as WulkaACredentials;
                //        instance.Endpoint.Behaviors.Add(new WulkaAEndpointBehavior(new WulkaAMessageInspector(WulkaaCred.UserName, WulkaaCred.Password)));
                //        break;
                //    case WulkaAType.Wulkaas:
                //        var WulkaasCred = credentials as WulkaASCredentials;
                //        instance.Endpoint.Behaviors.Add(new WulkaAEndpointBehavior(new WulkaASMessageInspector(WulkaasCred.UserName, WulkaasCred.FirstName, WulkaasCred.LastName, WulkaasCred.Session, WulkaasCred.ServiceCode)));
                //        break;
                //}

            }
            if (instance != null)
            {
                instance.Endpoint.Behaviors.Add(new ContextEndpointBehavior());
              
                foreach (var operation in instance.Endpoint.Contract.Operations)
                {
                    var dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dcsob == null)
                    {
                        operation.Behaviors.Add(new DataContractSerializerOperationBehavior(operation) {MaxItemsInObjectGraph = 2147483647});
                    }
                    else
                    {
                        dcsob.MaxItemsInObjectGraph = 2147483647;
                    }
                }
              
                if (WulkaContext.Current != null)
                {
                    foreach (var key in WulkaContext.Current.Keys)
                    {
                        instance.InnerChannel.AddToContext(key, WulkaContext.Current[key]);
                    }
                }
                else
                {
                    foreach (var key in _context.Keys)
                    {
                        instance.InnerChannel.AddToContext(key, _context[key]);
                    }
                } 
            }
            return instance;
        }

        /// <summary>
        /// Gets the service URL.
        /// </summary>
        /// <param name="serviceUrlAppSettingsKey">The service URL app settings key.</param>
        /// <returns></returns>
        protected virtual string GetServiceUrl(string serviceUrlAppSettingsKey)
        {
            string serviceUrl = ConfigurationManager.AppSettings[serviceUrlAppSettingsKey];
            if (string.IsNullOrEmpty(serviceUrl))
            {
                string errorMessage = string.Format(
                    "No service URL found in configuration file to specify service endpoint address"
                    + ", please provide a key '{0}' in the section appSettings and set its value to the appropriate service URL.",
                    serviceUrlAppSettingsKey);
                throw new ApplicationException(errorMessage);
            }
            return serviceUrl;
        }

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            WulkaContext.Current = null;
        }

        #endregion
               
        
    }
}
