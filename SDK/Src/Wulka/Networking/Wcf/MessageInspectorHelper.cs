// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="MessageInspectorHelper.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using NLog;
using Wulka.Core;
using Wulka.Domain;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class MessageInspectorHelper.
    /// </summary>
    public class MessageInspectorHelper
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Decodes the Wulka context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>WulkaContext.</returns>
        public static WulkaContext DecodeWulkaContext(ref Message message)
        {
            //Logger.Info("Decoding Wulka Context...");
            var ctx = WulkaContext.Current ?? new WulkaContext();
            var headers = message.Headers.Where(h => h.Namespace == ServiceConst.ContextNamespace).ToList();
            foreach (var messageHeaderInfo in headers)
            {
                var position = message.Headers.FindHeader(messageHeaderInfo.Name, messageHeaderInfo.Namespace);
                var reader = message.Headers.GetReaderAtHeader(position);
                var value = reader.ReadElementString();
                ctx.Add(messageHeaderInfo.Name, value);
                //Logger.Info("\tParsed Header: {0} - Value:\t {1}", messageHeaderInfo.Name, value);
            }
            //Logger.Info("Decoded Wulka Context.");
            return ctx;
        }



        /// <summary>
        /// Encodes the Wulka context.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="channel">The channel.</param>
        /// <returns>System.Object.</returns>
        public static object EncodeWulkaContext(ref Message request, IClientChannel channel)
        {
            foreach (var WulkaContextHeader in channel.GetWulkaContextHeaders())
            {
                request.Headers.Add(WulkaContextHeader);
            }
            return null;
        }




        /// <summary>
        /// Encodes the Wulka context.
        /// </summary>
        /// <param name="ctx">The CTX.</param>
        /// <param name="reply">The reply.</param>
        internal static void EncodeWulkaContext(WulkaContext ctx, ref Message reply)
        {
            foreach (string key in ctx.Keys)
            {
                reply.Headers.Add(MessageHeader.CreateHeader(key, ServiceConst.ContextNamespace, ctx[key]));
            }
        }
    }
}
