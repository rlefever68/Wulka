// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="WulkaUPCredentials.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Wulka.Domain.Authentication
{


    /// <summary>
    /// Class WulkaUpCredentials.
    /// This class represents Username/Password Credentials
    /// </summary>
    public class UserNamePasswordCredentials : CredentialsBase
    {
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password { get; set; }


        /// <summary>
        /// Gets the type of the credential.
        /// </summary>
        /// <value>The type of the credential.</value>
        public override  CredentialsTypeEnum CredentialType
        {
            get { return CredentialsTypeEnum.UserNamePassword; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserNamePasswordCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        public UserNamePasswordCredentials(string userName, string password)
        {
            UserName = userName;
            Password = password;
        }
        
    }

}
