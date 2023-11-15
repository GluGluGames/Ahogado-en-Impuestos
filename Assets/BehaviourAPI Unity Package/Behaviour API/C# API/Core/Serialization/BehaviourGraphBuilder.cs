using System.Collections.Generic;

namespace BehaviourAPI.Core.Serialization
{
    /// <summary>
    /// Used to create a <see cref="BehaviourGraph"/> using serialized data.
    /// <para>This class is only used to create behaviour graphs from custom serialized
    /// data in external tools. Don't use it directly on code.</para>
    /// </summary>
    public class BehaviourGraphBuilder
    {
        BehaviourGraph graph;

        List<NodeData> nodes;

        /// <summary>
        /// Create a new <see cref="BehaviourGraphBuilder"/> with the specified graph.
        /// </summary>
        /// <param name="graph">The graph</param>
        public BehaviourGraphBuilder(BehaviourGraph graph)
        {
            this.graph = graph;
            nodes = new List<NodeData>();
        }

        /// <summary>
        /// Add a new node to the graph.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="node">The node reference.</param>
        /// <param name="parents">The node parent list.</param>
        /// <param name="children">The node child list.</param>
        public void AddNode(string name, Node node, List<Node> parents, List<Node> children)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                graph.AddNode(node);
            }
            else
            {
                graph.AddNode(name, node);
            }

            nodes.Add(new NodeData(node, parents, children));
        }

        /// <summary>
        /// Add a new node to the graph.
        /// </summary>
        /// <param name="node">The node reference.</param>
        /// <param name="parents">The node parent list.</param>
        /// <param name="children">The node child list.</param>
        public void AddNode(Node node, List<Node> parents, List<Node> children)
        {
            graph.AddNode(node);
            nodes.Add(new NodeData(node, parents, children));
        }

        /// <summary>
        /// Build the node connections and graph internal data.
        /// </summary>
        public void Build()
        {
            nodes.ForEach(n => n.Build());
            graph.Build();
        }

        private struct NodeData
        {
            public Node node;
            public List<Node> parents;
            public List<Node> children;

            public NodeData(Node node, List<Node> parents, List<Node> children)
            {
                this.node = node;
                this.parents = parents;
                this.children = children;
            }

            public void Build() => node.BuildConnections(parents, children);
        }
    }
}
