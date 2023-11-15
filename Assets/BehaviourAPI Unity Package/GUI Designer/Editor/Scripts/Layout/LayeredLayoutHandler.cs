using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;

    /// <summary>
    /// Layout handler for layered/directed acyclic graphs.
    /// </summary>
    public class LayeredLayoutHandler : LayoutHandler
    {
        private static readonly Vector2 k_nodeOffset = new Vector2(300, 200);

        Dictionary<NodeData, int> nodeLevelMap = new Dictionary<NodeData, int>();

        /// <summary>
        /// <inheritdoc/>
        /// 
        /// </summary>
        /// <param name="graphData"><inheritdoc/></param>
        protected override void ComputeLayout(GraphData graphData)
        {
            foreach (var node in graphData.nodes)
            {
                if (!nodeLevelMap.ContainsKey(node)) ComputeLevel(node);
            }

            var list = nodeLevelMap.ToList();

            var maxLevel = list.Max(kvp => kvp.Value);

            for (int i = 0; i <= maxLevel; i++)
            {
                List<NodeData> nodes = list.FindAll(kvp => kvp.Value == i).Select(kvp => kvp.Key).ToList();
                var dist = i;
                ComputePositions(nodes, dist);
            }
        }

        private int ComputeLevel(NodeData nodeData)
        {
            int currentLevel = 0;

            for (int i = 0; i < nodeData.childIds.Count; i++)
            {
                NodeData child = nodeIdMap[nodeData.childIds[i]];
                int childValue = nodeLevelMap.TryGetValue(child, out int level) ? level : ComputeLevel(child);
                if (childValue + 1 > currentLevel) currentLevel = childValue + 1;
            }
            nodeLevelMap[nodeData] = currentLevel;
            return currentLevel;
        }

        private void ComputePositions(List<NodeData> nodes, int level)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (level == 0)
                {
                    nodes[i].position = new Vector2(level, i) * k_nodeOffset;
                }
                else
                {
                    nodes[i].position = new Vector2(level, nodes[i].childIds.Select(c => nodeIdMap[c])
                        .Average(child => child.position.y));
                }
            }

            if (level != 0)
            {
                var midPos = nodes.Average(n => n.position.y);
                if (nodes.Count == 1)
                {
                    var dist = level * k_nodeOffset.x;
                    nodes[0].position = new Vector2(dist, midPos);
                }
                else
                {
                    nodes = nodes.OrderBy(n => n.position.y).ToList();
                    var dist = level * k_nodeOffset.x;
                    var midCount = (nodes.Count - 1) / 2f;

                    for (int i = 0; i < nodes.Count; i++)
                    {
                        nodes[i].position = new Vector2(dist, midPos) + k_nodeOffset * new Vector2(0, i - midCount);
                    }
                }
            }
        }
    }
}
