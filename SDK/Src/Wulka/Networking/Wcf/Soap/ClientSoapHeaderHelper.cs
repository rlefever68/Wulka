// ***********************************************************************
// Assembly         : Wulka
// Author           : Rafael Lefever
// Created          : 01-10-2012
//
// Last Modified By : Rafael Lefever
// Last Modified On : 01-10-2012
// ***********************************************************************
// <copyright file="ClientSoapHeaderHelper.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.ServiceModel;

namespace Wulka.Networking.Wcf.Soap
{
    /// <summary>
    /// Class ClientSoapHeaderHelper.
    /// </summary>
    public static class ClientSoapHeaderHelper
    {
        /// <summary>
        /// Sets the header.
        /// </summary>
        /// <param name="channel">The channel.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="value">The value.</param>
        public static void SetHeader(this IClientChannel channel, string headerName, object value)
        {
            channel.Extensions.Find<SoapHeadersClientHook>().Headers[headerName] = value;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel">The channel.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <returns>T.</returns>
        public static T GetHeader<T>(this IClientChannel channel, string headerName) where T : class
        {
            return (T)channel.Extensions.Find<SoapHeadersClientHook>().Headers[headerName];
        }
    }
}
