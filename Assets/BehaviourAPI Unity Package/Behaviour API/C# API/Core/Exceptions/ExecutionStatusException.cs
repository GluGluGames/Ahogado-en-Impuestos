using System;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Exception that is thrown when a <see cref="IStatusHandler"/> tries to change 
    /// its <see cref="Status"/> in a wrong way.
    /// </summary>
    public class ExecutionStatusException : Exception
    {
        /// <summary>
        /// The <see cref="IStatusHandler"/> that throws the exception.
        /// </summary>
        public IStatusHandler StatusHandler { get; set; }

        /// <summary>
        /// Create a new <see cref="ExecutionStatusException"/> with the specified status handler.
        /// </summary>
        /// <param name="statusHandler">The <see cref="IStatusHandler"/> that throws the exception.</param>
        public ExecutionStatusException(IStatusHandler statusHandler)
        {
            StatusHandler = statusHandler;
        }

        /// <summary>
        /// Create a new <see cref="ExecutionStatusException"/> with the specified status handler and a message.
        /// </summary>
        /// <param name="statusHandler">The <see cref="IStatusHandler"/> that throws the exception.</param>
        public ExecutionStatusException(IStatusHandler statusHandler, string message) : base(message)
        {
            StatusHandler = statusHandler;
        }
    }
}
