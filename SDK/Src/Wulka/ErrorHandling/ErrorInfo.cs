using System;
using NLog;
using Wulka.Domain.Base;

namespace Wulka.ErrorHandling
{
    /// <summary>
    /// Maintains information associated with a particular error situation
    /// </summary>
    public class ErrorInfo
    {
        private readonly int _errorNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorInfo"/> class.
        /// </summary>
        /// <param name="errorType">Type of the error.</param>
        /// <param name="facilityInfo">The facility info.</param>
        /// <param name="errorCode">The error code, must be unique for the given facility.</param>
        /// <param name="message">The error message.</param>
        /// <param name="args">The arguments for the error message, optional.</param>
        public ErrorInfo(ErrorType errorType, FacilityInfo facilityInfo, int errorCode, string message, params object[] args)
        {
            ErrorTypeValue = errorType;
            FacilityInfo = facilityInfo;
            ErrorCode = errorCode;
            _errorNumber = BuildErrorNumber(facilityInfo.Code, errorCode, errorType);
            Message = message;
            Arguments = args;
        }

        private static int BuildErrorNumber(FacilityCode facilityCode, int errorCode, ErrorType errorType)
        {
            //
            //  Error values are 32 bit values laid out as follows:
            //
            //   3 3 2 2 2 2 2 2 2 2 2 2 1 1 1 1 1 1 1 1 1 1
            //   1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0 9 8 7 6 5 4 3 2 1 0
            //  +---+-+-+-----------------------+-------------------------------+
            //  |Sev|C|R|     Facility          |               Code            |
            //  +---+-+-+-----------------------+-------------------------------+
            //
            //  where
            //
            //      Sev - is the severity code
            //
            //          00 - Success
            //          01 - Informational
            //          10 - Warning
            //          11 - Error
            //
            //      C - is the Customer code flag
            //
            //      R - is a reserved bit
            //
            //      Facility - is the facility code
            //
            //      Code - is the facility's status code
            //
            int errorNumber;
            unchecked // use unchecked to suppress overflow checking 
            {
                int isError = (int)errorType;
                const int isCustomerValue = 0x20000000;
                errorNumber = isError
                            | isCustomerValue
                            | (0x0FFF0000 & ((int)facilityCode << 16))
                            | (0x0000FFFF & errorCode);
            }
            return errorNumber;
        }

        /// <summary>
        /// Gets the error group info.
        /// </summary>
        /// <value>The error group info.</value>
        public ErrorType        ErrorTypeValue    { get; private set; }

        /// <summary>
        /// Gets the error group info.
        /// </summary>
        /// <value>The error group info.</value>
        public FacilityInfo     FacilityInfo    { get; private set; }
        
        /// <summary>
        /// Gets the error code associated with this error.  This code is unique
        /// within a facility.
        /// </summary>
        /// <value>The error code.</value>
        public int              ErrorCode       { get; private set; }
        
        /// <summary>
        /// Gets the error number associated with this error.  The error number is 
        /// unique over all facilities 
        /// </summary>
        /// <value>The error number.</value>
        public int Number
        {
            get
            {
                return _errorNumber;
            }
        }
        
        /// <summary>
        /// Gets the error message (unformatted).
        /// </summary>
        /// <value>The message.</value>
        public string           Message         { get; private set; }
        
        /// <summary>
        /// Gets the arguments for the error message.
        /// </summary>
        /// <value>The arguments.</value>
        public object[]         Arguments       { get; private set; }

        /// <summary>
        /// Logs the error contained in this instance to the specified logger.
        /// </summary>
        /// <param name="caller">The Type of the caller, used as log name.</param>
        [Obsolete]
        public void Log(Type caller)
        {
            // obsolete
        }

        /// <summary>
        /// Logs the error contained in this instance to the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public void Log(Logger logger)
        {
            switch (ErrorTypeValue)
            {
                case ErrorType.Warning:
                    logger.Warn(ToString());
                    break;
                case ErrorType.Error:
                    logger.Error(ToString());
                    break;
                default:
                    logger.Info(ToString());
                    break;
            }
        }

        /// <summary>
        /// Logs the error contained in this instance to the specified logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <returns>Returns this instance of error info</returns>
        public ErrorInfo LogTo(Logger logger)
        {
            Log(logger);
            return this;
        }

        /// <summary>
        /// Adds this instance to the specified extended result.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns>Returns this instance of error info</returns>
        public ErrorInfo AddTo(Result result)
        {
            result.Add(this);
            return this;
        }

        /// <summary>
        /// Logs the error contained in this instance to the specified logger and logs the associated exception
        /// </summary>
        /// <param name="caller">The Type of the caller, used as log name.</param>
        /// <param name="ex">The exception to be logged.</param>
        [Obsolete]
        public void LogError(Type caller, Exception ex)
        {
           // obsolete
        }

        /// <summary>
        /// Logs the error contained in this instance to the specified logger and logs the associated exception
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="ex">The exception to be logged.</param>
        public void LogError(Logger logger, Exception ex)
        {
            logger.Error(ToString(), ex);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="ErrorInfo"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="ErrorInfo"/>.
        /// </returns>
        public override string  ToString()
        {
            string message = string.Format(Message, Arguments);
 	        return string.Format("{0} 0x{1} ({2},{3},{4}): {5}",
                ErrorTypeToString(ErrorTypeValue),
                _errorNumber.ToString("X8"),
                _errorNumber,
                FacilityInfo.Code,
                ErrorCode,
                message);
        }

        /// <summary>
        /// Converts an error type to a string.
        /// </summary>
        /// <param name="errorType">Error type to be converted.</param>
        /// <returns>The string value representing the error type</returns>
        private static string ErrorTypeToString(ErrorType errorType)
        {
            switch(errorType)
            {
                case ErrorType.Success:
                    return "SUCCESS";
                case ErrorType.Informational:
                    return "INFORMATIONAL";
                case ErrorType.Warning:
                    return "WARNING";
                case ErrorType.Error:
                    return "ERROR";
                default:
                    throw new ArgumentOutOfRangeException("errorType", errorType, "Unexpected value encountered for error type");
            }
        }
    }
}
