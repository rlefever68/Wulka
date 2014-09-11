// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : rlefever
// Created          : 11-22-2013
//
// Last Modified By : rlefever
// Last Modified On : 11-22-2013
// ***********************************************************************
// <copyright file="CouchDocument.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Domain.Interfaces;
using Wulka.Interfaces;
using Wulka.Utils;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// This is a base class that domain objects can inherit in order to get
    /// Id and Rev instance variables. You can also implement ICouchDocument yourself if
    /// you are not free to pick this class as your base. Some static methods to read and write
    /// CouchDB documents are also kept here.
    /// This class can also be used if you only need to retrieve id and revision from CouchDB.
    /// See sample subclasses to understand how to use this class.
    /// </summary>
    [DataContract]
    public class BigDDocument : IReconcilingDocument
    {
        /// <summary>
        /// The _reconcile by
        /// </summary>
        private ReconcileStrategy _reconcileBy = ReconcileStrategy.None;
        
        /// <summary>
        /// The _source data
        /// </summary>
        private BigDDocument _sourceData;

        private string _id;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDocument"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="rev">The rev.</param>
        public BigDDocument(string id, string rev)
        {
            Id = id;
            Rev = rev;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDocument"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public BigDDocument(string id)
        {
            Id = id;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDocument"/> class.
        /// </summary>
        public BigDDocument()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDocument"/> class.
        /// </summary>
        /// <param name="doc">The document.</param>
        public BigDDocument(IDictionary<string, JToken> doc)
            : this(doc["_id"].Value<string>(), doc["_rev"].Value<string>())
        {
        }

        /// <summary>
        /// Gets or sets the reconcile by.
        /// </summary>
        /// <value>The reconcile by.</value>
        public virtual ReconcileStrategy ReconcileBy
        {
            get { return _reconcileBy; }
            set { _reconcileBy = value; }
        }

        /// <summary>
        /// The data set used to construct this document
        /// </summary>
        /// <value>The source data.</value>
        protected BigDDocument SourceData { get { return _sourceData; } }

        #region ICouchDocument Members

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        
        public string Id 
        {
            get { return _id; }
            set { SetId(value);}
        }

        protected virtual void SetId(string value)
        {
            if(_id==value) return;
            _id = String.IsNullOrEmpty(value) ? GuidUtils.NewCleanGuid : value;
        }


        /// <summary>
        /// Gets or sets the rev.
        /// </summary>
        /// <value>The rev.</value>
        public string Rev {get; set; }

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
        public virtual void WriteJson(JsonWriter writer)
        {
            WriteIdAndRev(this, writer);
        }

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="obj">The object.</param>
        public virtual void ReadJson(JObject obj)
        {
            ReadIdAndRev(this, obj);

            if (ReconcileBy == ReconcileStrategy.None) return;
            var constructorInfo = GetType().GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null) return;
            _sourceData = (BigDDocument)constructorInfo.Invoke(new object[0]);
            // set this to prevent infinite recursion
            _sourceData.ReconcileBy = ReconcileStrategy.None;
            _sourceData.ReadJson(obj);
        }

        #endregion

        /// <summary>
        /// Equals the fields.
        /// </summary>
        /// <param name="v1">The v1.</param>
        /// <param name="v2">The v2.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private static bool EqualFields(object v1, object v2)
        {
            if (v1 == null)
                return v2 == null;
            return v2 != null && v1.Equals(v2);
        }

        /// <summary>
        /// Automatically reconcile the database copy with the target instance. This method
        /// uses reflection to perform the reconcilliation, and as such won't perform as well
        /// as other version, but is available for low-occurance scenarios
        /// </summary>
        /// <param name="databaseCopy">The database copy.</param>
        protected void AutoReconcile(IBigDbDocument databaseCopy)
        {
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
                // if we haven't changed the field, 
                if (EqualFields(field.GetValue(_sourceData), field.GetValue(this)))
                    field.SetValue(this, field.GetValue(databaseCopy));

            foreach (var prop in properties)
                if (!prop.CanWrite || prop.GetIndexParameters().Length > 0)
                    continue;
                else if (EqualFields(prop.GetValue(_sourceData, null), prop.GetValue(this, null)))
                    prop.SetValue(this, prop.GetValue(databaseCopy, null), null);

            // this is non-negotiable
            Rev = databaseCopy.Rev;
        }

        /// <summary>
        /// Automatics the clone.
        /// </summary>
        /// <returns>CouchDocument.</returns>
        protected BigDDocument AutoClone()
        {
            var constructorInfo = GetType().GetConstructor(Type.EmptyTypes);
            if (constructorInfo == null) return null;
            var doc = constructorInfo.Invoke(new object[0]) as BigDDocument;
            var properties = GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (var field in fields)
                field.SetValue(doc, field.GetValue(this));

            foreach (var prop in properties)
                if (!prop.CanWrite || prop.GetIndexParameters().Length > 0)
                    continue;
                else
                    prop.SetValue(doc, prop.GetValue(this, null), null);
            return doc;
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>CouchDocument.</returns>
        protected virtual BigDDocument Clone()
        {
            var doc = (BigDDocument) GetType().GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            doc.Rev = Rev;
            doc.Id = Id;

            return doc;
        }

        /// <summary>
        /// Saves the commited.
        /// </summary>
        public void SaveCommited()
        {
            switch (ReconcileBy)
            {
                case ReconcileStrategy.AutoMergeFields:
                    _sourceData = AutoClone();
                    break;
                case ReconcileStrategy.ManualMergeFields:
                    _sourceData = Clone();
                    break;
            }

            if (_sourceData != null)
                _sourceData.ReconcileBy = ReconcileStrategy.None;
        }

        /// <summary>
        /// Called by the runtime when a conflict is detected during save. The supplied parameter
        /// is the database copy of the document being saved.
        /// </summary>
        /// <param name="databaseCopy">The database copy.</param>
        public virtual void Reconcile(IBigDbDocument databaseCopy)
        {
            if (ReconcileBy == ReconcileStrategy.AutoMergeFields)
            {
                AutoReconcile(databaseCopy);
                return;
            }

            Rev = databaseCopy.Rev;
        }

        /// <summary>
        /// Gets the database copy.
        /// </summary>
        /// <param name="db">The database.</param>
        /// <returns>IReconcilingDocument.</returns>
        public virtual IReconcilingDocument GetDatabaseCopy(IBigDDatabase db)
        {
            return db.GetDocument<BigDDocument>(Id);
        }


        ///// <summary>
        ///// Writes the json object.
        ///// </summary>
        ///// <param name="writer">The writer.</param>
        //public void WriteJsonObject(JsonWriter writer)
        //{
        //    writer.WriteStartObject();
        //    WriteJson(writer);
        //    writer.WriteEndObject();
        //}

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <returns>System.String.</returns>
        public static string WriteJson(ICanJson doc, bool isData = true)
        {
            if (doc is IDomainObject)
            {
                return JsonConvert.SerializeObject(doc, Formatting.Indented,
                    new JsonSerializerSettings() {TypeNameHandling = TypeNameHandling.Objects});
            }
            else
            {
                var sb = new StringBuilder();
                using (JsonWriter jsonWriter = new JsonTextWriter(new StringWriter(sb, CultureInfo.InvariantCulture)))
                {
                    if (!(doc is ISelfContained))
                    {
                        jsonWriter.WriteStartObject();
                        doc.WriteJson(jsonWriter);
                        jsonWriter.WriteEndObject();
                    }
                    else
                        doc.WriteJson(jsonWriter);

                    string result = sb.ToString();
                    return result;

                }
            }

        }

        /// <summary>
        /// Writes the identifier and rev.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="writer">The writer.</param>
        public static void WriteIdAndRev(IBigDbDocument doc, JsonWriter writer)
        {
            if (doc.Id != null)
            {
                writer.WritePropertyName("_id");
                writer.WriteValue(doc.Id);
            }
            if (doc.Rev != null)
            {
                writer.WritePropertyName("_rev");
                writer.WriteValue(doc.Rev);
            }
        }

        /// <summary>
        /// Reads the identifier and rev.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="obj">The object.</param>
        public static void ReadIdAndRev(IBigDbDocument doc, JObject obj)
        {
            doc.Id = obj["_id"].Value<string>();
            doc.Rev = obj["_rev"].Value<string>();
        }

        /// <summary>
        /// Reads the identifier and rev.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <param name="reader">The reader.</param>
        public static void ReadIdAndRev(IBigDbDocument doc, JsonReader reader)
        {
            reader.Read();
            if (reader.TokenType == JsonToken.PropertyName && (reader.Value as string == "_id"))
            {
                reader.Read();
                doc.Id = reader.Value as string;
            }
            reader.Read();
            if (reader.TokenType == JsonToken.PropertyName && (reader.Value as string == "_rev"))
            {
                reader.Read();
                doc.Rev = reader.Value as string;
            }
        }


    }
}