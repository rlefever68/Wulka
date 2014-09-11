// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : ON8RL
// Created          : 12-11-2013
//
// Last Modified By : ON8RL
// Last Modified On : 12-11-2013
// ***********************************************************************
// <copyright file="SessionFactory.cs" company="">
//     Copyright (c) . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using Wulka.Authentication;

namespace Wulka.Domain.Authentication
{
    /// <summary>
    /// Class SessionFactory.
    /// </summary>
    public class SessionFactory
    {
        /// <summary>
        /// Creates the default Wulka session.
        /// </summary>
        /// <returns>WulkaSession.</returns>
        public static WulkaSession CreateDefaultWulkaSession()
        {
            var sid = Guid.NewGuid().ToString();
            return new WulkaSession()
            {
                Id = sid,
                Username = AuthenticationDefaults.GuestUserName,
                SessionId = sid,
                AuthenticationMode = AuthenticationDefaults.AuthModeId,
                ApplicationFunctionId = AuthenticationDefaults.ServiceCode
            };
        }

    }
}
