// ***********************************************************************
// Assembly         : Broobu.SessionProxy.Contract
// Author           : Rafael Lefever
// Created          : 08-07-2014
//
// Last Modified By : Rafael Lefever
// Last Modified On : 08-07-2014
// ***********************************************************************
// <copyright file="WulkaSession.cs" company="Broobu">
//     Copyright (c) Broobu. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Wulka.Authentication;
using Wulka.Domain.Base;
using Wulka.Validation;

namespace Wulka.Domain.Authentication
{
    /// <summary>
    /// Class WulkaSession.
    /// </summary>
    [DataContract]
    public class WulkaSession : DomainObject<WulkaSession>
    {
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        /// <value>The username.</value>
        [DataMember]
        public string Username { get; set; }
        /// <summary>
        /// Gets or sets the account identifier.
        /// </summary>
        /// <value>The account identifier.</value>
        [DataMember]
        public string AccountId { get; set; }
        /// <summary>
        /// Gets or sets the connection time.
        /// </summary>
        /// <value>The connection time.</value>
        [DataMember]
        public DateTime ConnectionTime { get; set; }
        /// <summary>
        /// Gets or sets the last request.
        /// </summary>
        /// <value>The last request.</value>
        [DataMember]
        public DateTime LastRequest { get; set; }
        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>The first name.</value>
        [DataMember]
        public string FirstName { get; set; }
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>The last name.</value>
        [DataMember]
        public string LastName { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this instance is known.
        /// </summary>
        /// <value><c>true</c> if this instance is known; otherwise, <c>false</c>.</value>
        [DataMember]
        public bool IsKnown { get; set; }
        /// <summary>
        /// Gets or sets the authentication mode.
        /// </summary>
        /// <value>The authentication mode.</value>
        [DataMember]
        public string AuthenticationMode { get; set; }
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        [DataMember]
        public string Host { get; set; }

        /// <summary>
        /// The _current
        /// </summary>
        private static WulkaSession _current;
        /// <summary>
        /// Gets or sets the current.
        /// </summary>
        /// <value>The current.</value>
        public static WulkaSession Current
        {
        		get
        		{
        				return _current;
        		}
        		set
        		{
        				_current = value;
        		}
        }

        /// <summary>
        /// Validates the specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>System.String.</returns>
        protected override string Validate(string columnName)
        {
            return DataErrorInfoValidator<WulkaSession>.Validate(this, columnName);
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns>ICollection&lt;System.String&gt;.</returns>
        protected override ICollection<string> Validate()
        {
            return DataErrorInfoValidator<WulkaSession>.Validate(this);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is default session.
        /// </summary>
        /// <value><c>true</c> if this instance is default session; otherwise, <c>false</c>.</value>
        public bool IsDefaultSession 
        {
            get
            {
                return (Username == AuthenticationDefaults.GuestUserName);
            }
        }

        /// <summary>
        /// Gets or sets the application function identifier.
        /// </summary>
        /// <value>The application function identifier.</value>
        [DataMember]     
        public string ApplicationFunctionId { get; set; }


        /// <summary>
        /// The _role identifier
        /// </summary>
        private string _roleId;
        /// <summary>
        /// Gets or sets the role identifier.
        /// </summary>
        /// <value>The role identifier.</value>
        [DataMember]
        public string RoleId
        {
            get
            {
                if (String.IsNullOrWhiteSpace(_roleId))
                    return SysConst.NullGuid;
                return _roleId;
            }
            set
            {
                _roleId = value;
            }
        }



        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        public CredentialsBase Credentials 
        {
            get 
            {
                return new ExtendedCredentials(Username, FirstName, LastName, SessionId, ApplicationFunctionId);
            }
        }


 



    }
}