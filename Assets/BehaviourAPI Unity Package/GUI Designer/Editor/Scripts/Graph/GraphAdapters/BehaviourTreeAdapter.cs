using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using Framework;
    using BehaviourTrees;

    [CustomGraphAdapter(typeof(BehaviourTree))]
    public class BehaviourTreeAdapter : GraphAdapter
    {
        public override string IconPath => BehaviourAPISettings.instance.IconPath + "Graphs/bt.png";

        public override void AutoLayout(GraphData graphData)
        {
            LayoutHandler layoutHandler = new TreeLayoutHandler();
            layoutHandler.Compute(graphData);
        }

        protected override EditorHierarchyNode CreateNodeHierarchy(Type graphtype, List<Type> types)
        {
            EditorHierarchyNode mainNode = new EditorHierarchyNode("BT Nodes");

            EditorHierarchyNode leafNode = new EditorHierarchyNode(typeof(LeafNode));
            EditorHierarchyNode compositeNode = new EditorHierarchyNode("Composite nodes");
            EditorHierarchyNode decoratorNode = new EditorHierarchyNode("Decorator nodes");

            for(int i = 0; i < types.Count; i++)
            {
                if (typeof(CompositeNode).IsAssignableFrom(types[i]))
                {
                    compositeNode.Childs.Add(new EditorHierarchyNode(types[i]));
                }
                else if (typeof(DecoratorNode).IsAssignableFrom(types[i]))
                {
                    decoratorNode.Childs.Add(new EditorHierarchyNode(types[i]));
                }
            }

            mainNode.Childs.Add(leafNode);
            mainNode.Childs.Add(decoratorNode);
            mainNode.Childs.Add(compositeNode);
            return mainNode;
        }
    }
}
