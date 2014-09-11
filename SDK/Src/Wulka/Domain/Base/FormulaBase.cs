// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-28-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-28-2014
// ***********************************************************************
// <copyright file="Formula.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Domain.Interfaces;
using Wulka.Validation;

namespace Wulka.Domain.Base
{
    /// <summary>
    /// Class Formula.
    /// </summary>
    [DataContract]
    public abstract class FormulaBase : TaxonomyObject<FormulaBase>, IFormula
    {
        /// <summary>
        /// The _input
        /// </summary>
        private List<IValueItem> _input = new List<IValueItem>();
        /// <summary>
        /// The _output
        /// </summary>
        private List<IValueItem> _output = new List<IValueItem>();

        /// <summary>
        /// Gets or sets the input.
        /// </summary>
        /// <value>The input.</value>
        public List<IValueItem> Input
        {
            get { return _input; }
            set { _input = value; }
        }

        /// <summary>
        /// Gets or sets the output.
        /// </summary>
        /// <value>The output.</value>
        public List<IValueItem> Output
        {
            get { return _output; }
            set { _output = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FormulaBase"/> class.
        /// </summary>
        protected FormulaBase()
        {
            Icon = Properties.Resources.Formula;
        }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<FormulaBase>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<FormulaBase>.Validate(this);
        }
    }
}