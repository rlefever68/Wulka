using System.ServiceModel;
using NLog;

namespace Wulka.Agent
{
	/// <summary>
	/// Exposes methods that will handle the most common exceptions
	/// that occur in an agent.
	/// </summary>
	public class AgentExceptionHandler
	{
        private readonly Logger _logger;

		/// <summary>
		/// Singleton
		/// </summary>
		public static AgentExceptionHandler Instance = new AgentExceptionHandler();

	    public AgentExceptionHandler()
	    {
            _logger = LogManager.GetCurrentClassLogger();
	    }

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		public void Handle(FaultException exception, string agentName)
		{
			_logger.Warn("Unexpected Service Exception caught : {0}", exception.Message);
			_logger.Debug(string.Format("Unexpected Service Exception caught : {0}", exception.Message), exception);

			throw new AgentException(agentName, "An Unexpected Service Exception was caught", exception);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		/// <param name="methodName">Name of the method.</param>
		public void Handle(FaultException exception, string agentName, string methodName)
		{
			_logger.Warn("Unexpected Service Exception caught while executing {0} : {1}",
                methodName, exception.Message);
			_logger.Debug(string.Format("Unexpected Service Exception caught while executing {0} : {1}",
                methodName, exception.Message), exception);

			throw new AgentException(agentName,
									 "An Unexpected Service Exception was caught while executing " + methodName,
									 exception);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		public void Handle(CommunicationException exception, string agentName)
		{
			_logger.Warn("Communication Exception caught : {0}", exception.Message);
			_logger.Debug(string.Format("Communication Exception caught : {0}", exception.Message), exception);

			throw new AgentException(agentName, "Could not connect to service", exception);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		/// <param name="methodName">Name of the method.</param>
		public void Handle(CommunicationException exception, string agentName, string methodName)
		{
			_logger.Warn("Communication Exception caught while executing {0} : {1}",
                methodName, exception.Message);
			_logger.Debug(string.Format("Communication Exception caught while executing {0} : {1}",
                methodName, exception.Message), exception);

			throw new AgentException(agentName, "Could not connect to service while executing " + methodName, exception);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		public void Handle(CommunicationObjectFaultedException exception, string agentName)
		{
			_logger.Warn("Communication Object Faulted caught : {0}", exception.Message);
			_logger.Debug(string.Format("Communication Object Faulted caught : {0}", exception.Message), exception);
		}

		/// <summary>
		/// Handles the specified exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <param name="agentName">Name of the agent.</param>
		/// <param name="methodName">Name of the method.</param>
		public void Handle(CommunicationObjectFaultedException exception, string agentName, string methodName)
		{
			_logger.Warn("Communication Exception caught while executing {0} : {1}",
                methodName, exception.Message);
			_logger.Debug(string.Format("Communication Exception caught while executing {0} : {1}",
                methodName, exception.Message), exception);

			throw new AgentException(agentName, "Could not connect to service while executing " 
                + methodName, exception);
		}
	}
}