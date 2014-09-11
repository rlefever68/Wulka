// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 01-09-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-11-2014
// ***********************************************************************
// <copyright file="IWriteable.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Wulka.Interfaces
{
    /// <summary>
    /// Interface IWriteable
    /// </summary>
    public interface IWriteable
    {
        /// <summary>
        /// Called After the DomainObject has been written to the store.
        /// </summary>
        void OnSaved();
        /// <summary>
        /// Called Before the DomainObject is being written to the store.
        /// </summary>
        bool OnSaving();
        /// <summary>
        /// Called Before Deleting the DomainObject
        /// </summary>
        bool OnDeleting();
        /// <summary>
        /// Called After the DomainObject has been Deleted from the Store
        /// </summary>
        void OnDeleted();
    }
}
