// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="ExtendedMessageInspector.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Security.Cryptography;
using System.ServiceModel.Channels;
using System.Text;
using Wulka.Authentication;
using Wulka.Domain.Authentication;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class ExtendedMessageInspector.
    /// </summary>
    public class ExtendedMessageInspector : IUserNameMessageInspector
    {
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public string Session { get; set; }
        /// <summary>
        /// The _service code
        /// </summary>
        private string _serviceCode;
        /// <summary>
        /// Gets or sets the service code.
        /// </summary>
        /// <value>The service code.</value>
        public string ServiceCode
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_serviceCode))
                    _serviceCode = AuthenticationDefaults.ServiceCode;
                return _serviceCode;
            }
            set
            {
                _serviceCode = value;
            }
        }



        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        public CredentialsTypeEnum Type
        {
            get { return CredentialsTypeEnum.UserNameSession; }
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedMessageInspector"/> class.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="session">The session.</param>
        /// <param name="serviceCode">The service code.</param>
        public ExtendedMessageInspector(string user, string firstName, string lastName, string session, string serviceCode)
        {
            UserName = user;
            FirstName = firstName;
            LastName = lastName;
            Session = session;
            ServiceCode = serviceCode;
        }

        #region IClientMessageInspector Members

        /// <summary>
        /// Enables inspection or modification of a message after a reply message is received but prior to passing it back to the client application.
        /// </summary>
        /// <param name="reply">The message to be transformed into types and handed back to the client application.</param>
        /// <param name="correlationState">Correlation state data.</param>
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {}

        /// <summary>
        /// Enables inspection or modification of a message before a request message is sent to a service.
        /// </summary>
        /// <param name="request">The message to be sent to the service.</param>
        /// <param name="channel">The WCF client object channel.</param>
        /// <returns>The object that is returned as the <paramref name="correlationState " />argument of the <see cref="M:System.ServiceModel.Dispatcher.IClientMessageInspector.AfterReceiveReply(System.ServiceModel.Channels.Message@,System.Object)" /> method. This is null if no correlation state is used.The best practice is to make this a <see cref="T:System.Guid" /> to ensure that no two <paramref name="correlationState" /> objects are the same.</returns>
        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            HttpRequestMessageProperty httpRequest;
            if (request.Properties.ContainsKey(HttpRequestMessageProperty.Name))
                httpRequest = request.Properties[HttpRequestMessageProperty.Name] as HttpRequestMessageProperty;
            else
            {
                httpRequest = new HttpRequestMessageProperty();
                request.Properties.Add(HttpRequestMessageProperty.Name, httpRequest);
            }

            string hash = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(ServiceCode)).ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}{1}{2}", UserName, FirstName, LastName))));
//            string hash = Convert.ToBase64String(new HMACSHA256(Encoding.UTF8.GetBytes(ServiceCode)).ComputeHash(Encoding.UTF8.GetBytes(string.Format("{0}", UserName))));

            httpRequest.Headers["Authorization"] = string.Format("Wulkaas {0}:{1}", Session, hash);
            return null;
        }

        #endregion
    }
}
