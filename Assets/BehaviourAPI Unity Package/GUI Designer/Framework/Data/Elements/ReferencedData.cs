using System;
using System.Reflection;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using System.Linq;

    [System.Serializable]
    public class ReferenceData
    {
        [SerializeField] string fieldName;  
        [SerializeReference] object value;
        [SerializeReference] string fieldType;

        public ReferenceData(string name, Type type)
        {
            fieldName = name;
            fieldType = type.AssemblyQualifiedName;
        }

        public string FieldName => fieldName;
        public string FieldType => fieldType;
        public object Value { get => value; set => this.value = value; }

        public void Build(Node node, BSBuildingInfo buildData)
        {
            if (Value == null) return;

            FieldInfo field = node.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);

            if(field == null || field.FieldType.AssemblyQualifiedName != fieldType)
            {
                Debug.LogWarning($"The field \"{fieldName}\" does not exist or is not assignable from {fieldType}, the value was not set properly.\n");
                return;
            }

            if (value is SerializedContextMethod method)
            {
                Component component = string.IsNullOrEmpty(method.componentName) ? buildData.Runner : buildData.Runner.gameObject.GetComponent(method.componentName);

                Type classType = string.IsNullOrEmpty(method.componentName) ? buildData.Runner.GetType() : Type.GetType(method.componentName);               

                if (!field.FieldType.IsSubclassOf(typeof(Delegate))) return;

                MethodInfo delegateMethod = field.FieldType.GetMethod("Invoke");
                ParameterInfo[] parameters = delegateMethod.GetParameters();
                Type[] parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

                var del = method.GetDelegate(buildData.Runner, parameterTypes, field.FieldType);

                if (del != null) field.SetValue(node, del);
            }
            else
            {
                field.SetValue(node, value);
                if (value is IBuildable buildable) buildable.Build(buildData);
            }
        }
    }
}