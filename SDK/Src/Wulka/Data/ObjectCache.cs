// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-16-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-06-2014
// ***********************************************************************
// <copyright file="ObjectCache.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Timers;
using NLog;
using Wulka.Configuration;
using Wulka.Domain;
using Wulka.Domain.Interfaces;
using Wulka.Interfaces;

namespace Wulka.Data
{
    /// <summary>
    /// Class ObjectCache.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ObjectCache<T> 
        where T:IDomainObject, new()
    {

        /// <summary>
        /// The _cache
        /// </summary>
        private static Dictionary<string, T> _cache;

        /// <summary>
        /// Gets the cache.
        /// </summary>
        /// <value>The cache.</value>
        internal static Dictionary<string, T> Cache 
        {
            get { return _cache ?? (_cache = new Dictionary<string, T>()); }
        }



        /// <summary>
        /// Gets the worker.
        /// </summary>
        /// <value>The worker.</value>
        private static IProvider<T> Worker
        {
            get { return ProviderWorkerContainer<T>.Instance.Worker; }
        }



        /// <summary>
        /// The _timer
        /// </summary>
        private static Timer _timer;
        /// <summary>
        /// Gets the refresh timer.
        /// </summary>
        /// <value>The refresh timer.</value>
        private static Timer RefreshTimer
        {
            get 
            {
                if (_timer != null) return _timer;
                _timer = new Timer 
                {
                    Interval = ConfigurationHelper.CacheSyncInterval * 1000
                };
                _timer.Elapsed += (sender,args) => SynchronizeCacheAsync(OnCachSynchronized);
                _timer.Enabled = true;
                return _timer;
            }
        }

        /// <summary>
        /// Called when [cach synchronized].
        /// </summary>
        private static void OnCachSynchronized()
        {
           
        }

        /// <summary>
        /// Synchronizes the cache asynchronous.
        /// </summary>
        /// <param name="action">The action.</param>
        private static void SynchronizeCacheAsync(Action action)
        {
            using (var wrk = new BackgroundWorker())
            {
                wrk.DoWork += (s, e) => SynchronizeCache();
                wrk.RunWorkerCompleted += (s, e) =>
                {
                    if (action != null)
                        action();
                    wrk.Dispose();
                };
                wrk.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Synchronizes the cache.
        /// </summary>
        private static void SynchronizeCache()
        {
            lock (Cache)
            {
                Logger.Info("Synchronizing Provider Cache for '{0}'.", typeof(T));
                foreach (var item in Cache.Values)
                {
                    var stored = Worker.GetById(item.Id);
                    if (item.Rev == stored.Rev) continue;
                    Logger.Info("\tUpdating cache for {0} [{1}] - revision [{2}]", typeof(T), stored.Id, stored.Rev);
                    Cache[item.Id] = stored;
                }
                Logger.Info("Provider Cache for {0} synchronized.", typeof(T));
                Logger.Info("Next Synchronization in {0} seconds.", ConfigurationHelper.CacheSyncInterval);
            }
        }



        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger
        {
            get
            {
                return LogManager.GetCurrentClassLogger();
            }
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>`0.</returns>
        public static T Get(string id)
        {
            lock (Cache)
            {
                if (Exists(id)) return Cache[id];
                var it =Worker.GetById(id);
                if (it != null)
                    Cache[id] = it;
                else
                    return default(T);
                return Cache[id];
            }
        }

        /// <summary>
        /// Existses the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool Exists(string id)
        {
            lock (Cache)
            {
                return Cache.ContainsKey(id);                
            }
        }


        /// <summary>
        /// Stores the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        public static T Store(T item)
        {
            lock (Cache)
            {
                item = Worker.Save(item);
                Cache[item.Id] = item;
                return item;
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>`0[][].</returns>
        internal static T[] GetAll()
        {
            lock (Cache)
            {
                return Cache.Values.ToArray();
            }
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        internal static T Delete(T item)
        {
            lock (Cache)
            {
                if (Exists(item.Id))
                {
                    Cache.Remove(item.Id);
                }
                return Worker.Delete(item);
            }
        }


        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>`0.</returns>
        internal static T Delete(string id)
        {
            lock (Cache)
            {
                if (Exists(id))
                {
                    Cache.Remove(id);
                }
                return Worker.Delete(id);
            }
        }



        internal static int Count(RequestBase req)
        {
            return Worker.Count(req);
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>`0[][].</returns>
        internal static T[] Query(RequestBase req)
        {
            lock (Cache)
            {
                var items = Worker.Query(req);
                foreach (var it in items)
                {
                    Cache[it.Id] = it;
                }
                return items;
            }
        }

        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[][].</returns>
        internal static byte[] ReadAttachment(T item, string name)
        {
            return Worker.ReadAttachment(item, name);
        }



        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[][].</returns>
        internal static byte[] ReadAttachment(string id, string name)
        {
            return Worker.ReadAttachment(id, name);
        }


        /// <summary>
        /// Determines whether the specified item has attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified item has attachment; otherwise, <c>false</c>.</returns>
        public static bool HasAttachment(T item, string name)
        {
            return Worker.HasAttachment(item, name);
        }

        /// <summary>
        /// Determines whether the specified identifier has attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified identifier has attachment; otherwise, <c>false</c>.</returns>
        internal static bool HasAttachment(string id, string name)
        {
            return Worker.HasAttachment(id, name);
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int64.</returns>
        public static long Count()
        {
            return Cache.LongCount();
        }



    }
}
