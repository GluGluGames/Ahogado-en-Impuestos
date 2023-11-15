using System.Linq;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    using UtilitySystems;
    using Framework;

    [CustomGraphCodeGenerator(typeof(UtilitySystem))]
    public class UtilitySystemCodeGenerator : GraphCodeGenerator
    {
        private static readonly string k_VariableFactorMethod = "CreateVariable";
        private static readonly string k_ConstantFactorMethod = "CreateConstant";
        private static readonly string k_CurveFactorMethod = "CreateCurve";
        private static readonly string k_FusionFactorMethod = "CreateFusion";

        private static readonly string k_UtilityActionMethod = "CreateAction";
        private static readonly string k_UtilityExitNodeMethod = "CreateExitNode";
        private static readonly string k_UtilityBucketMethod = "CreateBucket";

        public override void GenerateGraphDeclaration(GraphData graphData, CodeTemplate template)
        {
            GraphIdentifier = template.GetSystemElementIdentifier(graphData.id);
            var type = graphData.graph.GetType();
            UtilitySystem us = graphData.graph as UtilitySystem;

            var graphCreationExpression = new CodeObjectCreationExpression(us.GetType());
            graphCreationExpression.Add(new CodeCustomExpression(us.Inertia.ToCodeFormat()));

            var graphStatement = new CodeVariableDeclarationStatement(type, GraphIdentifier);
            graphStatement.RightExpression = graphCreationExpression;

            template.AddNamespace("BehaviourAPI.UtilitySystems");
            template.AddGraphCreationStatement(graphStatement);
            template.CurrentGraphIdentifier = GraphIdentifier;

            foreach (var nodeData in graphData.nodes)
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
            switch (nodeData.node)
            {
                case VariableFactor variableFactor:
                    code.SetMethod(k_VariableFactorMethod);
                    code.AddFunction("Variable");
                    code.AddFloat(variableFactor.min);
                    code.AddFloat(variableFactor.max);
                    break;

                case ConstantFactor constantFactor:
                    code.SetMethod(k_ConstantFactorMethod);
                    code.AddFloat(constantFactor.value);
                    break;

                case FusionFactor:
                    code.SetMethod(k_FusionFactorMethod + "<" + nodeData.node.TypeName() + ">");

                    for (int i = 0; i < nodeData.childIds.Count; i++)
                        GenerateCode(GetNodeById(nodeData.childIds[i]), template);

                    code.AddChildList();
                    break;

                case CurveFactor:
                    code.SetMethod(k_CurveFactorMethod + "<" + nodeData.node.TypeName() + ">");

                    if(nodeData.childIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                    code.AddFirstChild();
                    break;

                case UtilityAction:
                    code.SetMethod(k_UtilityActionMethod);

                    if (nodeData.childIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                    code.AddFirstChild();
                    code.AddAction("Action");

                    if (nodeData.parentIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                    code.AddFirstParent(true);
                    break;

                case UtilityExitNode utilityExitNode:
                    code.SetMethod(k_UtilityExitNodeMethod);

                    if (nodeData.childIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.childIds[0]), template);

                    code.AddFirstChild();
                    code.AddStatus(utilityExitNode.ExitStatus);

                    if (nodeData.parentIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                    code.AddFirstParent(true);
                    break;
                case UtilityBucket utilityBucket:
                    code.SetMethod(k_UtilityBucketMethod);
                    code.AddFloat(utilityBucket.Inertia);
                    code.AddFloat(utilityBucket.BucketThreshold);

                    if (nodeData.parentIds.Count > 0)
                        GenerateCode(GetNodeById(nodeData.parentIds[0]), template);

                    code.AddFirstParent(true);
                    break;
            }
        }      
    }
}
