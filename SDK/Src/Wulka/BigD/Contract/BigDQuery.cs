// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-13-2014
// ***********************************************************************
// <copyright file="CouchQuery.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Contract.Interfaces;
using Wulka.Interfaces;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// A view query with all its options. A CouchQuery is constructed to hold all query options that
    /// CouchDB views support and to support ETag caching.
    /// A CouchQuery object can be executed multiple times, holds the last result, the ETag for it,
    /// and a reference to the CouchDatabase object used to perform the query.
    /// </summary>
    public class BigDQuery
    {
        /// <summary>
        /// The view
        /// </summary>
        public readonly IBigDViewDefinition View;

        // Special options
        /// <summary>
        /// The check e tag using head
        /// </summary>
        public bool checkETagUsingHead;
        /// <summary>
        /// The options
        /// </summary>
        public Dictionary<string, string> Options = new Dictionary<string, string>();
        /// <summary>
        /// The post data
        /// </summary>
        public string postData;
        /// <summary>
        /// The result
        /// </summary>
        public BigDViewResult Result;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDQuery"/> class.
        /// </summary>
        /// <param name="view">The view.</param>
        public BigDQuery(IBigDViewDefinition view)
        {
            View = view;
        }

        /// <summary>
        /// Clears the options.
        /// </summary>
        public void ClearOptions()
        {
            Options = new Dictionary<string, string>();
        }

        /// <summary>
        /// Setting POST data which will automatically trigger the query to be a POST request.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Data(string data)
        {
            postData = data;
            return this;
        }


        /// <summary>
        /// This is a bulk key request, not to be confused with requests using complex keys, see Key().
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Keys(object[] keys)
        {
            var bulk = new BigDBulkKeys(keys);
            var json = BigDDocument.WriteJson(bulk);
            Data(json);
            return this;
        }

        /// <summary>
        /// This is a bulk key request, not to be confused with requests using complex keys, see Key().
        /// </summary>
        /// <param name="keys">The keys.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Keys(IList<object> keys)
        {
            var bulk = new BigDBulkKeys(keys.ToArray());
            var json = BigDDocument.WriteJson(bulk);
            Data(json);
            return this;
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Key(object value)
        {
            Options["key"] = value == null ? "null" : MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Key(params object[] value)
        {
           Options["key"] = value == null ? "null" : JToken.FromObject(value).ToString();
           return this;
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery StartKey(object value=null)
        {
            if(value!=null)
                Options["startkey"] = MakeValidJson(value);
            return this;
        }

        private string MakeValidJson(object value)
        {
            if (value == null) return "null";
            var s = JToken.FromObject(value).ToString();
            try
            {
                var jt = JToken.Parse(s);
                return s;
            }
            catch (Exception)
            {
                if(value is string)
                    return System.String.Format("\"{0}\"", value);
                return Convert.ToString(value);
            }
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery StartKey(params object[] value)
        {
            if(value!=null)
                Options["startkey"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Starts the key document identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery StartKeyDocumentId(string value)
        {
            if(!System.String.IsNullOrWhiteSpace(value))
                Options["startkey_docid"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery EndKey(object value)
        {
            if(value!=null)
                Options["endkey"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Any valid JSON value is a valid key. This means:
        /// null, true, false, a string, a number, a Dictionary (JSON object) or an array (JSON array)
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery EndKey(params object[] value)
        {
            if(value!=null)
                Options["endkey"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Ends the key document identifier.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery EndKeyDocumentId(string value)
        {
            if(!System.String.IsNullOrWhiteSpace(value))
                Options["endkey_docid"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Limits the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Limit(int value)
        {
            if(value>0)
                Options["limit"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Stales this instance.
        /// </summary>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Stale(bool doStale=true)
        {
            if(doStale)
                Options["stale"] = "ok";
            return this;
        }

        /// <summary>
        /// Descendings this instance.
        /// </summary>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Descending(bool doDescending=true)
        {
            if(doDescending)
                Options["descending"] = "true";
            return this;
        }

        /// <summary>
        /// Skips the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Skip(int value)
        {
            if(value>0)
                Options["skip"] = MakeValidJson(value);
            return this;
        }

        /// <summary>
        /// Groups the specified do group.
        /// </summary>
        /// <param name="doGroup">if set to <c>true</c> [do group].</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Group(bool doGroup=false)
        {
            if (doGroup)
                Options["group"] = Convert.ToString(doGroup);
            return this;
        }

        /// <summary>
        /// Groups the level.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery GroupLevel(int value)
        {
            Options["group_level"] = Convert.ToString(value);
            return this;
        }

        /// <summary>
        /// Reduces the specified do reduce.
        /// </summary>
        /// <param name="doReduce">if set to <c>true</c> [do reduce].</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery Reduce(bool doReduce=true)
        {
            if(doReduce)
                Options["reduce"] = MakeValidJson(true);
            return this;
        }

        /// <summary>
        /// Includes the documents.
        /// </summary>
        /// <param name="doIncludeDocs">if set to <c>true</c> [do include docs].</param>
        /// <returns>CouchQuery.</returns>
        public BigDQuery IncludeDocuments(bool doIncludeDocs=true)
        {
            if(doIncludeDocs)
                Options["include_docs"] = Convert.ToString(true);
            return this;
        }

        /// <summary>
        /// Tell this query to do a HEAD request first to see
        /// if ETag has changed and only then do the full request.
        /// This is only interesting if you are reusing this query object.
        /// </summary>
        /// <returns>CouchQuery.</returns>
        public BigDQuery CheckETagUsingHead()
        {
            checkETagUsingHead = true;
            return this;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <returns>CouchGenericViewResult.</returns>
        public BigDGenericViewResult GetResult()
        {
            try
            {
                return GetResult<BigDGenericViewResult>();
            }
            catch (WebException e)
            {
                throw BigDException.Create("Query failed", e);
            } 
        }

        /// <summary>
        /// Determines whether [is cached and valid].
        /// </summary>
        /// <returns><c>true</c> if [is cached and valid]; otherwise, <c>false</c>.</returns>
        public bool IsCachedAndValid()
        {
            // If we do not have a result it is not cached
            if (Result == null)
            {
                return false;
            }
            IBigDRequest req = View.Request().QueryOptions(Options);
            req.Etag(Result.etag);
            return req.Head().Send().IsETagValid();
        }


        /// <summary>
        /// Strings this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        public string String()
        {
            return Request().String();
        }


        /// <summary>
        /// Requests this instance.
        /// </summary>
        /// <returns>ICouchRequest.</returns>
        public IBigDRequest Request()
        {
            var req = View.Request().QueryOptions(Options);
            if (postData != null)
            {
                req.Data(postData).Post();
            }
            return req;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        public T GetResult<T>() where T : BigDViewResult, new()
        {
            var req = Request();

            if (Result == null)
            {
                Result = new T();
            }
            else
            {
                // Tell the request what we already have
                req.Etag(Result.etag);
                if (checkETagUsingHead)
                {
                    // Make a HEAD request to avoid transfer of data
                    if (req.Head().Send().IsETagValid())
                    {
                        return (T) Result;
                    }
                    // Set back to GET before proceeding below
                    req.Get();
                }
            }

            JObject json;
            try
            {
                json = req.Parse();
            }
            catch(WebException e)
            {
                throw BigDException.Create("Query failed", e);
            }
            
            if (json != null) // ETag did not match, view has changed
            {
                Result.Result(json);
                Result.etag = req.Etag();
            }
            return (T) Result;
        }

        /// <summary>
        /// Streams the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>CouchViewResultStream&lt;T&gt;.</returns>
        public CouchViewResultStream<T> StreamResult<T>() where T: ICanJson, new()
        {
            try
            {
                return new CouchViewResultStream<T>(Request().Stream());
            }
            catch (WebException e)
            {
                throw BigDException.Create("Query failed", e);
            }
        }

    }
}