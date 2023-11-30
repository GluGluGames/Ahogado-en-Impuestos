using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using UtilitySystems;
    using Framework;

    [CustomGraphAdapter(typeof(UtilitySystem))]
    public class UtilitySystemAdapter : GraphAdapter
    {
        public override string IconPath => BehaviourAPISettings.instance.IconPath + "Graphs/us.png";
        public override void AutoLayout(GraphData graphData)
        {
            LayoutHandler layoutHandler = new LayeredLayoutHandler();
            layoutHandler.Compute(graphData);
        }

        protected override EditorHierarchyNode CreateNodeHierarchy(Type graphtype, List<Type> types)
        {
            EditorHierarchyNode mainNode = new EditorHierarchyNode("Utility nodes");
            EditorHierarchyNode factorNodes = new EditorHierarchyNode("Factors");
            EditorHierarchyNode leafFactorNode = new EditorHierarchyNode("Leaf factors");
            EditorHierarchyNode fusionFactorNode = new EditorHierarchyNode("Fusion factors");
            EditorHierarchyNode curveFactor = new EditorHierarchyNode("Curve factors");

            EditorHierarchyNode actionNode = new EditorHierarchyNode(typeof(UtilityAction));
            EditorHierarchyNode exitNode = new EditorHierarchyNode(typeof(UtilityExitNode));
            EditorHierarchyNode bucketNode = new EditorHierarchyNode(typeof(UtilityBucket));

            factorNodes.Childs.Add(leafFactorNode);
            factorNodes.Childs.Add(fusionFactorNode);
            factorNodes.Childs.Add(curveFactor);
            for (int i = 0; i < types.Count; i++)
            {
                if (typeof(FusionFactor).IsAssignableFrom(types[i]))
                    fusionFactorNode.Childs.Add(new EditorHierarchyNode(types[i]));
                else if (typeof(CurveFactor).IsAssignableFrom(types[i]))
                    curveFactor.Childs.Add(new EditorHierarchyNode(types[i]));
                else if (typeof(LeafFactor).IsAssignableFrom(types[i]))
                    leafFactorNode.Childs.Add(new EditorHierarchyNode(types[i]));
            }

            mainNode.Childs.Add(actionNode);
            mainNode.Childs.Add(exitNode);
            mainNode.Childs.Add(bucketNode);
            mainNode.Childs.Add(factorNodes);
            return mainNode;
        }
    }
}
