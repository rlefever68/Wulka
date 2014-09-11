// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="WulkaUsCredentials.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

namespace Wulka.Domain.Authentication
{
    /// <summary>
    /// Class WulkaUsCredentials.
    /// This class represents Username/Session Credentials
    /// </summary>
    public class UserNameSessionCredentials : CredentialsBase
    {

        /// <summary>
        /// Gets or sets the session.
        /// </summary>
        /// <value>The session.</value>
        public string Session { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameSessionCredentials"/> class.
        /// </summary>
        public UserNameSessionCredentials() 
        { 
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="UserNameSessionCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="session">The session.</param>
        public UserNameSessionCredentials(string userName, string session)
        {
            UserName = userName;
            Session = session;
        }

        /// <summary>
        /// Gets the type of the credential.
        /// </summary>
        /// <value>The type of the credential.</value>
        public override CredentialsTypeEnum CredentialType
        {
            get { return CredentialsTypeEnum.UserNameSession; }
        }
    }

}
