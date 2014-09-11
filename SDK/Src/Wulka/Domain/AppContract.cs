// ***********************************************************************
// Assembly         : Wulka
// Author           : Rafael Lefever
// Created          : 09-11-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 09-05-2014
// ***********************************************************************
// <copyright file="AppContract.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Base;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Domain
{
    /// <summary>
    /// Class AppContract.
    /// </summary>
    [DataContract]
    public class AppContract : TaxonomyObject<AppContract>, IAppContract
    {
        /// <summary>
        /// Gets or sets the publisher.
        /// </summary>
        /// <value>The publisher.</value>
        [DataMember]
        public string Publisher { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DataMember]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the name of the technical.
        /// </summary>
        /// <value>The name of the technical.</value>
        [DataMember]
        public string TechnicalName { get; set; }

        /// <summary>
        /// Gets or sets the layer.
        /// </summary>
        /// <value>The layer.</value>
        [DataMember]
        public string Layer { get; set; }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<AppContract>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<AppContract>.Validate(this);
        }

    }
}
