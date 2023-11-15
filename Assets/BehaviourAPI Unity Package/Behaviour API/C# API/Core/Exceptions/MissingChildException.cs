using System;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Exception that is thrown when a node tries to access a child node that is null.
    /// </summary>
    public class MissingChildException : NullReferenceException
    {
        /// <summary>
        /// The node that throws the exception.
        /// </summary>
        public Node Node;

        /// <summary>
        /// Create a new <see cref="MissingChildException"/> with the specified node.
        /// </summary>
        /// <param name="node">The node that throws the exception.</param>
        public MissingChildException(Node node)
        {
            Node = node;
        }

        /// <summary>
        /// Create a new <see cref="MissingChildException"/> with the specified node and a message.
        /// </summary>
        /// <param name="node">The node that throws the exception.</param>
        /// <param name="message">The message.</param>
        public MissingChildException(Node node,string message) : base(message)
        {
            Node = node;
        }
    }
}
