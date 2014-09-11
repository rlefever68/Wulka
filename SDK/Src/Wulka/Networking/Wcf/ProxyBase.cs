// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 02-03-2014
//
// Last Modified By : ON8RL
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="ProxyBase.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using NLog;
using Wulka.Core;
using Wulka.Crypto;
using Wulka.Domain.Authentication;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class ProxyBase.
    /// </summary>
    /// <typeparam name="T">The type of the t interface.</typeparam>
    public abstract class ProxyBase<T> : IDisposable
        where T : class
    {
        /// <summary>
        /// The _reusable fault unwrapping instance
        /// </summary>
        private static Type _reusableFaultUnwrappingInstance;

        private readonly Logger _logger = LogManager.GetLogger(String.Format("Proxy<{0}>", typeof(T).Name));

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyBase{T}"/> class.
        /// </summary>
        protected ProxyBase()
        {
            if(WulkaContext.Current==null)
                WulkaContext.Current = new WulkaContext();
        }

        #region GetInstance


        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <returns>`0.</returns>
        protected T GetInstance(string configName)
        {
            T instance = ProxyConnectionPool.RequestFromPool<T>();
            if (instance != null)
            {
                _logger.Info("Returned pooled ProxyBase Instance {0} from configuration {1}.", instance.GetType(), configName);
                return instance;
            }
            // Build the type
            Type type = ProxyBuilder.BuildType<T, WCFProxyClassBuilder<T>>();
            // Create new instance
            instance = (T)Activator.CreateInstance(type, new object[] { configName });
            ProxyConnectionPool.Register(instance);
            _logger.Info("Created New ProxyBase Instance {0} from configuration {1}", instance.GetType(), configName);
            return instance;
        }
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>TInterface.</returns>
        protected T GetInstance(Binding binding, EndpointAddress endpoint)
        {
            binding.Upscale();
            //FxLog<T>.DebugFormat("Upscaling binding {0}", binding);
            T instance = ProxyConnectionPool.RequestFromPool<T>();
            if (instance != null)
            {
                _logger.Info("Returned pooled ProxyBase Instance {0} from binding {1} on endpoint {2}.", instance.GetType(), binding, endpoint);
                return instance;
            }
            // Build the type
            Type type = ProxyBuilder.BuildType<T, WCFProxyClassBuilder<T>>();
            //FxLog<T>.DebugFormat("Type {0} is build.", type);
            // Create new instance
            instance = (T)Activator.CreateInstance(type, new object[] { binding, endpoint });
            //FxLog<T>.DebugFormat("Instance {0} of type {1} is created.", instance, type);
            ProxyConnectionPool.Register(instance);
            _logger.Info("Created new ProxyBase Instance {0} from binding {1} on endpoint {2}.", instance.GetType(), binding, endpoint);
            return instance;
        }


        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>TInterface.</returns>
        protected T GetInstance(Binding binding, EndpointAddress address, CredentialsBase credentials)
        {
            binding.Upscale(); 
            _logger.Info("Getting instance from binding {0} at address {1}", binding, address);
            var instance = GetInstance(binding, address);

            if (GetClt(instance) != null)
            {
                AddWulkaCredentials(instance, credentials);
                var bhv = GetClt(instance).Endpoint.Behaviors.Find<ContextEndpointBehavior>();
                if (bhv == null)
                    GetClt(instance).Endpoint.Behaviors.Add(new ContextEndpointBehavior());
                foreach (var operation in GetClt(instance).Endpoint.Contract.Operations)
                {
                    var dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dcsob == null)
                    {
                        operation.Behaviors.Add(new DataContractSerializerOperationBehavior(operation) { MaxItemsInObjectGraph = 2147483647 });
                    }
                    else
                    {
                        dcsob.MaxItemsInObjectGraph = 2147483647;
                    }
                }
                foreach (var key in WulkaContext.Current.Keys)
                {
                    GetClt(instance).InnerChannel.AddToContext(key, WulkaContext.Current[key]);
                }
            }
            return instance;
        }
        #endregion

        #region GetReusableInstance
        /// <summary>
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <returns>TInterface.</returns>
        protected static T GetReusableInstance(string configName)
        {
            T instance = ProxyConnectionPool.RequestFromPool<T>();
            if (instance != null)
                return instance;

            // Build the type
            Type type = ProxyBuilder.BuildType<T, WCFReusableProxyClassBuilder<T>>();

            // Create new instance
            instance = (T)Activator.CreateInstance(type, new object[] { configName });
            ProxyConnectionPool.Register(instance);

            return instance;
        }
        /// <summary>
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>TInterface.</returns>
        protected static T GetReusableInstance(Binding binding, EndpointAddress endpoint)
        {
            binding.Upscale();
            T instance = ProxyConnectionPool.RequestFromPool<T>();
            if (instance != null)
                return instance;

            // Build the type
            Type type = ProxyBuilder.BuildType<T, WCFReusableProxyClassBuilder<T>>();

            // Create new instance
            instance = (T)Activator.CreateInstance(type, new object[] { binding, endpoint });

            ProxyConnectionPool.Register(instance);

            return instance;
        }
        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>TInterface.</returns>
        protected T GetReusableInstance(Binding binding, EndpointAddress address, CredentialsBase credentials)
        {
            binding.Upscale();
            var instance = GetReusableInstance(binding, address);
            AddWulkaCredentials(instance, credentials);
            if (GetClt(instance) != null)
            {
                var bhv = GetClt(instance).Endpoint.Behaviors.Find<ContextEndpointBehavior>();
                if (bhv == null)
                    GetClt(instance).Endpoint.Behaviors.Add(new ContextEndpointBehavior());
                foreach (var operation in GetClt(instance).Endpoint.Contract.Operations)
                {
                    var dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                    if (dcsob == null)
                    {
                        operation.Behaviors.Add(new DataContractSerializerOperationBehavior(operation) { MaxItemsInObjectGraph = 2147483647 });
                    }
                    else
                    {
                        dcsob.MaxItemsInObjectGraph = 2147483647;
                    }
                }
                foreach (var key in WulkaContext.Current.Keys)
                {
                    GetClt(instance).InnerChannel.AddToContext(key, WulkaContext.Current[key]);
                }
            }
            return instance;
        }
        #endregion

        #region GetReusableFaultUnwrappingInstance

        /// <summary>
        /// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// The class will also unwrap any FaultException and throw the original Exception.
        /// </summary>
        /// <param name="configName">Name of the configuration.</param>
        /// <returns>TInterface.</returns>
        protected T GetReusableFaultUnwrappingInstance(string configName)
        {
            T instance = ProxyConnectionPool.RequestFromPool<T>();
            if (instance != null)
            {
                _logger.Info("Returned pooled FaultUnWrapping ProxyBase Instance {0} from configuration {1}.", instance.GetType(), configName);
                return instance;
            }

            if (_reusableFaultUnwrappingInstance == null)
            {
                // no need to lock as the ProxyBuilder is thread safe and will always return the same stuff
                // Build the type
                _reusableFaultUnwrappingInstance = ProxyBuilder.BuildType<T, WCFReusableFaultWrapperProxyClassBuilder<T>>();
            }

            // Create new instance
            instance = (T)Activator.CreateInstance(_reusableFaultUnwrappingInstance, new object[] { configName });
            _logger.Info("Returned New FaultUnWrapping ProxyBase Instance {0} from configuration {1}.", instance, configName);
            ProxyConnectionPool.Register(instance);
            return instance;
        }
        /// <summary>
        /// Returns a new instance for a client proxy over the specified interface with the specified config name used for initialization.
        /// This instance of the proxy can be reused as it will always "clean" itself
        /// if the channel is faulted.
        /// The class will also unwrap any FaultException and throw the original Exception.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpoint">The endpoint.</param>
        /// <returns>TInterface.</returns>
        protected static T BuildReusableFaultUnwrappingInstance(Binding binding, EndpointAddress endpoint)
        {
            binding.Upscale();
            //TInterface instance = ProxyConnectionPool.RequestFromPool<TInterface>();
            //if (instance != null)
            //    return instance;

            if (_reusableFaultUnwrappingInstance == null)
            {
                // no need to lock as the ProxyBuilder is thread safe and will always return the same stuff
                // Build the type
                _reusableFaultUnwrappingInstance = ProxyBuilder.BuildType<T, WCFReusableFaultWrapperProxyClassBuilder<T>>();
            }

            // Create new instance
            T instance = (T)Activator.CreateInstance(_reusableFaultUnwrappingInstance, new object[] { binding,endpoint });
            //ProxyConnectionPool.Register(instance);

            return instance;
        }


        /// <summary>
        /// Creates the client.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="address">The address.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>TInterface.</returns>
        protected T GetReusableFaultUnwrappingInstance(Binding binding, EndpointAddress address, CredentialsBase credentials)
        {
            binding.Upscale();
            var instance = BuildReusableFaultUnwrappingInstance(binding, address);
            AddWulkaCredentials(instance, credentials);
            if (GetClt(instance) == null) return instance;
            var bhv = GetClt(instance).Endpoint.Behaviors.Find<ContextEndpointBehavior>();
            if (bhv == null)
                GetClt(instance).Endpoint.Behaviors.Add(new ContextEndpointBehavior());
            foreach (var operation in GetClt(instance).Endpoint.Contract.Operations)
            {
                var dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob == null)
                {
                    operation.Behaviors.Add(new DataContractSerializerOperationBehavior(operation) { MaxItemsInObjectGraph = 2147483647 });
                }
                else
                {
                    dcsob.MaxItemsInObjectGraph = 2147483647;
                }
            }
            foreach (var key in WulkaContext.Current.Keys)
            {
                GetClt(instance).InnerChannel.AddToContext(key, WulkaContext.Current[key]);
                // _logger.Info("Added Context {0}={1}", key, WulkaContext.Current[key]);
            }
            return instance;
        }
        #endregion

        /// <summary>
        /// Adds the Wulka credentials.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="credentials">The credentials.</param>
        private void AddWulkaCredentials(T instance, CredentialsBase credentials)
        {
            if (credentials != null)
            {
                WulkaContext.Current.Clear();
                var ncr = CredentialCache.DefaultCredentials;
                var inst = GetClt(instance);
                if (inst == null)  return;
                var cr = inst.ClientCredentials;
                if(cr!=null)
                    cr.Windows.ClientCredential = ncr.GetCredential(inst.Endpoint.Address.Uri, "Negotiate");
                AddWulkaCredentialsInternal(inst, WulkaContext.Current, credentials);
            }
        }

        /// <summary>
        /// Adds the Wulka credentials internal.
        /// </summary>
        /// <param name="inst">The inst.</param>
        /// <param name="context">The p MS context.</param>
        /// <param name="credentials">The credentials.</param>
        protected virtual void AddWulkaCredentialsInternal(object inst, WulkaContext context, CredentialsBase credentials)
        {
            if (credentials != null)
            {
                switch (credentials.CredentialType)
                {
                    case CredentialsTypeEnum.UserNamePassword:
                        {
                            var cred = credentials as UserNamePasswordCredentials;
                            context.Add(WulkaContextKey.UserName, cred.UserName);
                            context.Add(WulkaContextKey.PasswordEnc, CryptoEngine.Encrypt(cred.Password));
                            GetClt(inst).Endpoint.Behaviors.Add(new UserNameEndpointBehavior(new UserNamePasswordMessageInspector(cred.UserName, cred.Password)));
                            break;
                        }
                    case CredentialsTypeEnum.UserNameSession:
                        {
                            var creds = credentials as UserNameSessionCredentials;
                            context.Add(WulkaContextKey.UserName, creds.UserName);
                            context.Add(WulkaContextKey.SessionId, creds.Session);
                            GetClt(inst).Endpoint.Behaviors.Add(new UserNameEndpointBehavior(new UserNameSessionMessageInspector(creds.UserName, creds.Session)));
                            break;
                        }
                    case CredentialsTypeEnum.Extended:
                        {
                            var crede = credentials as ExtendedCredentials;
                            context.Add(WulkaContextKey.UserName, crede.UserName);
                            context.Add(WulkaContextKey.SessionId, crede.Session);
                            context.Add(WulkaContextKey.ServiceCode, crede.ServiceCode);
                            GetClt(inst).Endpoint.Behaviors.Add(new UserNameEndpointBehavior(new ExtendedMessageInspector(crede.UserName, crede.FirstName, crede.LastName, crede.Session, crede.ServiceCode)));
                            break;
                        }
                }
            }
        }





        /// <summary>
        /// Closes the client.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected virtual void CloseClient(T instance)
        {
            var client = GetClt(instance);
            if (client != null)
            {
                if (client.State == CommunicationState.Opened)
                {
                    CloseClientInternal(instance);
                }
                else
                {
                    client.Abort();
                    client.Dispose();
                }
            }
        }

        /// <summary>
        /// Closes the client internal.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected virtual void CloseClientInternal(T instance)
        {
            var client = GetClt(instance);
            if (client != null)
            {
                if (client.InnerChannel != null)
                {
                    client.InnerChannel.Close();
                    client.InnerChannel.Dispose();
                }

                client.Close();
                client.Dispose();
            }
        }

        /// <summary>
        /// Gets the CLT.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>IClientBase.</returns>
        protected IClientBase GetClt(object instance)
        {
            return (instance as IClientBase);
        }

        #region IDisposable Members
        /// <summary>
        /// Releases the unmanaged resources used by the Wulka.Networking.Wcf.ProxyBase and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            
        }

        /// <summary>
        /// Releases all resources used by the Wulka.Networking.Wcf.ProxyBase.
        /// </summary>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
