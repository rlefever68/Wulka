// ***********************************************************************
// Assembly         : Iris.Fx
// Author           : Rafael Lefever
// Created          : 06-06-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 06-06-2014
// ***********************************************************************
// <copyright file="BerkleyDbProviderWorker.cs" company="Insoft">
//     Copyright (c) Insoft. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using BerkeleyDB;
using Iris.Fx.Interfaces;

namespace Iris.Fx.BerkleyDb
{
    /// <summary>
    /// Class BerkleyDbProviderWorker.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class BerkleyDbProviderWorker<T> : IProvider<T>
        where T: IDomainObject, new()
    {
        /// <summary>
        /// Saves the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] Save(T[] items)
        {
            throw new NotImplementedException();
        }

        private static Database CreateDatabase()
        {
            return BTreeDatabase.Open(
                typeof (T).FullName,
                new BTreeDatabaseConfig() {AutoCommit = true});
        }
        
    
        /// <summary>
        /// Saves the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T Save(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T GetById(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] GetAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T Delete(T item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>T.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T Delete(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the specified items.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] Delete(T[] items)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Wheres the specified field name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] Where(string fieldName, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] Query(string query, params object[] args)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries the sorted.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="sortBy">The sort by.</param>
        /// <param name="arguments">The arguments.</param>
        /// <returns>T[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public T[] QuerySorted(string query, string sortBy, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public byte[] ReadAttachment(T item, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns>System.Byte[].</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public byte[] ReadAttachment(string id, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified item has attachment.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified item has attachment; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasAttachment(T item, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the specified identifier has attachment.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        /// <returns><c>true</c> if the specified identifier has attachment; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasAttachment(string id, string name)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int Count()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Counts the where.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountWhere(string fieldName, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Counts the query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="args">The arguments.</param>
        /// <returns>System.Int32.</returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountQuery(string query, params object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
