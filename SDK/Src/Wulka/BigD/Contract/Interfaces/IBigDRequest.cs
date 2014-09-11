// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 07-20-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 07-20-2014
// ***********************************************************************
// <copyright file="IBigDRequest.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Wulka.BigD.Contract.Interfaces
{
    /// <summary>
    /// Interface IBigDRequest
    /// </summary>
    public interface IBigDRequest
    {
        /// <summary>
        /// Adds the header.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest AddHeader(string key, string value);
        /// <summary>
        /// Checks the specified message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Check(string message);
        /// <summary>
        /// Copies this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Copy();
        /// <summary>
        /// Datas the specified data stream.
        /// </summary>
        /// <param name="dataStream">The data stream.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Data(System.IO.Stream dataStream);
        /// <summary>
        /// Datas the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Data(string data);
        /// <summary>
        /// Datas the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Data(byte[] data);
        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Delete();
        /// <summary>
        /// Etags this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        string Etag();
        /// <summary>
        /// Etags the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Etag(string value);
        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Get();
        /// <summary>
        /// Heads this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Head();
        /// <summary>
        /// Determines whether [is e tag valid].
        /// </summary>
        /// <returns><c>true</c> if [is e tag valid]; otherwise, <c>false</c>.</returns>
        bool IsETagValid();
        /// <summary>
        /// MIMEs the type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest MimeType(string type);
        /// <summary>
        /// MIMEs the type json.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest MimeTypeJson();
        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T Parse<T>() where T : Newtonsoft.Json.Linq.JToken;
        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>Newtonsoft.Json.Linq.JObject.</returns>
        Newtonsoft.Json.Linq.JObject Parse();
        /// <summary>
        /// Pathes the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Path(string name);
        /// <summary>
        /// Posts this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Post();
        /// <summary>
        /// Posts the json.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest PostJson();
        /// <summary>
        /// Puts this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Put();
        /// <summary>
        /// Queries the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Query(string name);
        /// <summary>
        /// Queries the options.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest QueryOptions(System.Collections.Generic.ICollection<System.Collections.Generic.KeyValuePair<string, string>> options);
        /// <summary>
        /// Responses this instance.
        /// </summary>
        /// <returns>System.Net.WebResponse.</returns>
        System.Net.WebResponse Response();
        /// <summary>
        /// Results this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>T.</returns>
        T Result<T>() where T : Newtonsoft.Json.Linq.JToken;
        /// <summary>
        /// Results this instance.
        /// </summary>
        /// <returns>Newtonsoft.Json.Linq.JObject.</returns>
        Newtonsoft.Json.Linq.JObject Result();
        /// <summary>
        /// Sends this instance.
        /// </summary>
        /// <returns>IBigDRequest.</returns>
        IBigDRequest Send();
        /// <summary>
        /// Streams this instance.
        /// </summary>
        /// <returns>Newtonsoft.Json.JsonTextReader.</returns>
        Newtonsoft.Json.JsonTextReader Stream();
        /// <summary>
        /// Strings this instance.
        /// </summary>
        /// <returns>System.String.</returns>
        string String();
    }
}
