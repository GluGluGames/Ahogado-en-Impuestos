using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.Graphs
{
    using Framework;
    using StateMachines;

    [CustomGraphAdapter(typeof(FSM))]
    public class StateMachineAdapter : GraphAdapter
    {
        public override string IconPath => BehaviourAPISettings.instance.IconPath + "Graphs/fsm.png";
        public override void AutoLayout(GraphData graphData)
        {
            LayoutHandler layoutHandler = new CyclicLayoutHandler();
            layoutHandler.Compute(graphData);
        }

        protected override EditorHierarchyNode CreateNodeHierarchy(Type graphtype, List<Type> types)
        {
            EditorHierarchyNode mainNode = new EditorHierarchyNode("FSM nodes");
            EditorHierarchyNode stateNode = new EditorHierarchyNode("States");
            EditorHierarchyNode transitionNodes = new EditorHierarchyNode("Transitions");

            for (int i = 0; i < types.Count; i++)
            {
                if (typeof(State).IsAssignableFrom(types[i]))
                {
                    stateNode.Childs.Add(new EditorHierarchyNode(types[i]));
                }
                else if (typeof(Transition).IsAssignableFrom(types[i]))
                {
                    transitionNodes.Childs.Add(new EditorHierarchyNode(types[i]));
                }
            }

            mainNode.Childs.Add(stateNode);
            mainNode.Childs.Add(transitionNodes);
            return mainNode;
        }
    }
}
