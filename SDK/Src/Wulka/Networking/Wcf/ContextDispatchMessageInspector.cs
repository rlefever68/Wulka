using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NLog;
using Wulka.Configuration;
using Wulka.Core;
using Wulka.Domain;
using Wulka.Domain.Authentication;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    internal class ContextDispatchMessageInspector : IDispatchMessageInspector
    {
        private const string Ns = ServiceConst.ContextNamespace;


        private readonly Logger _logger = LogManager.GetCurrentClassLogger();




        #region IDispatchMessageInspector Members

        /// <summary>
        /// Called after an inbound message has been received but before the message is dispatched to the intended operation.
        /// </summary>
        /// <param name="request">The request message.</param>
        /// <param name="channel">The incoming channel.</param>
        /// <param name="instanceContext">The current service instance.</param>
        /// <returns>
        /// The object used to correlate state. This object is passed back in the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.BeforeSendReply(System.ServiceModel.Channels.Message@,System.Object)"/> method.
        /// </returns>
        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var ctxt = MessageInspectorHelper.DecodeWulkaContext(ref request);
            if (channel != null)
            {
                var listenUri = channel.LocalAddress.ToString().ToLower();
                if (listenUri.Contains("/mex")) return null;
                //_logger.Debug("Received message on {0}", listenUri);
            }
                if (IsValidatedSession(ctxt))
                {
                    var usr = ctxt[WulkaContextKey.UserName];
                    var sess = ctxt[WulkaContextKey.SessionId];
                    var app  = ctxt[WulkaContextKey.ServiceCode];
                    if (!CheckSession(usr, sess))
                    {
                        _logger.Info("User {0} could not be validated against session {1}", usr, sess);
                    }
                }
            WulkaContext.Current = ctxt;
            return null;
        }

        /// <summary>
        /// Determines whether [is validated session].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [is validated session]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidatedSession(WulkaContext ctxt)
        {
            if (ctxt != null)
            {
                return ctxt.Keys.Contains(WulkaContextKey.UserName) &&
                    ctxt.Keys.Contains(WulkaContextKey.SessionId);
            }
            return false;
        }

        /// <summary>
        /// Checks the session.
        /// </summary>
        private bool CheckSession(string userName, string sessionId)
        {
            //_logger.Info("Checking session for User: {0} - Session: {1}", userName, sessionId);
            var res = true;
            // TODO : This requires further debugging abandoned due to time constraints, the issue being that apperently user contexts are kept in the chain
            try
            {
                if (ConfigurationHelper.IsSessionValidationActive)
                {

                    res = ValidateSessionPortal
                        .Agent
                        .Validate(userName, sessionId);

                }
                if(res)
                   _logger.Info("[{0} - {1}] <== VALID SESSION", userName, sessionId);
            }
            catch (Exception ex)
            {
                _logger.Error("Session Check for user [{0}] session [{1}] has failed.\n Error:{2}", userName, sessionId, ex.GetCombinedMessages());
                res = false;
            }
            return res;
        }

        /// <summary>
        /// Called after the operation has returned but before the reply message is sent.
        /// </summary>
        /// <param name="reply">The reply message. This value is null if the operation is one way.</param>
        /// <param name="correlationState">The correlation object returned from the <see cref="M:System.ServiceModel.Dispatcher.IDispatchMessageInspector.AfterReceiveRequest(System.ServiceModel.Channels.Message@,System.ServiceModel.IClientChannel,System.ServiceModel.InstanceContext)"/> method.</param>
        public void BeforeSendReply(ref Message reply, object correlationState)
        {
            if (WulkaContext.Current != null)
            {
                MessageInspectorHelper.EncodeWulkaContext(WulkaContext.Current, ref reply);
            }
        }

        #endregion
    }
}