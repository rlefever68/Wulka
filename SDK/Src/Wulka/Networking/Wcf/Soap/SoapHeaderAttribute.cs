// ***********************************************************************
// Assembly         : Wulka
// Author           : Rafael Lefever
// Created          : 01-10-2012
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-10-2012
// ***********************************************************************
// <copyright file="SoapHeaderAttribute.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;

namespace Wulka.Networking.Wcf.Soap
{
    /// <summary>
    /// Class SoapHeaderAttribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SoapHeaderAttribute : Attribute
    {
        /// <summary>
        /// The name
        /// </summary>
        string name;
        /// <summary>
        /// The type
        /// </summary>
        Type type;
        /// <summary>
        /// The direction
        /// </summary>
        SoapHeaderDirection direction = SoapHeaderDirection.In;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoapHeaderAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        public SoapHeaderAttribute(string name, Type type)
        {
            this.name = name;
            this.type = type;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get
            {
                return name;
            }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public Type Type
        {
            get
            {
                return type;
            }
        }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public SoapHeaderDirection Direction
        {
            get
            {
                return direction;
            }
            set
            {
                direction = value;
            }
        }
    }

    /// <summary>
    /// Enum SoapHeaderDirection
    /// </summary>
    [Flags]
    public enum SoapHeaderDirection
    {
        /// <summary>
        /// The in
        /// </summary>
        In = 1,
        /// <summary>
        /// The out
        /// </summary>
        Out = 2,
        /// <summary>
        /// The in out
        /// </summary>
        InOut = 3,
    }
}
