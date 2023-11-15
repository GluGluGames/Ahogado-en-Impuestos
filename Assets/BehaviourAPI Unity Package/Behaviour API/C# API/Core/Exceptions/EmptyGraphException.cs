using System;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Exception that is thrown when a graph is executed without nodes.
    /// </summary>
    public class EmptyGraphException : Exception
    {
        /// <summary>
        /// The graph that throws the exception.
        /// </summary>
        public BehaviourGraph Graph { get; private set; }

        /// <summary>
        /// Create a new <see cref="EmptyGraphException"/> with the specified graph.
        /// </summary>
        /// <param name="graph">The graph that throws the exception.</param>
        public EmptyGraphException(BehaviourGraph graph)
        {
            Graph = graph;
        }

        /// <summary>
        /// Create a new <see cref="EmptyGraphException"/> with the specified graph and a message.
        /// </summary>
        /// <param name="graph">The graph that throws the exception.</param>
        /// <param name="message">The message.</param>
        public EmptyGraphException(BehaviourGraph graph, string message) : base(message)
        {
            Graph = graph;
        }
    }
}
