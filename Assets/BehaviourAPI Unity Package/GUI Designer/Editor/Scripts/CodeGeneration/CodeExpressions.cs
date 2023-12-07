using System;
using System.Collections.Generic;
using System.Linq;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor.CodeGenerator
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using Framework;

    public abstract class CodeElement
    {
        public abstract void GenerateCode(CodeWriter writer, CodeGenerationOptions options);
    }

    public abstract class CodeMember : CodeElement
    {
    }

    public class CodeFieldMember : CodeMember
    {
        private bool isPublic;
        private bool isSerializeField;

        public string name;
        public Type type;

        public CodeExpression InitExpression;

        public CodeFieldMember(string name, Type type, bool isPublic = false, bool isSerializeField = true)
        {
            this.name = name;
            this.type = type;
            this.isPublic = isPublic;
            this.isSerializeField = isSerializeField;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            if (isSerializeField) writer.Append("[SerializeField] ");
            writer.Append(isPublic ? "public " : "private ");
            writer.Append(type.Name + " " + name);
            if (InitExpression != null) InitExpression.GenerateCode(writer, options);
            writer.AppendLine(";");
        }
    }

    public class CodeMethodMember : CodeMember
    {
        public string Name;
        public Type returnType;

        public List<CodeParameter> Parameters = new List<CodeParameter>();
        public CodeMethodMember(string name, Type returnType)
        {
            Name = name;
            this.returnType = returnType;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.AppendLine("");

            var typeName = returnType == null ? "void" : returnType == typeof(float) ? "float" : returnType.Name;

            writer.Append($"private {typeName} {Name}(");

            for (int i = 0; i < Parameters.Count; i++)
            {
                Parameters[i].GenerateCode(writer, options);
                if (i != Parameters.Count - 1) writer.Append(", ");
            }

            writer.AppendLine(")");
            writer.AppendLine("{");
            writer.IdentationLevel++;
            writer.AppendLine("throw new System.NotImplementedException();");
            writer.IdentationLevel--;
            writer.AppendLine("}");
        }
    }

    public class CodeParameter : CodeElement
    {
        public Type type;
        public string name;

        public CodeParameter(Type type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.Append(type.Name + " " + name);
        }
    }


    public abstract class CodeStatement : CodeElement
    {
    }

    public class CodeVariableDeclarationStatement : CodeStatement
    {
        public Type Type;
        public string Identifier;
        public CodeExpression RightExpression;

        public CodeVariableDeclarationStatement(Type type, string identifier)
        {
            Type = type;
            Identifier = identifier;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            var typeName = options.useVarKeyword ? "var" : Type.Name;
            writer.Append($"{typeName} {Identifier} = ");
            if (RightExpression != null)
                RightExpression.GenerateCode(writer, options);
            else
                writer.Append("null");
            writer.AppendLine(";");
        }
    }

    public class CodeAssignationStatement : CodeStatement
    {
        public CodeExpression LeftExpression;
        public CodeExpression RightExpression;
        public override void GenerateCode(CodeWriter codeWritter, CodeGenerationOptions options)
        {
            LeftExpression.GenerateCode(codeWritter, options);
            codeWritter.Append(" = ");
            RightExpression.GenerateCode(codeWritter, options);
            codeWritter.AppendLine(";");
        }
    }

    public class CodeCustomStatement : CodeStatement
    {
        string statement;

        public CodeCustomStatement(string statement)
        {
            this.statement = statement;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.AppendLine(statement);
        }
    }

    public abstract class CodeExpression : CodeElement
    {
    }

    public class CodeMethodReferenceExpression : CodeExpression
    {
        public string invokerIdentifier;
        public string memberName;

        public CodeMethodReferenceExpression(string memberName)
        {
            this.memberName = memberName;
        }

        public CodeMethodReferenceExpression(string invokerIdentifier, string memberName)
        {
            this.invokerIdentifier = invokerIdentifier;
            this.memberName = memberName;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            if (invokerIdentifier != null) writer.Append(invokerIdentifier + ".");
            writer.Append(memberName);
        }
    }

    public class CodeMethodInvokeExpression : CodeExpression
    {
        public CodeMethodReferenceExpression methodReferenceExpression;
        public List<CodeExpression> parameters = new List<CodeExpression>();

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            methodReferenceExpression.GenerateCode(writer, options);
            writer.Append("(");
            for (int i = 0; i < parameters.Count; i++)
            {
                parameters[i].GenerateCode(writer, options);
                if (i != parameters.Count - 1) writer.Append(", ");
            }
            writer.Append(")");
        }

        public void Add(CodeExpression expression)
        {
            parameters.Add(expression);
        }
    }

    public class CodeNodeCreationMethodExpression : CodeMethodInvokeExpression
    {
        public string nodeName;

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            methodReferenceExpression.GenerateCode(writer, options);
            writer.Append("(");
            if (options.includeNames && !string.IsNullOrEmpty(nodeName))
            {
                writer.Append('\"' + nodeName + '\"');
                if (parameters.Count > 0) writer.Append(", ");
            }
            for (int i = 0; i < parameters.Count; i++)
            {
                parameters[i].GenerateCode(writer, options);
                if (i != parameters.Count - 1) writer.Append(", ");
            }
            writer.Append(")");
        }
    }

    public class CodeObjectCreationExpression : CodeExpression
    {
        public Type Type;

        public List<CodeExpression> parameters = new List<CodeExpression>();

        public CodeObjectCreationExpression(Type type)
        {
            Type = type;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.Append($"new {Type.Name}(");

            for (int i = 0; i < parameters.Count; i++)
            {
                parameters[i].GenerateCode(writer, options);
                if (i != parameters.Count - 1) writer.Append(", ");
            }

            writer.Append(")");
        }

        public void Add(CodeExpression parameter)
        {
            parameters.Add(parameter);
        }
    }

    public class CodeCustomExpression : CodeExpression
    {
        public string Expression;

        public CodeCustomExpression(string expression)
        {
            Expression = expression;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.Append(Expression);
        }
    }

    public class CodeNamedExpression : CodeExpression
    {
        public string name;
        public CodeExpression expression;

        public CodeNamedExpression(string name, CodeExpression expression)
        {
            this.name = name;
            this.expression = expression;
        }

        public override void GenerateCode(CodeWriter writer, CodeGenerationOptions options)
        {
            writer.Append(name + ": ");
            expression.GenerateCode(writer, options);
        }
    }

    /// <summary>
    /// Class that represents the set of instructions needed to instantiate a node.
    /// </summary>
    public class CodeNodeStatementGroup : CodeStatement
    {
        NodeData m_NodeData;

        CodeTemplate m_Template;

        List<CodeStatement> m_argumentStatements;
        CodeVariableDeclarationStatement m_NodeCreationStatement;
        CodeMethodInvokeExpression m_MethodInvoke;
        List<CodeStatement> m_PropertyStatements;


        public CodeNodeStatementGroup(NodeData nodeData, CodeTemplate template)
        {
            m_NodeData = nodeData;
            m_Template = template;
            string identifier = template.GetSystemElementIdentifier(nodeData.id);

            m_NodeCreationStatement = new CodeVariableDeclarationStatement(nodeData.node.GetType(), identifier);
            var nodeExpression = new CodeNodeCreationMethodExpression();
            nodeExpression.nodeName = nodeData.name;
            m_MethodInvoke = nodeExpression;
            m_NodeCreationStatement.RightExpression = m_MethodInvoke;

            m_argumentStatements = new List<CodeStatement>();
            m_PropertyStatements = new List<CodeStatement>();
        }

        public void Commit() => m_Template.AddNodeStatement(this);

        public void SetMethod(string k_method)
        {
            string graphIdentifier = m_Template.CurrentGraphIdentifier;
            m_MethodInvoke.methodReferenceExpression = new CodeMethodReferenceExpression(graphIdentifier, k_method);
        }

        public void AddFloat(float value)
        {
            m_MethodInvoke.parameters.Add(new CodeCustomExpression(value.ToCodeFormat()));
        }

        public void AddChildList()
        {
            for (int i = 0; i < m_NodeData.childIds.Count; i++)
            {
                string childIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.childIds[i]);
                var childExpression = new CodeCustomExpression(childIdentifier);
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddFirstChild()
        {
            if (m_NodeData.childIds.Count > 0)
            {
                string childIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.childIds[0]);
                var childExpression = new CodeCustomExpression(childIdentifier);
                m_MethodInvoke.parameters.Add(childExpression);
            }
            else
            {
                var childExpression = new CodeCustomExpression("null /* missing child */");
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddAction(string fieldName, bool isNotMandatory = false, string paramName = null)
        {
            var actionData = m_NodeData.references.Find((r) => r.FieldName == fieldName);

            if (actionData != null && actionData.Value is Action action)
            {
                var actionIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.id) + "_action";
                var actionExpression = m_Template.GetActionExpression(action, actionIdentifier, m_argumentStatements);
                m_MethodInvoke.parameters.Add(actionExpression);
            }
            else if (!isNotMandatory)
            {
                var childExpression = new CodeCustomExpression("null /* missing action */");
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddPerception(string fieldName, bool isNotMandatory = false, string paramName = null)
        {
            var perceptionData = m_NodeData.references.Find((r) => r.FieldName == fieldName);

            if (perceptionData != null && perceptionData.Value is Perception perception)
            {
                var perceptionIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.id) + "_perception";
                var perceptionExpression = m_Template.GetPerceptionExpression(perception, perceptionIdentifier, m_argumentStatements);
                m_MethodInvoke.parameters.Add(perceptionExpression);
            }
            else if (!isNotMandatory)
            {
                var childExpression = new CodeCustomExpression("null /* missing perception */");
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddStatusFlags(StatusFlags statusFlags, bool isNotMandatory = false, string paramName = null)
        {
            if ((int)statusFlags == -1 || statusFlags == StatusFlags.Active)
            {
                if (isNotMandatory) return;
                else statusFlags = StatusFlags.Active;
            }
            string paramPrefix = string.IsNullOrEmpty(paramName) ? "" : paramName + ": ";
            var statusExpression = new CodeCustomExpression(paramPrefix + "StatusFlags." + statusFlags);
            m_MethodInvoke.parameters.Add(statusExpression);
        }


        public void AddFirstParent(bool isNotMandatory = false)
        {
            if (m_NodeData.parentIds.Count > 0)
            {
                string parentIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.parentIds[0]);
                var parentExpression = new CodeCustomExpression(parentIdentifier);
                m_MethodInvoke.parameters.Add(parentExpression);
            }
            else if (!isNotMandatory)
            {
                var childExpression = new CodeCustomExpression("null /* missing parent */");
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddStatus(Status exitStatus)
        {
            var statusExpression = new CodeCustomExpression("Status." + exitStatus);
            m_MethodInvoke.parameters.Add(statusExpression);
        }

        public void AddFunction(string fieldName, bool isNotMandatory = false)
        {
            var methodData = m_NodeData.references.Find((r) => r.FieldName == fieldName);

            if (methodData != null && methodData.Value is SerializedContextMethod contextMethod)
            {
                var fieldInfo = m_NodeData.node.GetType().GetField(methodData.FieldName);

                if (fieldInfo == null || !fieldInfo.FieldType.IsSubclassOf(typeof(Delegate))) return;

                var methodInfo = fieldInfo.FieldType.GetMethod("Invoke");
                var returnParam = methodInfo.ReturnParameter.ParameterType;
                var args = methodInfo.GetParameters().Select(p => p.GetType()).ToArray();

                var functionIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.id) + "_function";
                var functionExpression = m_Template.GenerateMethodCodeExpression(contextMethod, args, returnParam);
                m_MethodInvoke.parameters.Add(functionExpression);
            }
            else if (!isNotMandatory)
            {
                var childExpression = new CodeCustomExpression("null /* missing action */");
                m_MethodInvoke.parameters.Add(childExpression);
            }
        }

        public void AddBool(bool b)
        {
            var boolExpression = new CodeCustomExpression(b.ToCodeFormat());
            m_MethodInvoke.parameters.Add(boolExpression);
        }

        public void AddPropertyAssignations()
        {
            string nodeIdentifier = m_Template.GetSystemElementIdentifier(m_NodeData.id);
            foreach (var field in m_NodeData.node.GetType().GetFields(System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.FlattenHierarchy | System.Reflection.BindingFlags.Instance))
            {
                var value = field.GetValue(m_NodeData.node);
                if (value != null)
                {
                    var statement = m_Template.GetPropertyStatement(field.GetValue(m_NodeData.node), nodeIdentifier, field.Name, m_PropertyStatements);
                    m_PropertyStatements.Add(statement);
                }
            }
        }

        public override void GenerateCode(CodeWriter codeWriter, CodeGenerationOptions options)
        {
            m_argumentStatements.ForEach(s => s.GenerateCode(codeWriter, options));
            m_NodeCreationStatement.GenerateCode(codeWriter, options);
            m_PropertyStatements.ForEach(s => s.GenerateCode(codeWriter, options));
            codeWriter.AppendLine("");
        }
    }
}
