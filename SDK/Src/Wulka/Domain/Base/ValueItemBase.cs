// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-26-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-28-2014
// ***********************************************************************
// <copyright file="ValueItem.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class ValueItem.
    /// </summary>
    [DataContract]
    public abstract class ValueItemBase : Parameter, IValueItem
    {
        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        protected IDomainObject Subject { get { return Master.Find(SubjectId); } }

        /// <summary>
        /// Gets or sets the formula.
        /// </summary>
        /// <value>The formula.</value>
        [DataMember]
        public IFormula Formula { get; set; }
        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>The subject identifier.</value>
        [DataMember]
        public string SubjectId { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="ValueItemBase"/> class.
        /// </summary>
        protected ValueItemBase()
        {
            Formula = CreateFormula();
        }

        protected abstract FormulaBase CreateFormula();
    }
}