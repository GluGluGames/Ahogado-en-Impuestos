using BehaviourAPI.Core;
using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using Framework;

    /// <summary>
    /// Allows editor tools to use specific graph type.
    /// </summary>
    public abstract class GraphAdapter
    {
        /// <summary>
        /// Get the node hierarchy of the graph. Used to display de node creation window.
        /// </summary>
        public EditorHierarchyNode NodeHierarchy { get; private set; }

        /// <summary>
        /// Path to the icon file displayed in the graph creation panel. Override to set a custom icon.
        /// </summary>
        public virtual string IconPath => BehaviourAPISettings.instance.IconPath + "Graphs/default.png";

        /// <summary>
        /// Create the supported hierarchy of the graph
        /// </summary>
        /// <param name="graphType">The graph type./param>
        /// <param name="nodeTypes">The list of available node types.</param>
        public void BuildSupportedHerarchy(Type graphType, List<Type> nodeTypes)
        {
            var validTypes = GetValidNodeTypes(graphType, nodeTypes);
            NodeHierarchy = CreateNodeHierarchy(graphType, validTypes);
        }

        /// <summary>
        /// Compute the position of the nodes of the graph
        /// </summary>
        /// <param name="graphData">The graph data that contains the nodes.</param>
        public abstract void AutoLayout(GraphData graphData);

        /// <summary>
        /// Gets the node hierarchy of the graph.
        /// </summary>
        /// <param name="graphType">The graph type./param>
        /// <param name="nodeTypes">The list of available node types.</param>
        /// <returns></returns>
        protected abstract EditorHierarchyNode CreateNodeHierarchy(Type graphType, List<Type> nodeTypes);

        #region ----------------- Static methods ----------------

        private static Dictionary<Type, GraphAdapter> factoryCache = new Dictionary<Type, GraphAdapter>();

        /// <summary>
        /// Get the adapter assigned to the graph type. Create it if didn't exist yet.
        /// </summary>
        /// <param name="graphType">The graph type.</param>
        /// <returns>The graph adapter corresponding to the graph type.</returns>
        public static GraphAdapter GetAdapter(Type graphType)
        {
            var metadata = BehaviourAPISettings.instance.Metadata;
            if (metadata.GraphAdapterMap.TryGetValue(graphType, out Type adapterType))
            {
                return GetOrCreate(adapterType, graphType);
            }
            else
            {
                return null;
            }
        }

        private static GraphAdapter GetOrCreate(Type adapterType, Type graphType)
        {
            if (factoryCache.TryGetValue(adapterType, out GraphAdapter adapter))
            {
                return adapter;
            }
            else
            {
                adapter = (GraphAdapter)Activator.CreateInstance(adapterType);
                adapter.BuildSupportedHerarchy(graphType, BehaviourAPISettings.instance.Metadata.NodeTypes);
                factoryCache[adapterType] = adapter;
                return adapter;
            }
        }

        private static List<Type> GetValidNodeTypes(Type graphType, List<Type> nodeTypes)
        {
            var graph = (BehaviourGraph)Activator.CreateInstance(graphType);
            List<Type> validNodeTypes = new List<Type>();
            for (int i = 0; i < nodeTypes.Count; i++)
            {
                var node = (Node)Activator.CreateInstance(nodeTypes[i]);
                if (node.GraphType.IsAssignableFrom(graphType) && graph.NodeType.IsAssignableFrom(nodeTypes[i]))
                {
                    validNodeTypes.Add(nodeTypes[i]);
                }
            }
            return validNodeTypes;
        }

        #endregion
    }
}
