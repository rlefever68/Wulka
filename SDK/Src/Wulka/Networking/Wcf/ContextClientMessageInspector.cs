﻿using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NLog;
using Wulka.Core;
using Wulka.Domain;

namespace Wulka.Networking.Wcf
{
    internal class ContextClientMessageInspector : IClientMessageInspector
    {
        private const string Ns = ServiceConst.ContextNamespace;

        private Logger _logger = LogManager.GetCurrentClassLogger();
        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
            WulkaContext.Current = MessageInspectorHelper.DecodeWulkaContext(ref reply);
        }

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The  client object channel.</param>
        /// <returns>
        /// The object that is returned as the <paramref name="correlationState "/>argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)"/> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid"/> to ensure that no two <paramref name="correlationState"/> objects are the same.
        /// </returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            return MessageInspectorHelper.EncodeWulkaContext(ref request, channel);
        }

        #endregion
    }
}