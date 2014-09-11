using System;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NLog;
using Wulka.Domain;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    public class DiscoCache2
    {
        private static volatile DiscoCache2 _instance;
        private static readonly object SyncRoot = new object();
        private SerializableEndpoint[] _cache;


        private DiscoCache2() 
        {
        }


        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static DiscoCache2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                            _instance = CreateDiscoCache();
                        return _instance;
                    }
                }
                return _instance;
            }
        }

        private string _contractType;
        /// <summary>
        /// Creates the discovery cache.
        /// </summary>
        /// <returns></returns>
        private static DiscoCache2 CreateDiscoCache()
        {
            return new DiscoCache2();
        }

        /// <summary>
        /// Refreshes the discovery info.
        /// </summary>
        public void RefreshCache(string cfg, string contractType)
        {
            try
            {
                _contractType = contractType;
                _optimalEndpoint = null;
                var ds = DiscoHelper.CreateReusableDiscoClient(cfg);
                if (ds == null) 
                    throw new Exception("Unable to create Disco Client.");
                _cache = ds.GetEndpoints(contractType);
                if (_cache.Count() == 0)
                {
                    throw new Exception(String.Format("Service [{0}] not available.", contractType));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error refreshing cache {0}: {1}", contractType, ex);
                _logger.Error(ex.GetCombinedMessages());
                throw;
            }
        }


        private static readonly Logger _logger = LogManager.GetLogger("DiscoCache2");


        /// <summary>
        /// Refreshes the cache async.
        /// </summary>
        /// <param name="contractType">Type of the contract.</param>
        /// <param name="callBack">The call back.</param>
        public void RefreshCacheAsync(string cfg, string contractType, Action callBack)
        {
            try
            {
                using (var wrk = new BackgroundWorker())
                {
                    wrk.DoWork += (s, e) => RefreshCache(cfg, contractType);
                    wrk.RunWorkerCompleted += (s, e) => { wrk.Dispose(); if (callBack != null) callBack.Invoke(); };
                    wrk.RunWorkerAsync();
                }
            }
            catch
            {
                RefreshCacheAsync(cfg, contractType, callBack);         
            }
        }


        SerializableEndpoint _optimalEndpoint = null;
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
                            if (IsOnSameMachine(cit)) // Same machine
                            {
                                _optimalEndpoint = cit;
                                break;
                            }
                        }
                        else
                        {
                            if (cit.Binding.Scheme.ToLower().Contains("http"))
                            {
                                _optimalEndpoint = cit;
                            }
                        }
                    }
                }
                if (_optimalEndpoint==null)  
                    throw new Exception(String.Format("No usable endpoint could be found for service [{0}]", _contractType ));
                return _optimalEndpoint;
            }
        }

        /// <summary>
        /// Determines whether the serialized endpoint is on the same machine or not.
        /// </summary>
        /// <param name="ep">the endpoint</param>
        /// <returns>
        /// 	<c>true</c> if [is on same machine] [the specified cit]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsOnSameMachine(SerializableEndpoint ep)
        {
            return (ep.Address.Uri.AbsoluteUri.ToLower().Contains(System.Environment.MachineName.ToLower()));
        }


        /// <summary>
        /// Gets the end point adress.
        /// </summary>
        /// <returns></returns>
        internal EndpointAddress GetEndPointAdress()
        {
            return OptimalEndpoint.Address;
        }

        /// <summary>
        /// Gets the binding.
        /// </summary>
        /// <returns></returns>
        internal Binding GetBinding()
        {
            return OptimalEndpoint.Binding;
        }


        
    }
}
