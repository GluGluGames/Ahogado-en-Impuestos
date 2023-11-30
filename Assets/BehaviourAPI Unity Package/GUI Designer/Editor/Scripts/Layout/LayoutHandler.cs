namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;
    using System.Collections.Generic;

    /// <summary>
    /// Class that implements an auto layout algorythm to compute node positions in a graph
    /// </summary>
    public abstract class LayoutHandler
    {

        /// <summary>
        /// 
        /// </summary>
        protected Dictionary<string, NodeData> nodeIdMap = new Dictionary<string, NodeData>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphData"></param>
        public void Compute(GraphData graphData)
        {
            if (graphData.nodes.Count == 0) return;

            nodeIdMap = graphData.GetNodeIdMap();
            ComputeLayout(graphData);
        }

        /// <summary>
        /// Modify <paramref name="graphData"/> node positions using a layout algorythm.
        /// </summary>
        /// <param name="graphData">The graph modified.</param>
        protected abstract void ComputeLayout(GraphData graphData);
    }
}
