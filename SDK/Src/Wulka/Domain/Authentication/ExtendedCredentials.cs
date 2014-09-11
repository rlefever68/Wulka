// ***********************************************************************
// Assembly         : Wulka.Fx
// Author           : Rafael Lefever
// Created          : 02-03-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="ExtendedCredentials.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
namespace Wulka.Domain.Authentication
{
    /// <summary>
    /// Class ExtendedCredentials.
    /// </summary>
    public class ExtendedCredentials : UserNameSessionCredentials
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
        /// Gets or sets the service code.
        /// </summary>
        /// <value>The service code.</value>
        public string ServiceCode { get; set; }




        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCredentials"/> class.
        /// </summary>
        public ExtendedCredentials()
        { }



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionId">The session identifier.</param>
        public ExtendedCredentials(string userName, string sessionId)
            :base(userName,sessionId)
        { 

        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="sessionId">The session identifier.</param>
        /// <param name="serviceCode">The service code.</param>
        public ExtendedCredentials(string userName, string sessionId, string serviceCode)
            : base(userName, sessionId)
        {
            ServiceCode = serviceCode;
        }



        /// <summary>
        /// Initializes a new instance of the <see cref="ExtendedCredentials"/> class.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="firstName">The first name.</param>
        /// <param name="lastName">The last name.</param>
        /// <param name="session">The session.</param>
        /// <param name="serviceCode">The service code.</param>
        public ExtendedCredentials(string userName, string firstName, string lastName, string session, string serviceCode)
        {
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            Session = session;
            ServiceCode = serviceCode;
        }

        /// <summary>
        /// Gets the type of the credential.
        /// </summary>
        /// <value>The type of the credential.</value>
        public override CredentialsTypeEnum CredentialType
        {
            get { return CredentialsTypeEnum.Extended; }
        }
    }

}
