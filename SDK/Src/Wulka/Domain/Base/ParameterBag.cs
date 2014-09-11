// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-26-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-28-2014
// ***********************************************************************
// <copyright file="ParameterBag.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class ParameterBag.
    /// </summary>
    [DataContract]
    public abstract class ParameterBag : Folder
    {
        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<ParameterBag>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<ParameterBag>.Validate(this);
        }
    }
}