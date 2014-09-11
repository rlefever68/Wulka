using System;
using System.Runtime.Serialization;

namespace Wulka.Exceptions
{
    /// <summary>
    /// Base exception for all Wulka exceptions
    /// </summary>
    public class WulkaException : System.ApplicationException
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="WulkaException"/> class.
        /// </summary>
		public WulkaException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WulkaException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public WulkaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WulkaException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public WulkaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WulkaException"/> class.
        /// </summary>
        /// <param name="serializationInfo">The serialization info.</param>
        /// <param name="streamingContext">The streaming context.</param>
        public WulkaException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }
 
	    #endregion   

    }
}
