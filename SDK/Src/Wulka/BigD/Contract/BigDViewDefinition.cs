// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-06-2014
// ***********************************************************************
// <copyright file="CouchViewDefinition.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.BigD.Contract.Linq;
using Wulka.Domain;

namespace Wulka.BigD.Contract
{
	/// <summary>
	/// A definition of a CouchDB view with a name, a map and a reduce function and a reference to the
	/// owning CouchDesignDocument.
	/// </summary>
	public class BigDViewDefinition : BigDViewDefinitionBase, IEquatable<IBigDViewDefinition>, IBigDViewDefinition
	{
		/// <summary>
		/// Basic constructor used in ReadJson() etc.
		/// </summary>
		/// <param name="name">View name used in URI.</param>
		/// <param name="doc">A design doc, can also be created on the fly.</param>
		public BigDViewDefinition(string name, BigDDesignDocument doc) : base(name, doc) {}

		/// <summary>
		/// Constructor used for permanent views, see CouchDesignDocument.
		/// </summary>
		/// <param name="name">View name.</param>
		/// <param name="map">Map function.</param>
		/// <param name="reduce">Optional reduce function.</param>
		/// <param name="doc">Parent document.</param>
		public BigDViewDefinition(string name, string map, string reduce, BigDDesignDocument doc): base(name, doc)
		{
			Map = map;
			Reduce = reduce;
		}

		/// <summary>
		/// Gets or sets the map.
		/// </summary>
		/// <value>The map.</value>
		public string Map { get; set; }
		/// <summary>
		/// Gets or sets the reduce.
		/// </summary>
		/// <value>The reduce.</value>
		public string Reduce { get; set; }

		/// <summary>
		/// Touches this instance.
		/// </summary>
		public void Touch()
		{
			Query().Limit(0).GetResult();
		}

		/// <summary>
		/// Queries this instance.
		/// </summary>
		/// <returns>CouchQuery.</returns>
		public BigDQuery Query()
		{
			return Doc.Owner.Query(this);
		}

		/// <summary>
		/// Linqs the query.
		/// </summary>
		/// <param name="request"></param>
		/// <typeparam name="T"></typeparam>
		/// <returns>CouchLinqQuery&lt;T&gt;.</returns>
		public CouchLinqQuery<T> LinqQuery<T>(RequestBase request) {
			var linqProvider = new BigDQueryProvider(Db(), this);
			return new CouchLinqQuery<T>(linqProvider);
		}

		/// <summary>
		/// Writes the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void WriteJson(JsonWriter writer)
		{
			writer.WritePropertyName(Name);
			writer.WriteStartObject();
			writer.WritePropertyName("map");
			writer.WriteValue(Map);
			if (Reduce != null)
			{
				writer.WritePropertyName("reduce");
				writer.WriteValue(Reduce);
			}
			writer.WriteEndObject();
		}

		/// <summary>
		/// Reads the json.
		/// </summary>
		/// <param name="obj">The object.</param>
		public void ReadJson(JObject obj)
		{
			Map = obj["map"].Value<string>();
			if (obj["reduce"] != null)
			{
				Reduce = obj["reduce"].Value<string>();
			}
		}

		/// <summary>
		/// Utility methods to make queries shorter.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key">The key.</param>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public IEnumerable<T> Key<T>(object key) where T : IBigDbDocument, new()
		{
			return Query().Key(key).IncludeDocuments().GetResult().Documents<T>();
		}


		/// <summary>
		/// Starts the key identifier.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id">The identifier.</param>
		/// <param name="limit"></param>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public IEnumerable<T> FromStartId<T>(string id, int limit) where T: IBigDbDocument, new()
		{
			var res = Query()
				.StartKey(String.Format("\"{0}\"",id))
				.Limit(limit)
				.IncludeDocuments()
				.GetResult();
			return res.Documents<T>();
		}


		/// <summary>
		/// Keys the start end.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public IEnumerable<T> KeyStartEnd<T>(object start, object end) where T : IBigDbDocument, new()
		{
			return Query().StartKey(start).EndKey(end).IncludeDocuments().GetResult().Documents<T>();
		}

		/// <summary>
		/// Keys the start end.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="start">The start.</param>
		/// <param name="end">The end.</param>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public IEnumerable<T> KeyStartEnd<T>(object[] start, object[] end) where T : IBigDbDocument, new()
		{
			return Query().StartKey(start).EndKey(end).IncludeDocuments().GetResult().Documents<T>();
		}

		/// <summary>
		/// Alls this instance.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns>IEnumerable&lt;T&gt;.</returns>
		public IEnumerable<T> All<T>() where T : IBigDbDocument, new()
		{
			return Query().IncludeDocuments().GetResult().Documents<T>();
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <param name="other">An object to compare with this object.</param>
		/// <returns>true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.</returns>
		public bool Equals(IBigDViewDefinition other)
		{
			return 
				Name != null && 
				Name.Equals(other.Name) && 
				Map != null &&
				Map.Equals(other.Map) && 
				Reduce != null &&
				Reduce.Equals(other.Reduce);
		}
	}
}