using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    using Framework;

    public abstract class GraphCodeGenerator
    {
        GraphData m_GraphData;

        Dictionary<string, NodeData> m_NodeIdMap;

        HashSet<string> m_GeneratedNodes;

        public string GraphIdentifier { get; protected set; }

        public static GraphCodeGenerator GetGenerator(GraphData graphData)
        {
            var metadata = BehaviourAPISettings.instance.Metadata;
            var graphType = graphData.graph.GetType();

            if (metadata.CodeGeneratorMap.TryGetValue(graphType, out Type generatorType))
            {
                var generator = (GraphCodeGenerator)Activator.CreateInstance(generatorType);
                generator.m_GraphData = graphData;
                generator.m_NodeIdMap = graphData.GetNodeIdMap();
                generator.m_GeneratedNodes = new HashSet<string>();
                return generator;
            }
            else
            {
                return null;
            }
        }

        public void GenerateCode(CodeTemplate template)
        {
            GenerateGraphDeclaration(m_GraphData, template);
        }

        public abstract void GenerateGraphDeclaration(GraphData graphData, CodeTemplate codeTemplate);

        protected NodeData GetNodeById(string id) => m_NodeIdMap.GetValueOrDefault(id);

        protected void MarkGenerated(string id) => m_GeneratedNodes.Add(id);

        protected bool IsGenerated(string id) => m_GeneratedNodes.Contains(id);

        protected CodeExpression GetChildExpression(string nodeId, CodeTemplate template)
        {
            var nodeIdentifierName = template.GetSystemElementIdentifier(nodeId);
            if (nodeIdentifierName != null)
            {
                return new CodeCustomExpression(nodeIdentifierName);
            }
            else
            {
                return new CodeCustomExpression("null /* missing node */");
            }
        }
    }
}
