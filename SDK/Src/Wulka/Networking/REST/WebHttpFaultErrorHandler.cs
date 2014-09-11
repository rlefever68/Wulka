// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 12-31-2013
//
// Last Modified By : Rafael Lefever
// Last Modified On : 12-31-2013
// ***********************************************************************
// <copyright file="WebHttpFaultErrorHandler.cs" company="Broobu Systems Ltd.">
//     Copyright (c) Broobu Systems Ltd.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization.Json;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Wulka.Networking.Rest
{
    /// <summary>
    /// Class WebHttpFaultErrorHandler.
    /// </summary>
    public class WebHttpFaultErrorHandler : IErrorHandler
    {
        #region IErrorHandler Members

        /// <summary>
        /// Enables error-related processing and returns a value that indicates whether the dispatcher aborts the session and the instance context in certain cases.
        /// </summary>
        /// <param name="error">The exception thrown during processing.</param>
        /// <returns>true if  should not abort the session (if there is one) and instance context if the instance context is not <see cref="F:System.ServiceModel.InstanceContextMode.Single" />; otherwise, false. The default is false.</returns>
        public bool HandleError(Exception error)
        {
            return true;
        }

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1" /> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception" /> object thrown in the course of the service operation.</param>
        /// <param name="version">The SOAP version of the message.</param>
        /// <param name="fault">The <see cref="T:System.ServiceModel.Channels.Message" /> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            fault = GetJsonFaultMessage(version, error);
            this.ApplyJsonSettings(ref fault);
            this.ApplyHttpResponseSettings(ref fault, System.Net.HttpStatusCode.BadRequest, "Error");
        }

        /// <summary>
        /// Applies the json settings.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// Apply Json settings to the message
        protected virtual void ApplyJsonSettings(ref Message fault)
        {
            // Use JSON encoding  
            var jsonFormatting =
              new WebBodyFormatMessageProperty(WebContentFormat.Json);
            fault.Properties.Add(WebBodyFormatMessageProperty.Name, jsonFormatting);
        }

        /// <summary>
        /// Applies the HTTP response settings.
        /// </summary>
        /// <param name="fault">The fault.</param>
        /// <param name="statusCode">The status code.</param>
        /// <param name="statusDescription">The status description.</param>
        /// Get the HttpResponseMessageProperty
        protected virtual void ApplyHttpResponseSettings(
          ref Message fault, System.Net.HttpStatusCode statusCode,
          string statusDescription)
        {
            var httpResponse = new HttpResponseMessageProperty()
            {
                StatusCode = statusCode,
                StatusDescription = statusDescription

            };
            httpResponse.Headers.Add(HttpResponseHeader.ContentType, "application/json");
            fault.Properties.Add(HttpResponseMessageProperty.Name, httpResponse);
        }

        /// <summary>
        /// Gets the json fault message.
        /// </summary>
        /// <param name="version">The version.</param>
        /// <param name="error">The error.</param>
        /// <returns>Message.</returns>
        protected virtual Message GetJsonFaultMessage(MessageVersion version, Exception error)
        {
            BaseFault detail = null;
            var knownTypes = new List<Type>();
            string faultType = error.GetType().Name; //default  

            if ((error is FaultException) &&
                (error.GetType().GetProperty("Detail") != null))
            {
                detail =
                  (error.GetType().GetProperty("Detail").GetGetMethod().Invoke(
                   error, null) as BaseFault);
                if (detail != null)
                {
                    knownTypes.Add(detail.GetType());
                    faultType = detail.GetType().Name;
                }
            }

            WebHttpFault jsonFault = new WebHttpFault
            {
                Message = error.Message,
                Detail = detail,
                FaultType = faultType
            };

            var faultMessage = Message.CreateMessage(version, "", jsonFault,
              new DataContractJsonSerializer(jsonFault.GetType(), knownTypes));

            return faultMessage;
        }

        #endregion
    }
}
