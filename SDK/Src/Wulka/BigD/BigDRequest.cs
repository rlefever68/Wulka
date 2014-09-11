using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Wulka.BigD.Configuration;
using Wulka.BigD.Contract;
using Wulka.BigD.Contract.Interfaces;

namespace Wulka.BigD
{
    /// <summary>
    /// A CouchDB HTTP request with all its options. This is where we do the actual HTTP requests to CouchDB.
    /// </summary>
    public class BigDRequest : IBigDRequest
    {
        private const int UploadBufferSize = 100000;
        private readonly IBigDDatabase db;
        private Stream postStream;
        private readonly IBigDServer server;
        private string etag, etagToCheck;
        private readonly Dictionary<string, string> headers = new Dictionary<string, string>();

        // Query options
        private string method = "GET"; // PUT, DELETE, POST, HEAD
        private string mimeType;
        private string path;
        private string query;

        private JToken result;

        #region Contructors

        public BigDRequest(IBigDServer server)
        {
            this.server = server;
        }

        public BigDRequest(IBigDDatabase db)
        {
            server = db.Server;
            this.db = db;
        }

        #endregion

        /// <summary>
        /// Sets the e-tag value
        /// </summary>
        /// <param name="value">The e-tag value</param>
        /// <returns>A CouchRequest with the new e-tag value</returns>
        public IBigDRequest Etag(string value)
        {
            etagToCheck = value;
            headers["If-Modified"] = value;
            return this;
        }

        /// <summary>
        /// Sets the absolute path in the query
        /// </summary>
        /// <param name="name">The absolute path</param>
        /// <returns>A <see cref="BigDRequest"/> with the new path set.</returns>
        public IBigDRequest Path(string name)
        {
            path = name;
            return this;
        }

        /// <summary>
        /// Sets the raw query information in the request. Don't forget to start with a question mark (?).
        /// </summary>
        /// <param name="value">The raw query</param>
        /// <returns>A <see cref="BigDRequest"/> with the new query set.</returns>
        public IBigDRequest Query(string value)
        {
            query = value;
            return this;
        }

        public IBigDRequest QueryOptions(ICollection<KeyValuePair<string, string>> options)
        {
            if (options == null || options.Count == 0)
            {
                return this;
            }

            var sb = new StringBuilder();
            sb.Append("?");
            foreach (var q in options)
            {
                if (sb.Length > 1)
                {
                    sb.Append("&");
                }
                sb.Append(HttpUtility.UrlEncode(q.Key));
                sb.Append("=");
                sb.Append(HttpUtility.UrlEncode(q.Value));
            }

            return Query(sb.ToString());
        }

        /// <summary>
        /// Turn the request into a HEAD request, HEAD requests are problematic
        /// under Mono 2.4, but has been fixed in later releases.
        /// </summary> 
        public IBigDRequest Head()
        {
            // NOTE: We need to do this until next release of mono where HEAD requests have been fixed!
            method = server.RunningOnMono ? "GET" : "HEAD";
            return this;
        }

        public IBigDRequest Copy()
        {
            method = "COPY";
            return this;
        }

        public IBigDRequest PostJson()
        {
            MimeTypeJson();
            return Post();
        }

        public IBigDRequest Post()
        {
            method = "POST";
            return this;
        }

        public IBigDRequest Get()
        {
            method = "GET";
            return this;
        }

        public IBigDRequest Put()
        {
            method = "PUT";
            return this;
        }

        public IBigDRequest Delete()
        {
            method = "DELETE";
            return this;
        }

        public IBigDRequest Data(string data)
        {
            return Data(Encoding.UTF8.GetBytes(data));
        }

        public IBigDRequest Data(byte[] data)
        {
            postStream = new MemoryStream(data);
            return this;
        }

        public IBigDRequest Data(Stream dataStream)
        {
            postStream = dataStream;
            return this;
        }

        public IBigDRequest MimeType(string type)
        {
            mimeType = type;
            return this;
        }

        public IBigDRequest MimeTypeJson()
        {
            MimeType("application/json");
            return this;
        }

        public JObject Result()
        {
            return (JObject)result;
        }

        public T Result<T>() where T : JToken
        {
            return (T)result;
        }

        public string Etag()
        {
            return etag;
        }

        public IBigDRequest Check(string message)
        {
            try
            {
                if (result == null)
                {
                    Parse();
                }
                if (!result["ok"].Value<bool>())
                {
                    throw BigDException.Create(string.Format(CultureInfo.InvariantCulture, message + ": {0}", result));
                }
                return this;
            }
            catch (WebException e)
            {
                throw BigDException.Create(message, e);
            }
        }

        private HttpWebRequest GetRequest()
        {
            var requestUri = new UriBuilder(ConfigurationHelper.CouchDbProtocol, server.Host, server.Port, ((db != null) ? db.Name + "/" : "") + path, query).Uri;
            var request = WebRequest.Create(requestUri) as HttpWebRequest;
            if (request == null)
            {
                throw BigDException.Create("Failed to create request");
            }
            request.Timeout = 3600000; // 1 hour. May use System.Threading.Timeout.Infinite;
            request.Method = method;

            if (mimeType != null)
            {
                request.ContentType = mimeType;
            }

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            if (!string.IsNullOrEmpty(server.EncodedCredentials))
			{
                request.Headers.Add("Authorization", server.EncodedCredentials);
			}
			
            if (postStream != null)
            {
                WriteData(request);
            }

            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "Request: {0} Method: {1}", requestUri, method));
            return request;
        }

        private void WriteData(HttpWebRequest request)
        {
            request.ContentLength = postStream.Length;
            using (Stream ps = request.GetRequestStream())
            {
                var buffer = new byte[UploadBufferSize];
                int bytesRead;
                while ((bytesRead = postStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    ps.Write(buffer, 0, bytesRead);
                }
            }
        }

        public JObject Parse()
        {
            return Parse<JObject>();
        }

        public T Parse<T>() where T : JToken
        {
            using (WebResponse response = GetResponse())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        using (var textReader = new JsonTextReader(reader))
                        {
                            PickETag(response);
                            if (etagToCheck != null)
                            {
                                if (IsETagValid())
                                {
                                    return null;
                                }
                            }
                            result = JToken.ReadFrom(textReader); // We know it is a top level JSON JObject.
                        }
                    }
                }
            }

            return (T)result;
        }

        /// <summary>
        /// Returns a Json stream from the server
        /// </summary>
        /// <returns></returns>
        public JsonTextReader Stream()
        {
            return new JsonTextReader(new StreamReader(GetResponse().GetResponseStream()));
        }

        private void PickETag(WebResponse response)
        {
            etag = response.Headers["ETag"];
            if (etag != null)
            {
                etag = etag.EndsWith("\"") ? etag.Substring(1, etag.Length - 2) : etag;
            }
        }

        /// <summary>
        /// Return the request as a plain string instead of trying to parse it.
        /// </summary>
        public string String()
        {
            using (WebResponse response = GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    PickETag(response);
                    if (etagToCheck != null)
                    {
                        if (IsETagValid())
                        {
                            return null;
                        }
                    }
                    return reader.ReadToEnd();
                }
            }
        }

        public WebResponse Response()
        {
            WebResponse response = GetResponse();

            PickETag(response);
            if (etagToCheck != null)
            {
                if (IsETagValid())
                {
                    return null;
                }
            }
            return response;
        }

        private WebResponse GetResponse()
        {
            return GetRequest().GetResponse();
        }

        public IBigDRequest Send()
        {
            using (WebResponse response = GetResponse())
            {
                PickETag(response);
                return this;
            }
        }

        public bool IsETagValid()
        {
            return etagToCheck == etag;
        }

        public IBigDRequest AddHeader(string key, string value)
        {
            headers[key] = value;
            return this;
        }
    }
}