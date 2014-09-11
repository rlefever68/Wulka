using System;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using NLog;
using Wulka.Exceptions;

namespace Wulka.Networking.Wcf
{
    /// <summary>
    /// FaultErrorHandler which logs the errors as unhandled exceptions
    /// </summary>
    public class FaultErrorHandler : IErrorHandler
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="FaultErrorHandler"/> class.
        /// </summary>
        public FaultErrorHandler()
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FaultErrorHandler"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service for which errors are handled.</param>
        [Obsolete("Constructor with parameters is obsolete, use parameterless constructor instead")]
        public FaultErrorHandler(Type serviceType)
        {
        }

        #region IErrorHandler Members

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="exception">The exception thrown during processing.</param>
        /// <returns>
        /// true if subsequent <see cref="T:System.ServiceModel.Dispatcher.IErrorHandler"/> implementations
        /// must not be called; otherwise, false. The default is false.
        /// </returns>
        public bool HandleError(Exception exception)
        {
            // Disabled exception logging because exceptions raised by the application are already logged and
            // there is currently no mechanism to check if an exception is raised by the application or is
            // really an unhandled exception
            Logger.Error("An Error occurred");
            Logger.Error(exception.GetCombinedMessages());
            return false; // Return false to indicate that the exception was not handled
        }

        private static readonly Logger Logger = LogManager.GetLogger("FaultErrorHandler");

        /// <summary>
        /// Enables the creation of a custom <see cref="T:System.ServiceModel.FaultException`1"/> that is returned from an exception in the course of a service method.
        /// </summary>
        /// <param name="error">The <see cref="T:System.Exception"/> object thrown in the course of the service operation.</param><param name="version">The SOAP version of the message.</param><param name="fault">The <see cref="T:System.ServiceModel.Channels.Message"/> object that is returned to the client, or service, in the duplex case.</param>
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            // Ignore
        }

        #endregion
    }
}