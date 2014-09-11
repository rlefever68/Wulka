// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="IUserNameMessageInspector.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System.ServiceModel.Dispatcher;
using Wulka.Domain.Authentication;

namespace Wulka.Networking.Wcf.Interfaces
{
    /// <summary>
    /// Interface IUserNameMessageInspector
    /// </summary>
    public interface IUserNameMessageInspector : IClientMessageInspector
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>The type.</value>
        CredentialsTypeEnum Type { get; }
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        string UserName { get; set; }
    }
}
