// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-31-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-07-2014
// ***********************************************************************
// <copyright file="Provider.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using NLog;
using Wulka.Domain;
using Wulka.Domain.Interfaces;
using Wulka.Exceptions;

namespace Wulka.Data
{
    /// <summary>
    /// The Provider Template class abstracts serialization into or deserialization of DomainObjects from their storage containers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Provider<T>
        where T : IDomainObject, new()
    {

        /// <summary>
        /// Saves the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>`0[][].</returns>
        public static T[] Save(T[] items)
        {
            var lst = new List<T>();
            foreach (var item in items)
            {
                try
                {
                    var it = Save(item);
                    lst.Add(it);
                }
                catch (Exception exception)
                {
                    item.AddError(exception.Message);
                    lst.Add(item);
                }
            }
            return lst.ToArray();
        }


        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger {
            get {
                return LogManager.GetLogger(String.Format("Provider<{0}>", typeof(T).Name));
        }
        }


        

        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        public static T Save(T item)
        {
            var res = item;
            try
            {
                Logger.Info("SAVING \n*********\n{0}\n*******",item.Id);
                var canSave = item.OnSaving();
                if (!canSave)
                {
                    Logger.Info("Not allowed to save '{0}'", item.Id);
                    res.AddInfo("Not allowed to save.");
                    return res;
                }
                item.TouchedAt = DateTime.UtcNow;
                res = ObjectCache<T>.Store(item);
                Logger.Info("Saved {0} - '{1}' Revision [{2}] at {3}", typeof(T), res.Id, res.Rev, item.TouchedAt);
                Logger.Info(res.ToString());
                if (res != null)
                    item.OnSaved();
                return res;
            }
            catch (Exception exception)
            {
                Logger.Error("Error saving object {0} {1} [{2}]\n", item.DocType, item.DisplayName, item.Id );
                Logger.Error(exception.GetCombinedMessages());
                if(res!=null)
                    res.AddError(exception.GetCombinedMessages());
                return res;
            }
        }

        /// <summary>
        /// Gets the item by its identifier. If hydrate is true, the provider class will try to deserialize the item's children.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="hydrate">if set to <c>true</c> [hydrate].</param>
        /// <returns>`0.</returns>
        public static T GetById(string id, bool hydrate=false)
        {
            var res = ObjectCache<T>.Get(id);
            if (hydrate)
            {
                res.Hydrate();
            }
            return res;
        }

        /// <summary>
        /// Gets all items.
        /// </summary>
        /// <returns>`0[][].</returns>
        public static T[] GetAll()
        {
            return ObjectCache<T>.GetAll();
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        public static T Delete(T item)
        {
            var res = item;
            try
            {
                var canDelete = item.OnDeleting();
                if (!canDelete) return res;
                res = ObjectCache<T>.Delete(item);
                res.OnDeleted();
            }
            catch (Exception exception)
            {
                res.AddError(exception.GetCombinedMessages());
            }
            return res;
        }

        /// <summary>
        /// Deletes the item with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="deleteChildren">if set to <c>true</c> [delete children].</param>
        /// <returns>`0.</returns>
        public static T Delete(string id, bool deleteChildren = true)
        {
            var res = ObjectCache<T>.Get(id);
            var canDelete = res.OnDeleting();
            if (!canDelete) return res;
            res = ObjectCache<T>.Delete(id);
            if(res!=null)
                res.OnDeleted();
            return res;
        }

        /// <summary>
        /// Deletes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>`0[][].</returns>
        public static T[] Delete(T[] items)
        {
            var lst = new List<T>();
            foreach (var item in items)
            {
                try
                {
                    var res = Delete(item);
                    lst.Add(res);
                }
                catch (Exception exception)
                {
                    Logger.Error("Error Deleting {0} - '{1}' Revision [{2}]", typeof(T), item.Id,item.Rev);
                    Logger.Error(exception.GetCombinedMessages());
                    item.AddError(exception.Message);
                    lst.Add(item);
                }
            }
            return lst.ToArray();
        }

        /// <summary>
        /// Wheres the specified field name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>`0[][].</returns>
        public static T[] Where(RequestBase req)
        {
            return ObjectCache<T>.Query(req);
        }




        /// <summary>
        /// Wheres the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>`0[][].</returns>
        public static T[] Query(RequestBase req)
        {
            return ObjectCache<T>.Query(req);
        }


        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] ReadAttachment(T item, string name)
        {
            return ObjectCache<T>.ReadAttachment(item, name);
        }

        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[][].</returns>
        public static byte[] ReadAttachment(string id, string name)
        {
            return ObjectCache<T>.ReadAttachment(id, name);
        }

        /// <summary>
        /// Determines whether the specified item has attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified item has attachment; otherwise, <c>false</c>.</returns>
        public static bool HasAttachment(T item, string name)
        {
            return ObjectCache<T>.HasAttachment(item, name);
        }

        /// <summary>
        /// Determines whether the specified identifier has attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified identifier has attachment; otherwise, <c>false</c>.</returns>
        public static bool HasAttachment(string id, string name)
        {
            return ObjectCache<T>.HasAttachment(id, name);
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public static long Count()
        {
            return ObjectCache<T>.Count();
        }



    }
}
