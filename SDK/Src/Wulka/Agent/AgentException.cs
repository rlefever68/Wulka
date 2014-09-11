using System;

namespace Wulka.Agent
{
    /// <summary>
    /// 
    /// </summary>
    public class AgentException : Exception
    {
        /// <summary>
        /// Gets or sets the agent.
        /// </summary>
        /// <value>The agent.</value>
        public string Agent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentException"/> class.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public AgentException(String agent, String message, Exception innerException)
            : this(agent + " - " + message, innerException)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="AgentException"/> class.
        /// </summary>
        /// <param name="agent">The agent.</param>
        /// <param name="innerException">The inner exception.</param>
        public AgentException(String agent, Exception innerException)
            : base(agent, innerException)
        {
            Agent = agent;
        }
    }

}
