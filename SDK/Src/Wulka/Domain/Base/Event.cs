// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 08-29-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-29-2014
// ***********************************************************************
// <copyright file="Reservation.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class Reservation.
    /// </summary>
    [DataContract]
    public class Event : TaxonomyObject<Event>, IEvent
    {
        private TimeSpan _timeSpan;

        /// <summary>
        /// Gets or sets the time span.
        /// </summary>
        /// <value>The time span.</value>
        [DataMember]
        public TimeSpan TimeSpan
        {
            get { return _timeSpan; }
            set { _timeSpan = value; RaisePropertyChanged("TimeSpan"); }
        }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<Event>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<Event>.Validate(this);
        }
    }
}
