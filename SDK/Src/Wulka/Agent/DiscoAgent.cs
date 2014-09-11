// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-20-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 12-25-2013
// ***********************************************************************
// <copyright file="DiscoAgent.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel;
using Wulka.Domain;
using Wulka.Interfaces;
using Wulka.Networking.Wcf;

namespace Wulka.Agent
{
    /// <summary>
    /// Class DiscoAgent.
    /// </summary>
    class DiscoAgent : DiscoProxy<IDisco>,  IDiscoAgent
    {
        public DiscoAgent(string discoUrl) : base(discoUrl)
        {
        }

        /// <summary>
        /// Gets the endpoint discovery metadata.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <returns>SerializableEndpoint[][].</returns>
        public  SerializableEndpoint[] GetEndpoints(string contractType)
        {
            //var clt = WCFClientProxy<IDisco>
            //    .GetReusableFaultUnwrappingInstance(ConfigurationManager.AppSettings[DiscoAppSettingsKey.DiscoEndpoint]);
            var clt = CreateClient();
            //var clt = DiscoHelper.CreateReusableFaultUnwrappingDiscoClient();
            using ((IDisposable)clt)
            {
                try
                {
                    return clt.GetEndpoints(contractType);
                }
                finally 
                {
                    CloseClient(clt);
                }
            }
        }

        /// <summary>
        /// Gets all endpoints.
        /// </summary>
        /// <returns>SerializableEndpoint[][].</returns>
        public SerializableEndpoint[] GetAllEndpoints()
        {
            //var clt = WCFClientProxy<IDisco>
            //    .GetReusableFaultUnwrappingInstance(ConfigurationManager.AppSettings[DiscoAppSettingsKey.DiscoEndpoint]);
            var clt = CreateClient();
            //var clt = DiscoHelper.CreateReusableFaultUnwrappingDiscoClient();
            using ((IDisposable)clt)
            {
                try
                {
                    var res = clt.GetAllEndpoints();
                    return res;
                }
                finally
                {
                    CloseClient(clt);
                }
                
            }
        }


      






        #region IDiscoAgent Members


        /// <summary>
        /// Gets the endpoints async.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        public void GetEndpointsAsync(string contractType)
        {
            SerializableEndpoint[] res = null;
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) =>
                {
                    res = GetEndpoints(contractType);
                };
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    if (GetEndpointsCompleted != null)
                        GetEndpointsCompleted(res);
                };
                wrk.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Gets all endpoints async.
        /// </summary>
        /// <param name="callback">The callback.</param>
        public void GetAllEndpointsAsync(Action<SerializableEndpoint[]> callback=null)
        {
            SerializableEndpoint[] res = null;
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) =>
                {
                    res = GetAllEndpoints();
                };
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    if (callback != null)
                        callback(res);
                    if (GetAllEndpointsCompleted != null)
                        GetAllEndpointsCompleted(res);
                };
                wrk.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Occurs when [get endpoints completed].
        /// </summary>
        public event Action<SerializableEndpoint[]> GetEndpointsCompleted;
        /// <summary>
        /// Occurs when [get all endpoints completed].
        /// </summary>
        public event Action<SerializableEndpoint[]> GetAllEndpointsCompleted;

        #endregion


        #region IDiscoAgent Members


        /// <summary>
        /// Occurs when [get all endpoint addresses completed].
        /// </summary>
        public event Action<DiscoItem[]> GetAllEndpointAddressesCompleted;

        /// <summary>
        /// Gets all endpoint addresses asynchronous.
        /// </summary>
        /// <param name="getAllEndpointAddressesCompleted">The get all endpoint addresses completed.</param>
        public void GetAllEndpointAddressesAsync(Action<DiscoItem[]> getAllEndpointAddressesCompleted=null)
        {
            DiscoItem[] result = null;
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) => { result = GetAllEndpointAddresses(); };
                wrk.RunWorkerCompleted += (s, e) => { if (GetAllEndpointAddressesCompleted != null) GetAllEndpointAddressesCompleted(result); };
                wrk.RunWorkerAsync();
            }
        }

        #endregion

        #region IDiscoService Members


        /// <summary>
        /// Gets all endpoint addresses.
        /// </summary>
        /// <returns>DiscoItem[][].</returns>
        public DiscoItem[] GetAllEndpointAddresses()
        {
            //var clt = WCFClientProxy<IDisco>
            //    .GetReusableFaultUnwrappingInstance(ConfigurationManager.AppSettings[DiscoAppSettingsKey.DiscoEndpoint]);
            var clt = CreateClient();
           // var clt = DiscoHelper.CreateReusableFaultUnwrappingDiscoClient();
            using ((IDisposable)clt)
            {
                try
                {
                    return clt.GetAllEndpointAddresses();
                }
                finally
                {
                    CloseClient(clt);
                }
            }
        }


        /// <summary>
        /// Gets the contract namespace.
        /// </summary>
        /// <returns>System.String.</returns>
        protected override string GetContractNamespace()
        {
            return DiscoServiceConst.Namespace;
        }

        #endregion

    }
}
