﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.ServiceModel.Description;
using System.Web;
using Wulka.Core;
using Wulka.Domain;
using Wulka.Domain.Authentication;

namespace Wulka.Networking.Wcf
{

    // This Class is Experimental, it will wait before a Session is created.


    /// <summary>
    /// Inherit from DiscoProxy if you need a client that will auto-discover 
    /// its corresponding service from the ESB.
    /// It is also no longer necessary to close the client after every call.
    /// The AutoGenerated Proxy will reuse its channel even if it is faulted, 
    /// and it will unwrap exceptions so there wont be any 
    /// </summary>
    /// <typeparam name="TInterface">The type of the interface.</typeparam>
    public abstract class DiscoProxy2<TInterface> : ProxyBase<TInterface>
        where TInterface : class
    {



        private readonly Dictionary<string, string> _context;

        public ClientCredentials Credentials { get; set; }


        private TInterface _client;
        protected TInterface Client
        {
            get
            {
                if ( (_client == null) && NeedsSyncClient )
                    _client = CreateSyncClient();
                return _client;
            }
            set
            {
                _client = value;
            }
        }
        

        public DiscoProxy2()
        {
            _context = new Dictionary<string, string>();
            //if ((_client == null) && NeedsSyncClient)
            //    _client = CreateSyncClient();
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


        #region IAutoDiscoveringAgent Members
        /// <summary>
        /// Gets the type of the contract.
        /// </summary>
        /// <returns></returns>
        protected virtual string GetContractType()
        {
            return String.Format("{0}:{1}", ServiceConst.Namespace, typeof(TInterface).Name);
        }


        /// <summary>
        /// Creates the specific client.
        /// </summary>
        /// <returns></returns>
        protected void ScanForClient()
        {
            IsScanning = true;
            DiscoCache2.Instance.RefreshCacheAsync(GetContractType(), () =>
                {
                    Client = GetReusableFaultUnwrappingInstance(DiscoCache2.Instance.GetBinding(),
                        DiscoCache2.Instance.GetEndPointAdress(),
                        WulkaCredentials.Current);
                    IsScanning = false;
                });
        }


        private bool _isScanning;
        protected bool IsScanning
        {
            get
            {
                lock (o)
                {
                    return _isScanning;
                }
            }
            set 
            {
                lock (o)
                {
                    _isScanning = value;
                }
            }
        }

        object o = new object();




        protected TInterface CreateSyncClient()
        {
            DiscoCache2.Instance.RefreshCache(GetContractType());
            return GetReusableFaultUnwrappingInstance(DiscoCache2.Instance.GetBinding(),
                        DiscoCache2.Instance.GetEndPointAdress(),
                        WulkaCredentials.Current);
        }

        /// <summary>
        /// Waits for valid client.
        /// </summary>
        /// <param name="act">The act.</param>
        protected void WithValidClient(Action act)
        {
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) =>
                {
                    while (Client == null) { if (!IsScanning) ScanForClient(); };
                };
                wrk.RunWorkerCompleted += (s, e) => { wrk.Dispose(); if (act != null) act(); };
                wrk.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        public override void Dispose()
        {
            CloseClient(Client);
            base.Dispose();
        }

        #endregion

        public bool NeedsSyncClient
        {
            get
            {
                return (HttpContext.Current != null)  || 
                    Convert.ToBoolean(ConfigurationManager.AppSettings[ClientAppSettingsKey.NeedsSyncClient]);
            }
        }
    }

}
