// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-19-2013
//
// Last Modified By : ON8RL
// Last Modified On : 07-13-2014
// ***********************************************************************
// <copyright file="ProviderWorkerCouchDb.cs" company="Broobu">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ComponentModel.Composition;
using System.Linq;
using NLog;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Domain;
using Wulka.Domain.Interfaces;
using Wulka.Exceptions;
using Wulka.Interfaces;
using Wulka.Utils;

namespace Wulka.BigD
{
    
    [Export(typeof(IProvider<>))]
    public class BigDWorker<T> : IProvider<T>
        where T : IDomainObject, new()
    {


        /// <summary>
        /// The _logger
        /// </summary>
        private readonly Logger _logger = LogManager.GetLogger(String.Format("BigDWorker<{0}>", typeof(T).Name));
        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        public T Save(T item)
        {
            try
            {
                Delete(item);
                return (T)Repository<T>.Db.SaveDocument(item);
            }
            catch (Exception exception)
            {
                item.AddError(exception.Message);
                _logger.Error(exception.GetCombinedMessages());
                return item;
            }
        }




        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Count()
        {
            return Repository<T>.Db.CountDocuments();
        }


        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>Stream.</returns>
        public byte[] ReadAttachment(T item, string name)
        {
            if (HasAttachment(item, name))
                return Repository<T>.Db
                    .ReadAttachment(item, name)
                    .GetResponseStream()
                    .ToBytes();
            return null;
        }

        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns>Stream.</returns>
        public byte[] ReadAttachment(string id, string name)
        {
            if (HasAttachment(id, name))
                return Repository<T>.Db
                    .ReadAttachment(id, name)
                    .GetResponseStream()
                    .ToBytes();
            return null;
        }

        /// <summary>
        /// Determines whether the specified item has attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified item has attachment; otherwise, <c>false</c>.</returns>
        public bool HasAttachment(T item, string name)
        {
            return Repository<T>.Db.HasAttachment(item, name);
        }

        /// <summary>
        /// Determines whether the specified identifier has attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified identifier has attachment; otherwise, <c>false</c>.</returns>
        public bool HasAttachment(string id, string name)
        {
            return Repository<T>.Db.HasAttachment(id, name);
        }


        /// <summary>
        /// Gets the session.
        /// </summary>
        /// <value>The session.</value>
        public object Session { get; private set; }

        /// <summary>
        /// Saves the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>`0[][].</returns>
        public T[] Save(T[] items)
        {
            try
            {
                return items.Select(Save).ToArray();
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
                return null;
            }
           
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>`0.</returns>
        public T GetById(string id)
        {
            try
            {
                return Repository<T>.Db.GetDocument<T>(id);
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
                return default(T);
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>`0[][].</returns>
        public T[] GetAll()
        {
            return Repository<T>.Db.GetAllDocuments<T>().ToArray();
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>`0.</returns>
        public T Delete(T item)
        {
            var it = item;
            try
            {
                it = GetById(item.Id);
                if (it != null)
                {
                    Repository<T>.Db.DeleteDocument(it);
                }
                return it;
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
                if (it != null)
                {
                    it.AddError(exception.Message);
                }
                return it;
            }
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>`0.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T Delete(string id)
        {
            var it = default(T);
            try
            {
                it = GetById(id);
                if (it != null)
                    Repository<T>.Db.DeleteDocument(it);
                return it;
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
                if (it != null)
                    it.AddError(exception.Message);
                return it;
            }
        }




        /// <summary>
        /// Deletes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>`0[][].</returns>
        public T[] Delete(T[] items)
        {
            try
            {
                return items.Select(Delete).ToArray();
            }
            catch (Exception exception)
            {
               _logger.Error(exception.GetCombinedMessages());
                return items;
            }
        }

        /// <summary>
        /// Wheres the specified request.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>T[].</returns>

        /// <summary>
        /// Gets the precalculated response.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>T[].</returns>
        private T[] GetResponse(RequestBase request)
        {
            T[] res = null;
            try
            {
                 res=Repository<T>.Db
                    .Query(request.DocName, request.ViewName)
                    .StartKeyDocumentId(request.StartId)
                    .EndKeyDocumentId(request.EndId)
                    .StartKey(request.StartKey)
                    .EndKey(request.EndKey)
                    .Skip(request.Skip)
                    .Limit(request.Limit)
                    .Reduce(request.Reduce)
                    .Stale(request.Stale)
                    .IncludeDocuments(request.IncludeDocs)
                    .GetResult()
                    .Documents<T>()
                    .ToArray();
            }
            catch (Exception)
            {
                res = GetTempResponse(request);
            }
            return res;
        }

        /// <summary>
        /// Queries the specified req.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] Query(RequestBase request)
        {
            if (request == null) return new T[] { };
            var res = !request.KeepView
                ? GetTempResponse(request) :
                GetResponse(request);
            foreach (var item in res)
            {
                item.Request = request;
            }
            return res;
        }

        /// <summary>
        /// Temporaries the result.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns>T[].</returns>
        private T[] GetTempResponse(RequestBase request)
        {
            IBigDViewDefinition vw = null;
            try
            {
                if(String.IsNullOrWhiteSpace(request.DocName))
                    request.DocName = Guid.NewGuid().ToString();
                if(String.IsNullOrWhiteSpace(request.ViewName))
                    request.ViewName = Guid.NewGuid().ToString();
                vw = GetView(request);
                var res = vw
                    .LinqQuery<T>(request)
                    .ToArray();
                return res;
            }
            catch (Exception exception)
            {
                _logger.Error(exception.GetCombinedMessages());
                return new T[] {};
            }
            finally
            {
                if (vw != null)
                    if(!request.KeepView)
                        Repository<T>.Db.DeleteDocument(vw.Doc);
            }
        }

        private IBigDViewDefinition GetView(RequestBase request)
        {
            try
            {
                return Repository<T>.Db.NewTempView(request.DocName,
                    request.ViewName,
                    request.Function);
            }
            catch (Exception)
            {
                return Repository<T>.Db.Query(request.DocName, request.ViewName).View;
            }
        }


        /// <summary>
        /// Counts the specified field name.
        /// </summary>
        /// <param name="req">The req.</param>
        /// <returns>System.Int32.</returns>
        public int Count(RequestBase request)
        {
            if (request == null) return 0;
            var vw = GetView(request);
            var res = vw.LinqQuery<T>(request).Count();
            if(!request.KeepView)
                Repository<T>.Db.DeleteDocument(vw.Doc);
            return res;
        }

    }
}
