using System;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Reflection;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using Core.Serialization;

    /// <summary>
    /// Class that serialize node data.
    /// </summary>
    [Serializable]
    public class NodeData
    {
        /// <summary>
        /// The name of the node.
        /// </summary>
        public string name;

        /// <summary>
        /// The unique id of this element.
        /// </summary>
        [HideInInspector] public string id;

        /// <summary>
        /// The position of the node in the editor.
        /// </summary>
        [HideInInspector] public UnityEngine.Vector2 position;

        /// <summary>
        /// The serializable reference of the node.
        /// </summary>
        [SerializeReference] public Node node;

        /// <summary>
        /// List that allows unity to serialize elements.
        /// </summary>
        [SerializeField] public List<ReferenceData> references = new List<ReferenceData>(); 

        /// <summary>
        /// List of parent nodes referenced by id.
        /// </summary>
        [HideInInspector] public List<string> parentIds = new List<string>();

        /// <summary>
        /// List of children nodes referenced by id.
        /// </summary>
        [HideInInspector] public List<string> childIds = new List<string>();

        public NodeData(Type type, Vector2 position)
        {
            this.position = position;
            node = (Node)Activator.CreateInstance(type);
            name = "";
            id = Guid.NewGuid().ToString();

            ValidateReferences(type);
        }

        /// <summary>
        /// Create a new node data by a node and id.
        /// Used to create data from a graph created directly in code.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> reference</param>
        /// <param name="id">The id of the element.</param>
        public NodeData(Node node, string id, string name)
        {
            this.node = node;
            this.id = id;
            this.name = name ?? "";
        }

        public bool Validate() => ValidateReferences(node.GetType());

        bool ValidateReferences(Type type)
        {
            List<ReferenceData> currentReferences = new List<ReferenceData>();
            Dictionary<string, ReferenceData> referenceMap = references.ToDictionary(r => r.FieldName, r => r);

            foreach (var fieldInfo in type.GetFields(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance))
            {
                if (typeof(Action).IsAssignableFrom(fieldInfo.FieldType) || 
                    typeof(Perception).IsAssignableFrom(fieldInfo.FieldType) ||
                    typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
                {
                    var refData = new ReferenceData(fieldInfo.Name, fieldInfo.FieldType);
                    currentReferences.Add(refData);
                    if(referenceMap.TryGetValue(fieldInfo.Name, out ReferenceData oldReferenceData))
                    {
                        string currentFullType = fieldInfo.FieldType.AssemblyQualifiedName;
                        if(currentFullType == oldReferenceData.FieldType)
                        {
                            refData.Value = oldReferenceData.Value;
                        }
                    }
                    else if(typeof(Delegate).IsAssignableFrom(fieldInfo.FieldType))
                    {
                        refData.Value = new SerializedContextMethod();
                    }
                }
            }
            references = currentReferences;

            return true;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public NodeData()
        {
        }

        /// <summary>
        /// Build the references that are not serialized directly in node.
        /// These are node connections, actions, perceptions and functions.
        /// </summary>
        /// <param name="graphBuilder">The builder used to set the connections.</param>
        public void BuildReferences(BehaviourGraphBuilder graphBuilder, BSBuildingInfo buildData)
        {
            if (node == null) Debug.LogWarning("BUILD ERROR: The referenced node is null");

            List<Node> parents = parentIds.Select(id => buildData.NodeMap[id]).ToList();
            List<Node> children = childIds.Select(id => buildData.NodeMap[id]).ToList();
            graphBuilder.AddNode(name, node, parents, children);
            references.ForEach(r => r.Build(node, buildData));
        }
    }
}