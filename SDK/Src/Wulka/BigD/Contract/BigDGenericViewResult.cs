using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Interfaces;
using Wulka.Utils.Json;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// This is a view result from a CouchQuery that can return CouchDocuments for
    /// resulting documents (include_docs) and/or ICanJson documents for the
    /// result values. A value returned from a CouchDB view does not need to be
    /// a CouchDocument.
    /// </summary>
    public class BigDGenericViewResult : BigDViewResult
    {
        /// <summary>
        /// Return all found values of given type
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <returns>All found values.</returns>
        public IEnumerable<T> Values<T>() where T : new()
        {
            var list = new List<T>();
            foreach (JToken row in Rows())
            {
                list.Add(row["value"].Value<T>());
            }
            return list;
        }
		
		/// <summary>
        /// Return all found values as documents of given type
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <returns>All found values.</returns>
        public IEnumerable<T> ValueDocuments<T>() where T : ICanJson, new()
        {
            return RetrieveDocuments<T>("value");
        }

		/// <summary>
        /// Return all ids in value as documents of given type.
        /// </summary>
        /// <typeparam name="T">Type of value.</typeparam>
        /// <returns>All found documents.</returns>
        public IEnumerable<T> ValueDocumentsWithIds<T>() where T : IBigDbDocument, new()
        {
            return RetrieveDocumentsWithIds<T>("value");
        }
		
        public IEnumerable<T> ValueDocuments<T>(Func<T> ctor)
        {
            return RetrieveArbitraryDocuments("value", ctor);
        }

        /// <summary>
        /// Return first value found as document of given type.
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <returns>First value found or null if not found.</returns>
        public T ValueDocument<T>() where T : ICanJson, new()
        {
            return RetrieveDocument<T>("value");
        }

        public T ArbitraryValueDocument<T>(Func<T> ctor)
        {
            return RetrieveArbitraryDocument("value", ctor);
        }

        public IEnumerable<T> ArbitraryValueDocuments<T>(Func<T> ctor)
        {
            return RetrieveArbitraryDocuments("value", ctor);
        }

        /// <summary>
        /// Return all found docs as documents of given type
        /// </summary>
        /// <typeparam name="T">Type of documents.</typeparam>
        /// <returns>List of documents found.</returns>
        public IEnumerable<T> Documents<T>() where T : IBigDbDocument, new()
        {
            return RetrieveDocuments<T>("doc");
        }

        public IEnumerable<T> ArbitraryDocuments<T>(Func<T> ctor)
        {
            return RetrieveArbitraryDocuments("doc", ctor);
        }

        /// <summary>
        /// Return all found docs as CouchJsonDocuments.
        /// </summary>
        /// <returns>List of documents found.</returns>
        public IEnumerable<BigDJsonDocument> Documents()
        {
            return RetrieveDocuments<BigDJsonDocument>("doc");
        }

        /// <summary>
        /// Return first document found as document of given type
        /// </summary>
        /// <typeparam name="T">Type of document</typeparam>
        /// <returns>First document found or null if not found.</returns>
        public T Document<T>() where T : IBigDbDocument, new()
        {
            return RetrieveDocument<T>("doc");
        }

        public T ArbitraryDocument<T>(Func<T> ctor)
        {
            return RetrieveArbitraryDocument("doc", ctor);
        }

        protected virtual IEnumerable<T> RetrieveDocuments<T>(string docOrValue) where T : ICanJson, new()
        {
            var list = new List<T>();
            foreach (JToken row in Rows())
            {
                //var doc = new T();
                if (row[docOrValue] == null)
                    continue;
                var doc = JSonHelper<T>.ReadJSon(row[docOrValue].Value<JObject>());
                doc.ReadJson(row[docOrValue].Value<JObject>());
                list.Add(doc);
            }
            return list;
        }
		
		protected virtual IEnumerable<T> RetrieveDocumentsWithIds<T>(string docOrValue) where T : IBigDbDocument, new()
        {
            var list = new List<T>();
			var found = new Dictionary<string, T>();
            foreach (JToken row in Rows())
            {
				var ids = row[docOrValue].Value<JArray>();
				foreach (JToken id in ids) {
					var stringId = id.Value<string>();
					if (!found.ContainsKey(stringId)) {
						var doc = new T();
	               	 	doc.Id = stringId;
						found[stringId] = doc;
						list.Add(doc);
					}
				}
            }
            return list;
        }
		
        protected virtual T RetrieveDocument<T>(string docOrValue) where T : ICanJson, new()
        {
            foreach (JToken row in Rows())
            {
                var doc = new T();
                doc.ReadJson(row[docOrValue].Value<JObject>());
                return doc;
            }
            return default(T);
        }
        protected virtual IEnumerable<T> RetrieveArbitraryDocuments<T>(string docOrValue, Func<T> ctor)
        {
            var list = new List<T>();
            foreach (JToken row in Rows())
            {
                var doc = new BigDDocumentWrapper<T>(ctor);
                doc.ReadJson(row[docOrValue].Value<JObject>());
                list.Add(doc.Instance);
            }
            return list;
        }

        protected virtual T RetrieveArbitraryDocument<T>(string docOrValue, Func<T> ctor)
        {
            foreach (JToken row in Rows())
            {
                var doc = new BigDDocumentWrapper<T>(ctor);
                doc.ReadJson(row[docOrValue].Value<JObject>());
                return doc.Instance;
            }
            return default(T);
        }

        public IEnumerable<BigDQueryDocument> RowDocuments()
        {
            return RowDocuments<BigDQueryDocument>();
        }

        public IEnumerable<T> RowDocuments<T>() where T : ICanJson, new()
        {
            var list = new List<T>();
            foreach (JObject row in Rows())
            {
                var doc = new T();
                doc.ReadJson(row);
                list.Add(doc);
            }
            return list;
        }

        public IEnumerable<T> ArbitraryRowDocuments<T>(Func<T> ctor)
        {
            var list = new List<T>();
            foreach (JObject row in Rows())
            {
                var doc = new BigDDocumentWrapper<T>(ctor);
                doc.ReadJson(row);
                list.Add(doc.Instance);
            }
            return list;
        }
    }
}