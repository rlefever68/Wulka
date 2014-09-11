// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-06-2014
// ***********************************************************************
// <copyright file="CouchDesignDocument.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
#if XAMARIN
#else
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.BigD.Contract.Lucene;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// A named design document in CouchDB. Holds CouchViewDefinitions and CouchLuceneViewDefinitions (if you use Couchdb-Lucene).
    /// </summary>
	public class BigDDesignDocument : BigDDocument, IEquatable<BigDDesignDocument>
	{
        /// <summary>
        /// The definitions
        /// </summary>
		public IList<BigDViewDefinition> Definitions = new List<BigDViewDefinition>();
		
#if XAMARIN
#else
		// This List is only used if you also have Couchdb-Lucene installed
        /// <summary>
        /// The lucene definitions
        /// </summary>
		public IList<BigDLuceneViewDefinition> LuceneDefinitions = new List<BigDLuceneViewDefinition>();
#endif

        /// <summary>
        /// The language
        /// </summary>
		public string Language = "javascript";
        /// <summary>
        /// The owner
        /// </summary>
		public IBigDDatabase Owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDesignDocument"/> class.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <param name="owner">The owner.</param>
		public BigDDesignDocument(string documentId, IBigDDatabase owner)
			: base("_design/" + documentId)
		{
			Owner = owner;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDDesignDocument"/> class.
        /// </summary>
		public BigDDesignDocument()
		{
			
		}

        /// <summary>
        /// Add view without a reduce function.
        /// </summary>
        /// <param name="name">Name of view</param>
        /// <param name="map">Map function</param>
        /// <returns>CouchViewDefinition.</returns>
		public BigDViewDefinition AddView(string name, string map)
		{
			return AddView(name, map, null);
		}

        /// <summary>
        /// Add view with a reduce function.
        /// </summary>
        /// <param name="name">Name of view</param>
        /// <param name="map">Map function</param>
        /// <param name="reduce">Reduce function</param>
        /// <returns>CouchViewDefinition.</returns>
		public BigDViewDefinition AddView(string name, string map, string reduce)
		{
			var def = new BigDViewDefinition(name, map, reduce, this);
			Definitions.Add(def);
			return def;
		}

        /// <summary>
        /// Removes the view named.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
		public void RemoveViewNamed(string viewName)
		{
			RemoveView(FindView(viewName));
		}

        /// <summary>
        /// Finds the view.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>CouchViewDefinition.</returns>
		private BigDViewDefinition FindView(string name)
		{
			return Definitions.First(x => x.Name == name);
		}

        /// <summary>
        /// Removes the view.
        /// </summary>
        /// <param name="view">The view.</param>
		public void RemoveView(BigDViewDefinition view)
		{
			view.Doc = null;
			Definitions.Remove(view);
		}


#if XAMARIN

#else

        /// <summary>
        /// Add Lucene fulltext view.
        /// </summary>
        /// <param name="name">Name of view</param>
        /// <param name="index">Index function</param>
        /// <returns>CouchLuceneViewDefinition.</returns>
		public BigDLuceneViewDefinition AddLuceneView(string name, string index)
		{
			var def = new BigDLuceneViewDefinition(name, index, this);
			LuceneDefinitions.Add(def);
			return def;
		}

        /// <summary>
        /// Add a Lucene view with a predefined index function that will index EVERYTHING.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>CouchLuceneViewDefinition.</returns>
		public BigDLuceneViewDefinition AddLuceneViewIndexEverything(string name)
		{
			return AddLuceneView(name,
								 @"function(doc) {
									var ret = new Document();

									function idx(obj) {
									for (var key in obj) {
										switch (typeof obj[key]) {
										case 'object':
										idx(obj[key]);
										break;
										case 'function':
										break;
										default:
										ret.add(obj[key]);
										break;
										}
									}
									};

									idx(doc);

									if (doc._attachments) {
									for (var i in doc._attachments) {
										ret.attachment(""attachment"", i);
									}
									}}");
		}

		// All these three methods duplicated for Lucene views, perhaps we should hold them all in one List?
        /// <summary>
        /// Removes the lucene view named.
        /// </summary>
        /// <param name="viewName">Name of the view.</param>
		public void RemoveLuceneViewNamed(string viewName)
		{
			RemoveLuceneView(FindLuceneView(viewName));
		}

        /// <summary>
        /// Finds the lucene view.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>CouchLuceneViewDefinition.</returns>
		private BigDLuceneViewDefinition FindLuceneView(string name)
		{
			return LuceneDefinitions.Where(x => x.Name == name).First();
		}

        /// <summary>
        /// Removes the lucene view.
        /// </summary>
        /// <param name="view">The view.</param>
		public void RemoveLuceneView(BigDLuceneViewDefinition view)
		{
			view.Doc = null;
			LuceneDefinitions.Remove(view);
		}

#endif
        /// <summary>
        /// If this design document is missing in the database,
        /// or if it is different - then we save it overwriting the one in the db.
        /// </summary>
		public void Synch()
		{
			if (!Owner.HasDocument(this)) {
				Owner.SaveDocument(this);
			} else
			{
				var docInDb = Owner.GetDocument<BigDDesignDocument>(Id);
				if (!docInDb.Equals(this)) {
					// This way we forcefully save our version over the one in the db.
					Rev = docInDb.Rev;
					Owner.WriteDocument(this);
				}
			}
		}

        /// <summary>
        /// Writes the json.
        /// </summary>
        /// <param name="writer">The writer.</param>
		public override void WriteJson(JsonWriter writer)
		{
			WriteIdAndRev(this, writer);
			writer.WritePropertyName("language");
			writer.WriteValue(Language);
			writer.WritePropertyName("views");
			writer.WriteStartObject();
			foreach (var definition in Definitions)
			{
				definition.WriteJson(writer);
			}
			writer.WriteEndObject();
#if XAMARIN

#else
			// If we have Lucene definitions we write them too
			if (LuceneDefinitions.Count > 0)
			{
				writer.WritePropertyName("fulltext");
				writer.WriteStartObject();
				foreach (var definition in LuceneDefinitions)
				{
					definition.WriteJson(writer);
				}
				writer.WriteEndObject();
			}
#endif
		}

        /// <summary>
        /// Reads the json.
        /// </summary>
        /// <param name="obj">The object.</param>
		public override void ReadJson(JObject obj)
		{
			ReadIdAndRev(this, obj);
			if (obj["language"] != null)
				Language = obj["language"].Value<string>();
			Definitions = new List<BigDViewDefinition>();
			var views = (JObject)obj["views"];

			foreach (var property in views.Properties())
			{
				var v = new BigDViewDefinition(property.Name, this);
				v.ReadJson((JObject)views[property.Name]);
				Definitions.Add(v);
			}

#if XAMARIN

#else

			var fulltext = (JObject)obj["fulltext"];
			// If we have Lucene definitions we read them too
			if (fulltext != null)
			{
				foreach (var property in fulltext.Properties())
				{
					var v = new BigDLuceneViewDefinition(property.Name, this);
					v.ReadJson((JObject) fulltext[property.Name]);
					LuceneDefinitions.Add(v);
				}
			}
#endif
		}

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		public bool Equals(BigDDesignDocument other)
		{
			return Id.Equals(other.Id) && Language.Equals(other.Language) && Definitions.SequenceEqual(other.Definitions);
		}
	}
}