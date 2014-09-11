// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 08-10-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-10-2014
// ***********************************************************************
// <copyright file="Bag.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Data
{

    /// <summary>
    /// Class Bag.
    /// </summary>
    [DataContract]
    public abstract class Bag : TaxonomyObject<Bag>, IBag
    {


        /// <summary>
        /// Grabs the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>IDomainObject.</returns>
        public IDomainObject Grab(string id)
        {
            return Find(id);
        }

        /// <summary>
        /// Puts the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>IDomainObject.</returns>
        public IDomainObject Put(IDomainObject item)
        {
            return AddPart(item);
        }

        protected abstract void Save();


        /// <summary>
        /// Gets the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>IEnumerable&lt;IDomainObject&gt;.</returns>
        public IEnumerable<IDomainObject> Get(Func<IDomainObject,bool> predicate)
        {
            return Parts.Where(predicate);
        }



        


        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<Bag>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<Bag>.Validate(this);
        }

    }
}
