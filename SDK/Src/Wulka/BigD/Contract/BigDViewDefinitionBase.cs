// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 02-03-2014
// ***********************************************************************
// <copyright file="CouchViewDefinitionBase.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Wulka.BigD.Contract.Interfaces;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// Class BigDViewDefinitionBase.
    /// </summary>
    public abstract class BigDViewDefinitionBase : IBigDViewDefinitionBase 
    {
        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        public BigDDesignDocument Doc { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDViewDefinitionBase"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="doc">The document.</param>
        protected BigDViewDefinitionBase(string name, BigDDesignDocument doc)
        {
            Doc = doc;
            Name = name;
        }

        /// <summary>
        /// Databases this instance.
        /// </summary>
        /// <returns>ICouchDatabase.</returns>
        public IBigDDatabase Db()
        {
            return Doc.Owner;
        }

        /// <summary>
        /// Requests this instance.
        /// </summary>
        /// <returns>ICouchRequest.</returns>
        public IBigDRequest Request()
        {
            return Db().Request(Path());
        }

        /// <summary>
        /// Pathes this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string Path()
        {
            if (Doc.Id == "_design/")
            {
                return Name;
            }
            return Doc.Id + "/_view/" + Name;
        }
    }
}