// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="UserNameEndpointBehavior.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ServiceModel.Description;
using Wulka.Networking.Wcf.Interfaces;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// Class UserNameEndpointBehavior.
    /// </summary>
    public class UserNameEndpointBehavior : IEndpointBehavior
    {
        /// <summary>
        /// Gets or sets the inspector.
        /// </summary>
        /// <value>The inspector.</value>
        public IUserNameMessageInspector Inspector { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameEndpointBehavior" /> class.
        /// </summary>
        /// <param name="inspector">The inspector.</param>
        public UserNameEndpointBehavior(IUserNameMessageInspector inspector)
        {
            Inspector = inspector;
        }
        #region IEndpointBehavior Members

        /// <summary>
        /// Implement to pass data at runtime to bindings to support custom behavior.
        /// </summary>
        /// <param name="endpoint">The endpoint to modify.</param>
        /// <param name="bindingParameters">The objects that binding elements require to support the behavior.</param>
        public void AddBindingParameters(ServiceEndpoint endpoint, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {}

        /// <summary>
        /// Implements a modification or extension of the client across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that is to be customized.</param>
        /// <param name="clientRuntime">The client runtime to be customized.</param>
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(Inspector);
        }

        /// <summary>
        /// Implements a modification or extension of the service across an endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint that exposes the contract.</param>
        /// <param name="endpointDispatcher">The endpoint dispatcher to be modified or extended.</param>
        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {}

        /// <summary>
        /// Implement to confirm that the endpoint meets some intended criteria.
        /// </summary>
        /// <param name="endpoint">The endpoint to validate.</param>
        public void Validate(ServiceEndpoint endpoint)
        {}

        #endregion
    }
}
