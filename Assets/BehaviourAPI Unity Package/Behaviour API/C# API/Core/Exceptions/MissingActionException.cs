using System;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Exception that is thrown when a node must execute an action and this action is null.
    /// </summary>
    public class MissingActionException : NullReferenceException
    {
        /// <summary>
        /// The node that throws the exception.
        /// </summary>
        public Node Node;

        /// <summary>
        /// Create a new <see cref="MissingActionException"/> with the specified node.
        /// </summary>
        /// <param name="node">The node that throws the exception.</param>
        public MissingActionException(Node node)
        {
            Node = node;
        }

        /// <summary>
        /// Create a new <see cref="MissingActionException"/> with the specified node and a message.
        /// </summary>
        /// <param name="node">The node that throws the exception.</param>
        /// <param name="message">The message.</param>
        public MissingActionException(Node node, string message) : base(message)
        {
            Node = node;
        }
    }
}
