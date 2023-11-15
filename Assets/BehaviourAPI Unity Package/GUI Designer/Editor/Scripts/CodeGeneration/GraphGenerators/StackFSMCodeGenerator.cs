using BehaviourAPI.StateMachines;
using BehaviourAPI.StateMachines.StackFSMs;
using BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator;
using BehaviourAPI.UnityToolkit.GUIDesigner.Framework;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    [CustomGraphCodeGenerator(typeof(StackFSM))]
    public class StackFSMCodeGenerator : StateMachineCodeGenerator
    {
        private static readonly string k_PopTransitionMethod = "CreatePopTransition";
        private static readonly string k_PushTransitionMethod = "CreatePushTransition";

        protected override void GenerateNodeCode(NodeData nodeData, CodeNodeStatementGroup code, CodeTemplate template)
        {
            if(nodeData.node is StackTransition)
            {
                switch (nodeData.node)
                {
                    case PopTransition pop:
                        code.SetMethod(k_PopTransitionMethod);

                        if (nodeData.parentIds.Count > 0)
                            GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                        code.AddFirstParent();

                        code.AddPerception("Perception", true, "perception");
                        code.AddAction("Action", true, "action");
                        code.AddStatusFlags(pop.StatusFlags, true, "statusFlags");
                        break;

                    case PushTransition push:
                        code.SetMethod(k_PushTransitionMethod);

                        if (nodeData.parentIds.Count > 0)
                            GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                        code.AddFirstParent();

                        if (nodeData.childIds.Count > 0)
                            GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                        code.AddFirstChild();

                        code.AddPerception("Perception", true, "perception");
                        code.AddAction("Action", true, "action");
                        code.AddStatusFlags(push.StatusFlags, true, "statusFlags");
                        break;
                }
            }
            else
            {
                base.GenerateNodeCode(nodeData, code, template);
            }           
        }
    }
}
