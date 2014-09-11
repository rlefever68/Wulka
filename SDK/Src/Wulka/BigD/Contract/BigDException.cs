// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-17-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-17-2013
// ***********************************************************************
// <copyright file="CouchException.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Wulka.BigD.Contract
{
    /// <summary>
    /// Class BigDException.
    /// </summary>
    [Serializable]
    public class BigDException : Exception
    {
        /// <summary>
        /// The status code
        /// </summary>
        public HttpStatusCode StatusCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="BigDException"/> class.
        /// </summary>
        public BigDException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BigDException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public BigDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception" /> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BigDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// Creates the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>Exception.</returns>
        public static Exception Create(string message)
        {
            return new BigDException(message);
        }

        /// <summary>
        /// Creates the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="e">The e.</param>
        /// <returns>Exception.</returns>
        public static Exception Create(string message, WebException e)
        {
            string msg = string.Format(CultureInfo.InvariantCulture, message + ": {0}", e.Message);
            if (e.Response != null)
            {
                var webResponse = (HttpWebResponse) e.Response;
                // Pick out status code
                HttpStatusCode code = webResponse.StatusCode;
                using (var stream = new JsonTextReader(new StreamReader(webResponse.GetResponseStream())))
                {
                    // if we don't get a valid {error:, reason:}, don't worry about it
                    try
                    {
                        var error = JToken.ReadFrom(stream);
                        msg += String.Format(CultureInfo.InvariantCulture, " error: {0}, reason: {1}", error.Value<string>("error"), error.Value<string>("reason"));
                    }
                    catch
                    { }

                    // Create any specific exceptions we care to use
                    if (code == HttpStatusCode.Conflict)
                    {
                        return new BigDConflictException(msg, e);
                    }
                    if (code == HttpStatusCode.NotFound)
                    {
                        return new BigDNotFoundException(msg, e);
                    }
                }
            }

            // Fall back on generic CouchException
            return new BigDException(msg, e);
        }
    }
}