using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    using Framework;
    using StateMachines;
    using System.Linq;

    [CustomGraphCodeGenerator(typeof(FSM))]
    public class StateMachineCodeGenerator : GraphCodeGenerator
    {
        private static readonly string k_StateMethod = "CreateState";
        private static readonly string k_ProbabilisticStateMethod = "CreateProbabilisticState";
        private static readonly string k_StateTransitionMethod = "CreateTransition";
        private static readonly string k_ExitTransitionMethod = "CreateExitTransition";

        public override void GenerateGraphDeclaration(GraphData graphData, CodeTemplate template)
        {
            GraphIdentifier = template.GetSystemElementIdentifier(graphData.id);
            var type = graphData.graph.GetType();
            var graphStatement = new CodeVariableDeclarationStatement(type, GraphIdentifier);
            graphStatement.RightExpression = new CodeObjectCreationExpression(type);

            template.AddNamespace("BehaviourAPI.StateMachines");
            template.AddGraphCreationStatement(graphStatement);

            template.CurrentGraphIdentifier = GraphIdentifier;

            foreach (var nodeData in graphData.nodes)
            {
                GenerateCode(nodeData, template);
            }
        }

        protected void GenerateCode(NodeData nodeData, CodeTemplate template)
        {
            if (nodeData == null || IsGenerated(nodeData.id)) return;
            CodeNodeStatementGroup nodeCode = new CodeNodeStatementGroup(nodeData, template);
            GenerateNodeCode(nodeData, nodeCode, template);
            MarkGenerated(nodeData.id);
            nodeCode.Commit();
        }

        protected virtual void GenerateNodeCode(NodeData nodeData, CodeNodeStatementGroup code, CodeTemplate template)
        {
            switch (nodeData.node)
            {
                case ProbabilisticState pState:
                    code.SetMethod(k_ProbabilisticStateMethod);
                    code.AddAction("Action", true);
                    code.AddPropertyAssignations();
                     break;

                case State:
                    code.SetMethod(k_StateMethod);
                    code.AddAction("Action", true);
                    break;

                case StateTransition stateTransition:
                    code.SetMethod(k_StateTransitionMethod);

                    if (nodeData.parentIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                    code.AddFirstParent();

                    if (nodeData.childIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                    code.AddFirstChild();
                    code.AddPerception("Perception", true, "perception");
                    code.AddAction("Action", true, "action");
                    code.AddStatusFlags(stateTransition.StatusFlags, true, "statusFlags");
                    break;
                   
                
                case ExitTransition exitTransition:
                    code.SetMethod(k_ExitTransitionMethod);

                    if (nodeData.parentIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                    code.AddFirstParent();
                    code.AddStatus(exitTransition.ExitStatus);
                    code.AddPerception("Perception", true, "perception");
                    code.AddAction("Action", true, "action");
                    code.AddStatusFlags(exitTransition.StatusFlags, true, "statusFlags");
                    break;
            }
        }
    }
}