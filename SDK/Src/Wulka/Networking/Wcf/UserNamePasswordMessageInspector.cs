// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="UserNamePasswordMessageInspector.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.ServiceModel.Channels;
using System.Text;
using Wulka.Domain.Authentication;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class UserNamePasswordMessageInspector.
    /// </summary>
    public class UserNamePasswordMessageInspector : IUserNameMessageInspector
    {
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName { get; set; }
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }



        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public CredentialsTypeEnum Type
        {
            get
            {
                return  CredentialsTypeEnum.UserNamePassword;
            }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="UserNamePasswordMessageInspector"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="pwd">The password.</param>
        public UserNamePasswordMessageInspector(string user, string pwd)
        {
            UserName = user;
            Password = pwd;
        }

        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {}

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns>The object that is returned as the <paramref name="correlationState " />argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)" /> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid" /> to ensure that no two <paramref name="correlationState" /> objects are the same.</returns>
        public object BeforeSendRequest(ref Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest;
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            else
            {
                httpRequest = new HttpRequestMessageProperty();
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }

            string WulkaaBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}", UserName, Password)));

            httpRequest.Headers["Authorization"] = string.Format("Wulkaa {0}", WulkaaBase64);
            return null;
        }

        #endregion

        
    }
}
