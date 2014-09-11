// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 08-10-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-10-2014
// ***********************************************************************
// <copyright file="IBag.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Wulka.Domain.Interfaces;

namespace Wulka.Data
{
    /// <summary>
    /// Interface IBag
    /// </summary>
    public interface IBag : IComposedObject
    {
        /// <summary>
        /// Grabs the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDomainObject.</returns>
        IDomainObject Grab(string id);
        /// <summary>
        /// Puts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IDomainObject.</returns>
        IDomainObject Put(IDomainObject item);
        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>IEnumerable&lt;IDomainObject&gt;.</returns>
        IEnumerable<IDomainObject> Get(Func<IDomainObject, bool> predicate);

    }
}