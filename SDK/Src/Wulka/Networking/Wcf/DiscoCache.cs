// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-20-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 12-29-2013
// ***********************************************************************
// <copyright file="DiscoCache.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NLog;
using Wulka.Configuration;
using Wulka.Domain;
using Wulka.Exceptions;
using Wulka.Logging;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class DiscoCache.
    /// </summary>
    public class DiscoCache
    {
        /// <summary>
        /// The _cache
        /// </summary>
        private SerializableEndpoint[] _cache;


        /// <summary>
        /// Creates the discovery cache.
        /// </summary>
        /// <returns>DiscoCache.</returns>
        public static DiscoCache CreateDiscoCache(string ecoSpaceUrl=null)
        {
            if (String.IsNullOrWhiteSpace(ecoSpaceUrl))
                ecoSpaceUrl = ConfigurationHelper.DiscoEndpoint;
            if (!Caches.ContainsKey(ecoSpaceUrl))
                return Caches[ecoSpaceUrl];
            var c = new DiscoCache() {EcoSpaceUrl=ecoSpaceUrl};
            Caches.Add(ecoSpaceUrl, c);
            return c;
        }


        public static Dictionary<string,DiscoCache> Caches = new Dictionary<string,DiscoCache>();


        /// <summary>
        /// Refreshes the discovery info.
        /// </summary>
        /// <exception cref="System.Exception">
        /// Unable to create Disco Client.
        /// or
        /// </exception>
        public void RefreshCache(string cfg=null)
        {
            try
            {
                if (String.IsNullOrEmpty(cfg))
                    cfg = EcoSpaceUrl;
                _optimalEndpoint = null;
                // var ds = DiscoHelper.CreateReusableDiscoClient();
                var ds = DiscoHelper.CreateReusableFaultUnwrappingDiscoClient(cfg);
                if (ds == null)
                    throw new Exception("Unable to create Disco Client.");
                _cache = ds.GetEndpoints(ContractType);
                if (!_cache.Any())
                {
                    throw new Exception(String.Format("Could not find a service called [{0}].", ContractType));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.GetCombinedMessages());
                throw;
            }
        }

        private readonly Logger _logger = LogManager.GetLogger("DiscoCache");


        /// <summary>
        /// The _optimal endpoint
        /// </summary>
        SerializableEndpoint _optimalEndpoint = null;
        /// <summary>
        /// Gets the optimal endpoint.
        /// </summary>
        /// <value>The optimal endpoint.</value>
        /// <exception cref="System.Exception"></exception>
        private SerializableEndpoint OptimalEndpoint
        {
            get
            {
                if (_optimalEndpoint != null)
                    return _optimalEndpoint;
                if (_cache == null)
                {
                    _optimalEndpoint = null;
                }
                else
                {
                    foreach (var cit in _cache)
                    {
                        if ((cit.Binding is NetNamedPipeBinding))
                        {
                            if (!IsOnSameMachine(cit)) continue;
                            _optimalEndpoint = cit;
                            break;
                        } 
                        if (cit.Binding.Scheme == Uri.UriSchemeHttps)
                        {
                            if (ConfigurationHelper.AllowSSL)
                            {
                                _optimalEndpoint = cit;
                            }
                        }
                        if (cit.Binding.Scheme == Uri.UriSchemeHttp)
                        {
                            if(_optimalEndpoint==null)
                                _optimalEndpoint = cit;
                        }
                    }
                }
                if (_optimalEndpoint == null)
                    throw new Exception(String.Format("No usable endpoint could be found for service [{0}]", _contractType));
                FxLog<DiscoCache>.DebugFormat("[{0}] picked as optimal endpoint.",_optimalEndpoint.Address);
                return _optimalEndpoint;
            }
        }

        /// <summary>
        /// Determines whether [is on same machine] [the specified cit].
        /// </summary>
        /// <param name="cit">The cit.</param>
        /// <returns><c>true</c> if [is on same machine] [the specified cit]; otherwise, <c>false</c>.</returns>
        private bool IsOnSameMachine(SerializableEndpoint cit)
        {
            var localIps = NetUtils.GetIPv4AssociatedWithLocalHost().Select(x=>x.ToString());
            string ipIn = NetUtils.GetFirstIp4(cit.Address.Uri.Host);
            var res = localIps.Contains(ipIn);
            return res;
        }


        /// <summary>
        /// Gets the end point adress.
        /// </summary>
        /// <returns>EndpointAddress.</returns>
        internal EndpointAddress GetEndPointAdress()
        {
            return OptimalEndpoint.Address;
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        /// <returns>Binding.</returns>
        internal Binding GetBinding()
        {
            return OptimalEndpoint.Binding;
        }


        /// <summary>
        /// The _contract type
        /// </summary>
        private string _contractType;
        /// <summary>
        /// Gets or sets the type of the contract.
        /// </summary>
        /// <value>The type of the contract.</value>
        public string ContractType 
        {
            get { return _contractType; }
            set
            {
                _contractType = value;
                RefreshCache(EcoSpaceUrl);
            }
                 
        }




        public string EcoSpaceUrl
        { get; set; }







    }
}
