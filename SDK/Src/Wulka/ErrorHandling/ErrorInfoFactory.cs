namespace Wulka.ErrorHandling
{
    /// <summary>
    /// Creates ErrorInfo's for the requested type.
    /// </summary>
    public static class ErrorInfoFactory
    {
        /// <summary>
        /// Creates an error info with error type error.
        /// </summary>
        /// <param name="facilityInfo">The facility info.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static ErrorInfo CreateErrorInfo(FacilityInfo facilityInfo, int errorCode, string message, params object[] args)
        {
            return new ErrorInfo(ErrorType.Error, facilityInfo, errorCode, message, args);
        }

        /// <summary>
        /// Creates an error info with error type warning.
        /// </summary>
        /// <param name="facilityInfo">The facility info.</param>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static ErrorInfo CreateWarningInfo(FacilityInfo facilityInfo, int errorCode, string message, params object[] args)
        {
            return new ErrorInfo(ErrorType.Warning, facilityInfo, errorCode, message, args);
        }

        /// <summary>
        /// Creates an informational info with error type informational.
        /// </summary>
        /// <param name="facilityInfo">The facility info.</param>
        /// <param name="infoCode">The info code.</param>
        /// <param name="message">The message.</param>
        /// <param name="args">The args.</param>
        /// <returns></returns>
        public static ErrorInfo CreateInformationalInfo(FacilityInfo facilityInfo, int infoCode, string message, params object[] args)
        {
            return new ErrorInfo(ErrorType.Informational, facilityInfo, infoCode, message, args);
        }
    }
}
