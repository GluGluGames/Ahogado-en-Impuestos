using System;
using System.Collections.Generic;

namespace BehaviourAPI.Core
{
    /// <summary> 
    /// The basic element of the behaviour graphs. 
    /// </summary>
    public abstract class Node : ICloneable
    {
        #region ------------------------------------------ Properties -----------------------------------------

        /// <summary>
        /// The graph of the node.
        /// </summary>
        public BehaviourGraph BehaviourGraph { get; internal set; }

        /// <summary>
        /// The type of graph that this node can belong.
        /// </summary>
        public abstract Type GraphType { get; }
        /// <summary>
        /// The type of nodes that this node can handle as a child(s).
        /// </summary>
        public abstract Type ChildType { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="Parents"/>.
        /// </summary>
        public abstract int MaxInputConnections { get; }

        /// <summary>
        /// Maximum number of elements in <see cref="Children"/>.
        /// </summary>
        public abstract int MaxOutputConnections { get; }

        /// <summary>   
        /// Gets the number of child nodes.
        /// </summary>
        /// <value> The number of elements in child node list. </value>
        public int ChildCount => Children.Count;

        /// <summary>   
        /// Gets the number of parent nodes.
        /// </summary>
        /// <value> The number of elements in parent node list. </value>
        public int ParentCount => Parents.Count;

        #endregion

        #region -------------------------------------- Private variables ----------------------------------------

        /// <summary>
        /// List of connections in the graph with this node as target.
        /// </summary>
        protected internal List<Node> Parents;

        /// <summary>
        /// List of connections in the graph with this node as source.
        /// </summary>
        protected internal List<Node> Children;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Empty constructor
        /// </summary>
        protected Node()
        {
            Children = new List<Node>();
            Parents = new List<Node>();
        }

        /// <summary>
        /// Get the child node at specified position.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node at <paramref name="index"/> position in child list.</returns>
        public Node GetChildAt(int index) => Children[index];

        /// <summary>
        /// Get the parent node at specified position.
        /// </summary>
        /// <param name="index">The index of the node.</param>
        /// <returns>The node at <paramref name="index"/> position in parent list.</returns>
        public Node GetParentAt(int index) => Parents[index];

        /// <summary>
        /// Checks if a node is connected with this as target.
        /// </summary>
        /// <param name="node">The checked target node</param>
        /// <returns>True if the checked node is a child of this node.</returns>
        public bool IsChildOf(Node node)
        {
            return Parents.Contains(node);
        }

        /// <summary>
        /// Checks if a node is connected with this as source.
        /// </summary>
        /// <param name="node">The checked source node</param>
        /// <returns>True if the checked node is a parent of this node.</returns>
        public bool IsParentOf(Node node)
        {
            return Children.Contains(node);
        }

        /// <summary>
        /// Get the fists child node.
        /// </summary>
        /// <returns>The first element in the child list or null if the list is empty.</returns>
        public Node GetFirstChild() => Children.Count > 0 ? Children[0] : null;

        /// <summary>
        /// Get the fists parent node.
        /// </summary>
        /// <returns>The first element in the parent list or null if the list is empty.</returns>
        public Node GetFirstParent() => Parents.Count > 0 ? Parents[0] : null;

        /// <summary>
        /// Return true if this node is the start node of the graph.
        /// </summary>
        public bool IsStartNode() => BehaviourGraph?.StartNode == this;

        /// <summary>
        /// Check if the node can have more children.
        /// </summary>
        /// <returns>True if can have more children, false otherwise.</returns>
        public bool CanAddAChild() => MaxOutputConnections == -1 || Children.Count < MaxOutputConnections;

        /// <summary>
        /// Check if the node can have more parents.
        /// </summary>
        /// <returns>True if can have more children, false otherwise.</returns>
        public bool CanAddAParent() => MaxInputConnections == -1 || Parents.Count < MaxInputConnections;

        /// <summary>
        /// Override this method to set internal node variables when building from serialized data.
        /// </summary>
        /// <param name="parents">The list of parent nodes.</param>
        /// <param name="children">The list of child nodes.</param>
        protected internal virtual void BuildConnections(List<Node> parents, List<Node> children)
        {
            if (MaxInputConnections != -1 && parents.Count > MaxInputConnections)
                throw new ArgumentException($"The parent list has to many elements ({parents.Count}, when the maximum in this node is {MaxInputConnections})");

            if (MaxOutputConnections != -1 && children.Count > MaxOutputConnections)
                throw new ArgumentException($"The child list has to many elements ({children.Count}, when the maximum in this node is {MaxOutputConnections})");

            Parents = parents;
            Children = children;
        }

        /// <summary>
        /// Overrides this method to set the node's internal variables when a copy is created.
        /// </summary>
        /// <returns>A field copy of the node. </returns>
        public virtual object Clone()
        {
            var node = (Node)MemberwiseClone();
            node.BehaviourGraph = null;
            node.Parents = new List<Node>();
            node.Children = new List<Node>();
            return node;
        }

        /// <summary>
        /// Override this method to define how it uses the execution context.
        /// </summary>
        public virtual void SetExecutionContext(ExecutionContext context)
        {
            return;
        }

        #endregion
    }
}
