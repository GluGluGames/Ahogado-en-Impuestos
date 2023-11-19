using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.Core
{
    /// <summary> 
    /// Behavior engine in the form of a directed graph.
    /// </summary>
    public abstract class BehaviourGraph : BehaviourEngine, ICloneable
    {
        #region ----------------------------------------- Properties -------------------------------------------

        /// <summary>
        /// The base type of the <see cref="Node"/> elements that this <see cref="BehaviourGraph"/> can contain.
        /// </summary>
        public abstract Type NodeType { get; }

        /// <summary>
        /// The default entry point of the graph. Its always the first element in the node list.
        /// </summary>
        public Node StartNode
        {
            get
            {
                if (Nodes.Count == 0)
                    throw new EmptyGraphException(this, "This graph is empty.");
                return Nodes[0];
            }
            protected set
            {
                if (!Nodes.Contains(value))
                    throw new ArgumentException("Ths node is not in the graph");

                if (Nodes[0] != value)
                {
                    if (Nodes.Remove(value)) Nodes.Insert(0, value);
                }
            }
        }

        /// <summary>
        /// True if nodes can have more than one connection with the same node.
        /// </summary>
        public abstract bool CanRepeatConnection { get; }

        /// <summary>
        /// True if connections can create loops.
        /// </summary>
        public abstract bool CanCreateLoops { get; }

        /// <summary>
        /// The amount of nodes in the graph.
        /// </summary>
        public int NodeCount => Nodes.Count;

        #endregion

        #region -------------------------------------- Private variables ----------------------------------------

        /// <summary>
        /// The graph node list.
        /// </summary>
        protected List<Node> Nodes = new List<Node>();

        Dictionary<string, Node> _nodeDict = new Dictionary<string, Node>();

        #endregion

        #region ---------------------------------------- Build methods -----------------------------------------

        /// <summary>
        /// Create a new node of type <typeparamref name="T"/> named <paramref name="name"/> in this Graph.
        /// </summary>
        /// <typeparam name="T">The type of the new node.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <returns>The created node.</returns>
        protected T CreateNode<T>(string name) where T : Node, new()
        {
            T node = new T();
            AddNode(name, node);
            return node;
        }

        /// <summary>
        /// Create a new node of type <typeparamref name="T"/> in this Graph.
        /// </summary>
        /// <typeparam name="T">The type of the new node.</typeparam>
        /// <returns>The created node.</returns>
        protected T CreateNode<T>() where T : Node, new()
        {
            T node = new T();
            AddNode(node);
            return node;
        }

        /// <summary>
        /// Add an existing node to the graph
        /// </summary>
        /// <param name="node">The node added.</param>
        /// <exception cref="ArgumentException">If the node type is not compatible with the graph.</exception>
        protected internal virtual void AddNode(Node node)
        {
            if (!NodeType.IsAssignableFrom(node.GetType()))
                throw new ArgumentException($"Error adding a node: An instance of type {node.GetType()} cannot be added, " +
                    $"this graph only handles nodes of types derived from {NodeType}");

            if (!node.GraphType.IsAssignableFrom(GetType()))
                throw new ArgumentException($"Error adding a node: An instance of type {node.GetType()} cannot be added, " +
                    $"This node can only belongs to a graph of types derived from {node.GraphType}");

            Nodes.Add(node);
            node.BehaviourGraph = this;
        }

        /// <summary>
        /// Add an existing node and identify it with a name.
        /// </summary>
        /// <param name="node">The node added.</param>
        /// <param name="name">The name of the node.</param>
        /// <exception cref="ArgumentException">If <paramref name="name"/> is null or whitespaced.</exception>
        /// <exception cref="ArgumentException">If <paramref name="name"/> was already used as key.</exception>
        protected internal void AddNode(string name, Node node)
        {
            AddNode(node);

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Can't add node to dictionary if the name is null or whitespaced");

            if (_nodeDict.ContainsKey(name))
                throw new ArgumentException($"A node with the specified name ({name}) already exists");

            _nodeDict.Add(name, node);

        }

        /// <summary>
        /// Connect two nodes, setting <paramref name="source"/> as parent of <paramref name="target"/> and <paramref name="target"/> as child of <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source node</param>
        /// <param name="target">The target node</param>
        /// <exception cref="ArgumentException">if <paramref name="source"/> or <paramref name="target"/> values are unvalid.</exception>
        protected void Connect(Node source, Node target)
        {
            if (source == null || target == null)
                throw new ArgumentException($"ERROR: Source or target are null references");

            if (source == target)
                throw new ArgumentException($"ERROR: Source and child cannot be the same node");

            if (!source.ChildType.IsAssignableFrom(target.GetType()))
                throw new ArgumentException($"ERROR: Source node child type({source.GetType()}) can handle target's type ({target.GetType()}) as a child. It should be {source.ChildType}");

            if (source.BehaviourGraph != this)
                throw new ArgumentException("ERROR: Source node is not in the graph.");

            if (target.BehaviourGraph != this)
                throw new ArgumentException("ERROR: Target node is not in the graph.");

            if (!source.CanAddAChild())
                throw new ArgumentException("ERROR: Maximum child count reached in source");

            if (!target.CanAddAParent())
                throw new ArgumentException("ERROR: Maximum parent count reached in target");

            if (!CanRepeatConnection && AreNodesDirectlyConnected(source, target))
                throw new ArgumentException("ERROR: Can't create two connections with the same source and target.");

            if (!CanCreateLoops && AreNodesConnected(target, source))
                throw new ArgumentException("ERROR: Can't create a loop in this graph.");

            source.Children.Add(target);
            target.Parents.Add(source);
        }

        /// <summary>
        /// Returns if graph has a connection path between <paramref name="source"/> and <paramref name="target"/>.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>True if a path between the nodes exists.</returns>
        public bool AreNodesConnected(Node source, Node target)
        {
            HashSet<Node> unvisitedNodes = new HashSet<Node>();
            HashSet<Node> visitedNodes = new HashSet<Node>();
            unvisitedNodes.Add(source);
            while (unvisitedNodes.Count > 0)
            {
                Node n = unvisitedNodes.First();
                unvisitedNodes.Remove(n);
                visitedNodes.Add(n);
                foreach (var child in n.Children)
                {
                    if (child == null) continue;

                    if (child == target)
                        return true;
                    if (!visitedNodes.Contains(child))
                        unvisitedNodes.Add(child);
                }
            }
            return false;
        }

        /// <summary>
        /// Return true if a connection (<paramref name="source"/> -> <paramref name="target"/>) already exists.
        /// </summary>
        /// <param name="source">The source node.</param>
        /// <param name="target">The target node.</param>
        /// <returns>True if the connection exists.</returns>
        public bool AreNodesDirectlyConnected(Node source, Node target)
        {
            // should be both true or false
            return source.IsParentOf(target) /*|| target.IsChildOf(source)*/;
        }

        /// <summary>
        /// Gets a field copy of the graph without all the nodes (only copies the fields). 
        /// </summary>
        /// <returns>The graph copy. </returns>
        public virtual object Clone()
        {
            var graph = (BehaviourGraph)MemberwiseClone();
            graph.Nodes = new List<Node>();
            graph._nodeDict = new Dictionary<string, Node>();
            return graph;
        }

        /// <summary>
        /// Override this method to buid the internal references after create a graph from serialized data.
        /// </summary>
        protected internal virtual void Build()
        {
            return;
        }

        /// <summary>
        /// Gets a list with all the nodes in the graph.
        /// </summary>
        public List<Node> NodeList => new List<Node>(Nodes);

        /// <summary>
        /// Inverts the name-node dictionary
        /// </summary>
        /// <returns>A new dictionary in which the key is the node and the value is its name.</returns>
        public Dictionary<Node, string> GetNodeNames()
        {
            return _nodeDict.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// FInd a node using index operator.
        /// </summary>
        /// <param name="key">The name of the node.</param>
        /// <returns>The node with the specified name.</returns>
        public Node this[string key] => FindNode(key);

        /// <summary>
        /// Find a node in this graph by it's <paramref name="name"/>
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph.</returns>
        /// <exception cref="KeyNotFoundException">If the key doesn't exist.</exception>
        public Node FindNode(string name)
        {
            if (_nodeDict.TryGetValue(name, out Node node))
            {
                return node;
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph");
            }
        }

        /// <summary>
        /// Find a node in this graph by it's <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph or null if was not found.</returns>
        public Node FindNodeOrDefault(string name)
        {
            if (_nodeDict.TryGetValue(name, out Node node))
            {
                return node;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Find a node of type <typeparamref name="T"/> in this graph by it's <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph.</returns>
        /// <exception cref="InvalidCastException">If the found node is not an instance of <typeparamref name="T"/></exception>
        /// <exception cref="KeyNotFoundException">If the key doesn't exist.</exception>
        public T FindNode<T>(string name) where T : Node
        {
            if (_nodeDict.TryGetValue(name, out Node node))
            {
                if (node is T element)
                {
                    return element;
                }
                else
                {
                    throw new InvalidCastException($"Node \"{name}\" exists, but is not an instance of {typeof(T).FullName} class.");
                }
            }
            else
            {
                throw new KeyNotFoundException($"Node \"{name}\" doesn't exist in this graph.");
            }
        }

        /// <summary>
        /// Find a node of type <typeparamref name="T"/> in this graph by it's <paramref name="name"/>
        /// </summary>
        /// <typeparam name="T">The type of the node.</typeparam>
        /// <param name="name">The name of the node.</param>
        /// <returns>The node with the given <paramref name="name"/> in this graph or null if was not found.</returns>
        public T FindNodeOrDefault<T>(string name) where T : Node
        {
            if (_nodeDict.TryGetValue(name, out Node node))
            {
                if (node is T element)
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Passes the context to all its nodes.
        /// </summary>
        public override void SetExecutionContext(ExecutionContext context)
        {
            Nodes.ForEach(node => node.SetExecutionContext(context));
        }

        #endregion
    }
}
