using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;
    using System;
    using UnityEngine;

    /// <summary>
    /// Layout handler for tree graphs.
    /// </summary>
    public class TreeLayoutHandler : LayoutHandler
    {
        private static readonly Vector2 k_nodeOffset = new Vector2(300, 200);


        /// <summary>
        /// 
        /// </summary>
        public List<float> levelDistMap = new List<float>();

        /// <summary>
        /// <inheritdoc/>
        /// Consists on a simplification of Reingold-Tilford Algorithm.
        /// </summary>
        /// <param name="graphData"><inheritdoc/></param>
        protected override void ComputeLayout(GraphData graphData)
        {
            foreach (var node in graphData.nodes)
            {
                if (node.parentIds.Count > 0) continue;

                ProcessTreeNode(node, 0, 0);
            }
        }

        private float ProcessTreeNode(NodeData node, int currentDeep, float targetXPos)
        {
            var x = GetTreeNodeXPosition(node, currentDeep, targetXPos);

            node.position = new Vector2(x, currentDeep) * k_nodeOffset;
            return x;
        }

        private float GetTreeNodeXPosition(NodeData node, int currentDeep, float targetXPos)
        {
            if (levelDistMap.Count >= currentDeep) levelDistMap.Add(targetXPos - 1);

            targetXPos = MathF.Max(levelDistMap[currentDeep] + 1, targetXPos);

            if (node.childIds.Count == 0)
            {
                levelDistMap[currentDeep] = targetXPos;
            }
            else
            {
                levelDistMap[currentDeep] = GetTreeBranchXPosition(node, currentDeep, targetXPos);
            }

            return levelDistMap[currentDeep];
        }

        private float GetTreeBranchXPosition(NodeData branchNode, int currentDeep, float targetXPos)
        {
            float firstX = 0f;
            float lastX = 0f;
            for (int i = 0; i < branchNode.childIds.Count; i++)
            {
                NodeData child = nodeIdMap[branchNode.childIds[i]];
                float childOffset = i - (branchNode.childIds.Count - 1f) / 2f;
                float targetChildXPos = targetXPos + childOffset;
                float computedChildXPos = ProcessTreeNode(child, currentDeep + 1, targetChildXPos);

                targetXPos = Mathf.Max(computedChildXPos - childOffset, targetXPos);

                if (i == 0) firstX = computedChildXPos;
                if (i == branchNode.childIds.Count - 1) lastX = computedChildXPos;
            }
            return (firstX + lastX) * .5f;
        }
    }
}
