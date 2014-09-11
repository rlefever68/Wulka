// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-22-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 12-22-2013
// ***********************************************************************
// <copyright file="CouchBulkDocuments.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Interfaces;

namespace Wulka.BigD
{
    /// <summary>
    /// Only used as psuedo doc when doing bulk updates/inserts.
    /// </summary>
    public class BigDBulkDocuments : ICanJson
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BigDBulkDocuments"/> class.
        /// </summary>
        /// <param name="docs">The docs.</param>
        public BigDBulkDocuments(IEnumerable<IBigDbDocument> docs)
        {
            Docs = docs;
        }

        /// <summary>
        /// Gets the docs.
        /// </summary>
        /// <value>The docs.</value>
        public IEnumerable<IBigDbDocument> Docs { get; private set; }

        #region ICouchBulk Members

        /// <summary>
        /// Counts this instance.
        /// </summary>
        /// <returns>System.Int32.</returns>
        public int Count()
        {
            return Docs.Count();
        }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void WriteJson(JsonWriter writer)
        {
            writer.WritePropertyName("docs");
            writer.WriteStartArray();
            foreach (IBigDbDocument doc in Docs)
            {
                if (doc is ISelfContained)
                {
                    doc.WriteJson(writer);
                }
                else
                {
                    writer.WriteStartObject();
                    doc.WriteJson(writer);
                    writer.WriteEndObject();
                }
            }
            writer.WriteEndArray();
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public virtual void ReadJson(JObject obj)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}