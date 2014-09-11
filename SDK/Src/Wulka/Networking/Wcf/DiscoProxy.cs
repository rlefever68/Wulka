﻿// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-13-2014
// ***********************************************************************
// <copyright file="DiscoProxy.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using NLog;
using Wulka.Core;
using Wulka.Domain;
using Wulka.Domain.Authentication;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// It is also no longer necessary to close the client after every call.
    /// The AutoGenerated Proxy will reuse its channel even if it is faulted,
    /// and it will unwrap exceptions so there wont be any.
    /// </summary>
    /// <typeparam name="T">The type of the interface.</typeparam>
    public abstract class DiscoProxy<T> : ProxyBase<T> where T : class
    {
        /// <summary>
        /// The pool list
        /// </summary>
        private static readonly Queue<Tuple<T, DateTime>> PoolList = new Queue<Tuple<T, DateTime>>();
        /// <summary>
        /// The _context
        /// </summary>
        private readonly Dictionary<string, string> _context;
        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        public ClientCredentials Credentials { get; set; }
        /// <summary>
        /// The _client
        /// </summary>
        private T _client;
        /// <summary>
        /// The logger
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly string _discoUrl;


        /// <summary>
        /// Initializes a new instance of the <see cref="DiscoProxy{T}"/> class.
        /// </summary>
        protected DiscoProxy(string discoUrl=null)
        {
            _context = new Dictionary<string, string>();
            _discoUrl = discoUrl;
        }

        

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        protected T Client
        {
            get { return _client ?? (_client = CreateClient()); }
        }

        /// <summary>
        /// Adds to context.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void AddToContext(string name, string value)
        {
            if (WulkaContext.Current != null)
            {
                WulkaContext.Current.Add(name, value);
            }
            else
            {
                _context.Add(name, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="System.String" /> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
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
            {
                WulkaContext.Current.Remove(name);
            }
            else
            {
                _context.Remove(name);
            }
        }

        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetContractType()
        {
            return String.Format("{0}:{1}", GetContractNamespace(), typeof(T).Name);
        }

        /// <summary>
        /// Gets the namespace of the contract.
        /// </summary>
        /// <returns>System.String.</returns>
        protected virtual string GetContractNamespace()
        {
            return ServiceConst.Namespace;
        }

        /// <summary>
        /// Gets an available instance of the specific client.
        /// </summary>
        /// <returns>T.</returns>
        protected T CreateClient()
        {
            try
            {
                T instance = null;
                lock (PoolList)
                {
                    bool valid = false;
                    while (!valid)
                    {
                        if (PoolList.Count > 0)
                        {
                            var queueItem = PoolList.Dequeue();

                            valid = queueItem.Item2.CompareTo(DateTime.Now.AddSeconds(-270)) > 0 && GetClt(queueItem.Item1).State == CommunicationState.Opened;
                            if (valid)
                                instance = queueItem.Item1;
                            else
                                DisposeClient(GetClt(queueItem.Item1));
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                if (instance != null)
                {
                    foreach (var key in WulkaContext.Current.Keys)
                    {
                        GetClt(instance).InnerChannel.AddToContext(key, WulkaContext.Current[key]);
                    }
                }
                else
                {
                    var contractType = GetContractType();
                    var discoC = DiscoCache.CreateDiscoCache( _discoUrl );
                    discoC.ContractType = contractType;
                    var binding = discoC.GetBinding();
                    var endpointAddress = discoC.GetEndPointAdress();
                    instance = CreateClientInternal(binding, endpointAddress, WulkaCredentials.Current);
                }

                return instance;
            }
            catch (Exception e)
            {
                var msg = String.Format("Error Creating Client for {0}: {1}", GetContractType(), e);
                var ex = new DiscoProxyException(msg, e);
                Logger.Error(msg);
                Logger.Error(ex.GetCombinedMessages());
                throw ex;
            }
        }


        /// <summary>
        /// Creates the client internal.
        /// </summary>
        /// <param name="binding">The binding.</param>
        /// <param name="endpointAddress">The endpoint address.</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>T.</returns>
        protected virtual T CreateClientInternal(Binding binding, EndpointAddress endpointAddress, CredentialsBase credentials)
        {
            ProxyConnectionPool.EnableConnectionPool = false;
            return GetReusableFaultUnwrappingInstance(binding, endpointAddress, credentials);
        }

        /// <summary>
        /// Closes the client.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected override void CloseClient(T instance)
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
                    DisposeClient(client);
                }
            }
        }

        /// <summary>
        /// Closes the client internal.
        /// </summary>
        /// <param name="instance">The instance.</param>
        protected override void CloseClientInternal(T instance)
        {
            lock (PoolList)
            {
                if (PoolList.Count <= 10 && instance != null)
                    PoolList.Enqueue(new Tuple<T, DateTime>(instance, DateTime.Now));
                else
                {
                    DisposeClient(GetClt(instance));
                }
            }
        }

        /// <summary>
        /// Disposes the client.
        /// </summary>
        /// <param name="client">The client.</param>
        private void DisposeClient(Interfaces.IClientBase client)
        {
            try
            {
                if (client == null) return;
                if (client.InnerChannel != null)
                {
                    client.InnerChannel.Close();
                    client.InnerChannel.Dispose();
                }
                client.Abort();
                client.Dispose();
            }
            catch (Exception e)
            {
                Logger.Error("Error Dispose Client for {0}", GetContractType());
                Logger.Error(e.GetCombinedMessages);
            }
        }

        /// <summary>
        /// Releases the unmanaged resources used by the Wulka.Networking.Wcf.DiscoProxy and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CloseClient(Client);
            }

            base.Dispose(disposing);
        }
    }
}