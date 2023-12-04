using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    using BehaviourTrees;
    using Framework;
    using System.Linq;

    [CustomGraphCodeGenerator(typeof(BehaviourTree))]
    public class BehaviourTreeCodeGenerator : GraphCodeGenerator
    {
        private static readonly string k_DecoratorMethod = "CreateDecorator";
        private static readonly string k_CompositeMethod = "CreateComposite";
        private static readonly string k_LeafMethod = "CreateLeafNode";

        public override void GenerateGraphDeclaration(GraphData graphData, CodeTemplate template)
        {
            GraphIdentifier = template.GetSystemElementIdentifier(graphData.id);
            var type = graphData.graph.GetType();
            var graphStatement = new CodeVariableDeclarationStatement(type, GraphIdentifier);
            graphStatement.RightExpression = new CodeObjectCreationExpression(type);

            template.AddNamespace("BehaviourAPI.BehaviourTrees");
            template.AddGraphCreationStatement(graphStatement);

            template.CurrentGraphIdentifier = GraphIdentifier;

            foreach (NodeData nodeData in graphData.nodes)
            {
                GenerateCode(nodeData, template);
            }
        }

        private void GenerateCode(NodeData nodeData, CodeTemplate template)
        {
            if (nodeData == null || IsGenerated(nodeData.id)) return;
            CodeNodeStatementGroup nodeCode = new CodeNodeStatementGroup(nodeData, template);
            GenerateNodeCode(nodeData, nodeCode, template);
            MarkGenerated(nodeData.id);
            nodeCode.Commit();
        }


        private void GenerateNodeCode(NodeData nodeData, CodeNodeStatementGroup code, CodeTemplate template)
        {
            switch(nodeData.node)
            {
                case LeafNode:
                    code.SetMethod(k_LeafMethod);
                    code.AddAction("Action");
                    break;

                case DecoratorNode:
                    code.SetMethod(k_DecoratorMethod + "<" + nodeData.node.TypeName() + ">");

                    if (nodeData.childIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                    code.AddFirstChild();
                    code.AddPropertyAssignations();
                    break;

                case CompositeNode comp:
                    code.SetMethod(k_CompositeMethod + "<" + nodeData.node.TypeName() + ">");
                    code.AddBool(comp.IsRandomized);

                    for(int i = 0; i < nodeData.childIds.Count; i++)
                        GenerateCode(GetNodeById(nodeData.childIds[i]), template);

                    code.AddChildList();
                    code.AddPropertyAssignations();
                    break;
            }
        }
    }
}
