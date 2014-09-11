using System;
using Wulka.ErrorHandling;

namespace Wulka.Exceptions
{
    public class ApplicationException : WulkaException
    {
        private readonly ErrorInfo _errorInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="errorInfo">The error info to be contained in this exception.</param>
        public ApplicationException(ErrorInfo errorInfo)
            : base(errorInfo.Message)
        {
            _errorInfo = errorInfo;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationException"/> class.
        /// </summary>
        /// <param name="errorInfo">The error info to be contained in this exception.</param>
        /// <param name="innerException">The inner exception.</param>
        public ApplicationException(ErrorInfo errorInfo, Exception innerException)
            : base(errorInfo.Message, innerException)
        {
            _errorInfo = errorInfo;
        }

        /// <summary>
        /// Gets the error info contained in this exception.
        /// </summary>
        /// <value>The error info contained in this exception.</value>
        public ErrorInfo ErrorInfo
        {
            get
            {
                return _errorInfo;
            }
        }
    }
}
