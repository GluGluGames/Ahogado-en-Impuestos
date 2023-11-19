using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using Core.Serialization;

    /// <summary>
    /// Class that serializes graph data
    /// </summary>
    [Serializable]
    public class GraphData
    {
        /// <summary>
        /// The name of the graph.
        /// </summary>
        public string name;

        /// <summary>
        /// The unique id of this element.
        /// </summary>
        [HideInInspector] public string id;

        /// <summary>
        /// The serializable reference of the graph.
        /// </summary>
        [SerializeReference] public BehaviourGraph graph;

        /// <summary>
        /// The serialized list of graph nodes.
        /// </summary>
        public List<NodeData> nodes = new List<NodeData>();

        /// <summary>
        /// Create a new <see cref="GraphData"/> by its graph type.
        /// </summary>
        /// <param name="graphType">The type of the <see cref="BehaviourGraph"/> stored.</param>
        public GraphData(Type graphType)
        {
            graph = (BehaviourGraph)Activator.CreateInstance(graphType);
            name = graphType.Name;
            id = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public GraphData()
        {
        }

        /// <summary>
        /// Create a new graph data by a Behaviour graph and name.
        /// </summary>
        /// <param name="graph">The <see cref="graph"/> reference.</param>
        /// <param name="name">The name of the graph.</param>
        public GraphData(BehaviourGraph graph, string name)
        {
            this.graph = graph;
            this.name = name;
            id = Guid.NewGuid().ToString();
            var nodes = graph.NodeList;
            Dictionary<Node, string> map = nodes.ToDictionary(n => n, n => Guid.NewGuid().ToString());

            var nodeNames = graph.GetNodeNames();
            for (int i = 0; i < nodes.Count; i++)
            {
                string id = map[nodes[i]];

                var nodename = nodeNames.GetValueOrDefault(nodes[i]);
                NodeData nodeData = new NodeData(nodes[i], id, nodename);


                for (int j = 0; j < nodes[i].ParentCount; j++)
                {
                    nodeData.parentIds.Add(map[nodes[i].GetParentAt(j)]);
                }

                for (int j = 0; j < nodes[i].ChildCount; j++)
                {
                    nodeData.childIds.Add(map[nodes[i].GetChildAt(j)]);
                }

                this.nodes.Add(nodeData);
            }
        }

        /// <summary>
        /// Gets a dictionary in which the key is the id of the node and the value is the node itself.
        /// </summary>
        /// <returns>The id - node dictionary.</returns>
        public Dictionary<string, NodeData> GetNodeIdMap()
        {
            return nodes.ToDictionary(n => n.id, n => n);
        }

        /// <summary>
        /// Sort <paramref name="nodeData"/> children list by a function delegate applied in the nodes.
        /// </summary>
        /// <param name="nodeData">The node whose childs are being ordered.</param>
        /// <param name="sortFunction">The function applied in the nodes.</param>
        public void OrderChildNodes(NodeData nodeData, Func<NodeData, float> sortFunction)
        {
            var dict = GetNodeIdMap();
            nodeData.childIds = nodeData.childIds.OrderBy(id =>
            {
                if (dict.TryGetValue(id, out NodeData data))
                {
                    return sortFunction(data);
                }
                else
                {
                    return float.MaxValue;
                }
            }).ToList();
        }

        /// <summary>
        /// Sort all node's children list by a function delegate applied in the nodes.
        /// </summary>
        /// <param name="sortFunction">The function applied in the nodes.</param>
        public void OrderAllChildNodes(Func<NodeData, float> sortFunction)
        {
            var dict = GetNodeIdMap();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].childIds = nodes[i].childIds.OrderBy(id =>
                {
                    if (dict.TryGetValue(id, out NodeData data))
                    {
                        return sortFunction(data);
                    }
                    else
                    {
                        return float.MaxValue;
                    }
                }).ToList();
            }
        }

        public HashSet<NodeData> GetDirectChildren(NodeData data)
        {
            var nodeIdMap = GetNodeIdMap();
            return data.childIds.Select(id => nodeIdMap[id]).ToHashSet();
        }

        public HashSet<NodeData> GetDirectParents(NodeData data)
        {
            var nodeIdMap = GetNodeIdMap();
            return data.parentIds.Select(id => nodeIdMap[id]).ToHashSet();
        }

        /// <summary>
        /// Gets a set with all reachable nodes from <paramref name="start"/> if the direction
        /// goes from parents to children.
        /// </summary>
        /// <param name="start">The source node.</param>
        /// <returns>The set with all reachable nodes.</returns>
        public HashSet<NodeData> GetChildPathing(NodeData start)
        {
            Dictionary<string, NodeData> nodeIdMap = GetNodeIdMap();
            HashSet<NodeData> visitedNodes = new HashSet<NodeData>();
            HashSet<NodeData> unvisitedNodes = new HashSet<NodeData>();

            unvisitedNodes.Add(start);
            while (unvisitedNodes.Count > 0)
            {
                var node = unvisitedNodes.First();
                unvisitedNodes.Remove(node);
                visitedNodes.Add(node);

                for (int i = 0; i < node.childIds.Count; i++)
                {
                    NodeData child = nodeIdMap[node.childIds[i]];

                    if (!visitedNodes.Contains(child))
                    {
                        unvisitedNodes.Add(child);
                    }
                }
            }
            return visitedNodes;
        }

        /// <summary>
        /// Gets a set with all reachable nodes from <paramref name="start"/> if the direction
        /// goes from children to parents.
        /// </summary>
        /// <param name="start">The source node.</param>
        /// <returns>The set with all reachable nodes.</returns>
        public HashSet<NodeData> GetParentPathing(NodeData start)
        {
            Dictionary<string, NodeData> nodeIdMap = GetNodeIdMap();
            HashSet<NodeData> visitedNodes = new HashSet<NodeData>();
            HashSet<NodeData> unvisitedNodes = new HashSet<NodeData>();

            unvisitedNodes.Add(start);
            while (unvisitedNodes.Count > 0)
            {
                var node = unvisitedNodes.First();
                unvisitedNodes.Remove(node);
                visitedNodes.Add(node);

                for (int i = 0; i < node.parentIds.Count; i++)
                {
                    NodeData child = nodeIdMap[node.parentIds[i]];

                    if (!visitedNodes.Contains(child))
                    {
                        unvisitedNodes.Add(child);
                    }
                }
            }
            return visitedNodes;
        }

        /// <summary>
        /// Build the internal references.
        /// </summary>
        /// <param name="data"></param>
        public void Build(BSBuildingInfo buildData)
        {
            var graphBuilder = new BehaviourGraphBuilder(graph);

            FixNodeNames();
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].BuildReferences(graphBuilder, buildData);  
            }

            graphBuilder.Build();
        }

        public bool ValidateReferences()
        {
            bool referencesChanged = false;
            foreach (NodeData nodeData in nodes)
            {
                referencesChanged |= nodeData.Validate();
            }
            return referencesChanged;
        }

        private void FixNodeNames()
        {
            HashSet<string> usedNames = new HashSet<string>();
            foreach (NodeData node in nodes)
            {
                if (!string.IsNullOrEmpty(node.name))
                {
                    var fixedName = node.name;
                    int index = 1;
                    while (usedNames.Contains(fixedName))
                    {
                        fixedName = node.name + "_" + index;
                        index++;
                    }
                    node.name = fixedName;
                    usedNames.Add(fixedName);
                }
            }
        }
    }
}
