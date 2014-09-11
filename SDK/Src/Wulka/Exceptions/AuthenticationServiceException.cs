using System;
using System.Runtime.Serialization;

namespace Wulka.Exceptions
{
    public enum Reason
    {
        [EnumMember]
        DatabaseUnavailable,

        [EnumMember]
        Unknown
    }

    /// <summary>
    /// Exposes Methods and Properties that will define an Authentication Service Exception
    /// </summary>
    [Serializable]
    public class AuthenticationServiceException : WulkaException
    {
        /// <summary>
        /// Gets or sets the reason this exception was thrown.
        /// </summary>
        /// <value>The reason.</value>
        [DataMember]
        public Reason Reason { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationServiceException"/> class.
        /// </summary>
        public AuthenticationServiceException(Reason reason)
        {
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationServiceException"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="message">The message.</param>
        public AuthenticationServiceException(Reason reason, string message)
            : base(message)
        {
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationServiceException"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AuthenticationServiceException(Reason reason, string message, Exception innerException)
            : base(message, innerException)
        {
            Reason = reason;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationServiceException"/> class.
        /// </summary>
        /// <param name="reason">The reason.</param>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected AuthenticationServiceException(Reason reason, SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Reason = reason;
        }
    }
}