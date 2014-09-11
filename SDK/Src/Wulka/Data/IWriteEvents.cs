// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-09-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-11-2014
// ***********************************************************************
// <copyright file="IWriteGraph.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;

namespace Wulka.Data
{
    /// <summary>
    /// Interface IWriteGraph
    /// </summary>
    public interface IWriteEvents
    {
        /// <summary>
        /// Called when [saved].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainObject">The domain object.</param>
        void OnSaved<T>(DomainObject<T> domainObject) 
            where T : IDomainObject;

        /// <summary>
        /// Called when [deleting].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainObject">The domain object.</param>
        bool OnDeleting<T>(DomainObject<T> domainObject) 
            where T : IDomainObject;

        /// <summary>
        /// Called when [saving].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainObject">The domain object.</param>
        bool OnSaving<T>(DomainObject<T> domainObject)
            where T : IDomainObject;

        /// <summary>
        /// Called when [deleted].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="domainObject">The domain object.</param>
        void OnDeleted<T>(DomainObject<T> domainObject)
            where T : IDomainObject;
    }
}