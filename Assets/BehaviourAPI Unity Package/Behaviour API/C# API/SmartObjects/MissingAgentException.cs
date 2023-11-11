using System;

namespace BehaviourAPI.SmartObjects
{
    /// <summary>
    /// Exception that is thrown when a request action send a request to a smart object without an agent assigned.
    /// </summary>
    public class MissingAgentException<T> : Exception where T : ISmartAgent
    {
        /// <summary>
        /// The request action that throws the exception.
        /// </summary>
        RequestAction<T> RequestAction { get; set; }

        /// <summary>
        /// Create a new missing agent exception with the specified request action.
        /// </summary>
        /// <param name="requestAction">The request action that throws the exception.</param>
        public MissingAgentException(RequestAction<T> requestAction)
        {
            RequestAction = requestAction;
        }

        /// <summary>
        /// Create a new missing agent exception with the specified request action.
        /// </summary>
        /// <param name="requestAction">The request action that throws the exception and a message.</param>
        public MissingAgentException(RequestAction<T> requestAction, string message) : base(message)
        {
            RequestAction = requestAction;
        }
    }
}
