using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    using Framework;
    using CodeGenerator;
    using Core;
    using Core.Actions;
    using Core.Perceptions;
    using Graphs;

    /// <summary>
    /// Class that manages all the Type metadata used in the package tools.
    /// </summary>
    public class APITypeMetadata
    {
        /// <summary>
        /// Dictionary that relates each non-abstract type of node to the Drawer that will be used to represent it in the editor.
        /// </summary>
        public Dictionary<Type, Type> NodeDrawerTypeMap { get; private set; } = new Dictionary<Type, Type>();

        /// <summary>
        /// Dictionary that relates each non-abstract type of behaviourGraph with
        /// </summary>
        public Dictionary<Type, Type> GraphAdapterMap { get; private set; } = new Dictionary<Type, Type>();

        /// <summary>
        /// Dictionary that relates each non-abstract type of behaviourGraph with
        /// </summary>
        public Dictionary<Type, Type> CodeGeneratorMap { get; private set; } = new Dictionary<Type, Type>();

        /// <summary>
        /// The hierarchy node used to select a new <see cref="Action"/> in the creation window.
        /// </summary>
        public EditorHierarchyNode ActionHierarchy { get; private set; }

        /// <summary>
        /// The hierarchy node used to select a new <see cref="Perception"/> in the creation window.
        /// </summary>
        public EditorHierarchyNode PerceptionHierarchy { get; private set; }

        /// <summary>
        /// Dictionary that stores all the component types in the App domain.
        /// </summary>
        public Dictionary<string, Type> componentMap { get; private set; } = new Dictionary<string, Type>();

        /// <summary>
        /// List of all node types available in the editor tool.
        /// </summary>
        public List<Type> NodeTypes = new List<Type>();

        /// <summary>
        /// Create a new API metadata.
        /// </summary>
        public APITypeMetadata()
        {
            var time = DateTime.Now;
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> compoundActionTypes = new List<Type>();
            List<Type> actionTypes = new List<Type>();
            List<Type> compoundPerceptionTypes = new List<Type>();
            List<Type> perceptionTypes = new List<Type>();
            HashSet<Type> nodeTypes = new HashSet<Type>();
            List<Type> graphTypes = new List<Type>();

            Dictionary<Type, Type> nodeDrawerMainTypeMap = new Dictionary<Type, Type>();
            Dictionary<Type, Type> graphAdapterMainTypeMap = new Dictionary<Type, Type>();
            Dictionary<Type, Type> graphCodeGeneratorMainTypeMap = new Dictionary<Type, Type>();

            int c = 0;
            for (int i = 0; i < assemblies.Length; i++)
            {
                Type[] types = assemblies[i].GetTypes();

                for (int j = 0; j < types.Length; j++)
                {
                    if (!IsValidType(types[j])) continue;

                    if (typeof(Node).IsAssignableFrom(types[j]))
                    {
                         nodeTypes.Add(types[j]);
                    }
                    else if (typeof(BehaviourGraph).IsAssignableFrom(types[j]))
                    {
                        graphTypes.Add(types[j]);
                    }
                    else if (typeof(UnityAction).IsAssignableFrom(types[j]) || typeof(UnityRequestAction).IsAssignableFrom(types[j]))
                    {
                        actionTypes.Add(types[j]);
                    }
                    else if (typeof(CompoundAction).IsAssignableFrom(types[j]))
                    {
                        compoundActionTypes.Add(types[j]);
                    }
                    else if (typeof(UnityPerception).IsAssignableFrom(types[j]))
                    {
                        perceptionTypes.Add(types[j]);
                    }
                    else if (typeof(CompoundPerception).IsAssignableFrom(types[j]))
                    {
                        compoundPerceptionTypes.Add(types[j]);
                    }
                    else if (typeof(NodeDrawer).IsAssignableFrom(types[j]))
                    {
                        var drawerAttribute = types[j].GetCustomAttribute<CustomNodeDrawerAttribute>();
                        if (drawerAttribute != null && typeof(Node).IsAssignableFrom(drawerAttribute.NodeType))
                        {
                            nodeDrawerMainTypeMap[drawerAttribute.NodeType] = types[j];
                        }
                    }
                    else if (typeof(GraphAdapter).IsAssignableFrom(types[j]))
                    {
                        var adapterAttribute = types[j].GetCustomAttribute<CustomGraphAdapterAttribute>();
                        if (adapterAttribute != null && typeof(BehaviourGraph).IsAssignableFrom(adapterAttribute.GraphType))
                        {
                            graphAdapterMainTypeMap[adapterAttribute.GraphType] = types[j];
                        }
                    }
                    else if (typeof(GraphCodeGenerator).IsAssignableFrom(types[j]))
                    {
                        var generatorAttribute = types[j].GetCustomAttribute<CustomGraphCodeGeneratorAttribute>();
                        if (generatorAttribute != null && typeof(BehaviourGraph).IsAssignableFrom(generatorAttribute.GraphType))
                        {
                            graphCodeGeneratorMainTypeMap[generatorAttribute.GraphType] = types[j];
                        }
                    }
                    else if (typeof(Component).IsAssignableFrom(types[j]))
                    {
                        componentMap.TryAdd(types[j].Name, types[j]);
                        c++;
                    }

                }
            }

            BuildNodeDrawerMap(nodeTypes, nodeDrawerMainTypeMap);

            NodeTypes = nodeTypes.ToList();
            GraphAdapterMap = BuildFullGraphTypeMap(graphTypes, graphAdapterMainTypeMap);
            CodeGeneratorMap = BuildFullGraphTypeMap(graphTypes, graphCodeGeneratorMainTypeMap);

            BuildActionHierarchy(actionTypes, compoundActionTypes);
            BuildPerceptionHierarchy(perceptionTypes, compoundPerceptionTypes);

            //  Debug.Log((DateTime.Now - time).TotalMilliseconds);
        }

        private bool IsValidType(Type type)
        {
            if (type.IsAbstract || type.IsGenericType) return false;

            foreach (var constructor in type.GetConstructors())
            {
                if (constructor.GetParameters().Length == 0)
                    return true;
            }

            return false;
        }

        private Dictionary<Type, Type> BuildFullGraphTypeMap(IEnumerable<Type> graphTypes, Dictionary<Type, Type> graphCodeGeneratorMainTypeMap)
        {
            var dict = new Dictionary<Type, Type>();
            foreach (Type graphType in graphTypes)
            {
                var type = graphType;
                bool mainTypeFound = false;
                while (type != typeof(BehaviourGraph) && !mainTypeFound)
                {
                    if (graphCodeGeneratorMainTypeMap.TryGetValue(type, out Type adapterType))
                    {
                        dict[graphType] = adapterType;
                        mainTypeFound = true;
                    }
                    else
                    {
                        type = type.BaseType;
                    }
                }
            }
            return dict;
        }


        private void BuildNodeDrawerMap(IEnumerable<Type> nodeTypes, Dictionary<Type, Type> nodeDrawerMainTypeMap)
        {
            foreach (Type nodeType in nodeTypes)
            {
                var type = nodeType;

                if (type.IsAbstract) continue;

                bool mainTypeFound = false;
                while (type != typeof(Node) && !mainTypeFound)
                {
                    if (nodeDrawerMainTypeMap.TryGetValue(type, out Type drawerType))
                    {
                        NodeDrawerTypeMap[nodeType] = drawerType;
                        mainTypeFound = true;
                    }
                    else
                    {
                        type = type.BaseType;
                    }
                }
            }
        }

        private void BuildActionHierarchy(List<Type> actionTypes, List<Type> compoundActionTypes)
        {
            ActionHierarchy = new EditorHierarchyNode("Actions", typeof(Action));
            ActionHierarchy.Childs.Add(new EditorHierarchyNode(typeof(Framework.CustomAction)));
            ActionHierarchy.Childs.Add(new EditorHierarchyNode(typeof(Framework.SimpleAction)));

            EditorHierarchyNode subgraphNode = new EditorHierarchyNode("Subgraph actions", typeof(SubsystemAction));
            subgraphNode.Childs.Add(new EditorHierarchyNode(typeof(SubgraphAction)));
            subgraphNode.Childs.Add(new EditorHierarchyNode(typeof(AssetSubgraphAction)));
            ActionHierarchy.Childs.Add(subgraphNode);

            Dictionary<string, EditorHierarchyNode> groups = new Dictionary<string, EditorHierarchyNode>();
            List<EditorHierarchyNode> ungroupedActionNodes = new List<EditorHierarchyNode>();

            EditorHierarchyNode requestActionHierarchyNode = new EditorHierarchyNode("Request actions", typeof(UnityRequestAction));

            for (int i = 0; i < actionTypes.Count; i++)
            {
                if (actionTypes[i].IsSubclassOf(typeof(UnityRequestAction)))
                {
                    requestActionHierarchyNode.Childs.Add(new EditorHierarchyNode(actionTypes[i]));
                }
                else
                {
                    var groupAttributes = actionTypes[i].GetCustomAttributes<SelectionGroupAttribute>();
                    EditorHierarchyNode actionTypeNode = new EditorHierarchyNode(actionTypes[i]);

                    foreach (var groupAttribute in groupAttributes)
                    {
                        if (!groups.TryGetValue(groupAttribute.name, out EditorHierarchyNode groupNode))
                        {
                            groupNode = new EditorHierarchyNode(groupAttribute.name, null);
                            groups[groupAttribute.name] = groupNode;
                        }
                        groupNode.Childs.Add(actionTypeNode);
                    }

                    if (groupAttributes.Count() == 0)
                    {
                        ungroupedActionNodes.Add(actionTypeNode);
                    }
                }
            }

            EditorHierarchyNode compoundActionHierarchyNode = new EditorHierarchyNode("Compound actions", null);
            compoundActionHierarchyNode.Childs = compoundActionTypes.FindAll(typeof(CompoundAction).IsAssignableFrom)
                .Select(compoundPerceptionType => new EditorHierarchyNode(compoundPerceptionType)).ToList();

            ActionHierarchy.Childs.Add(compoundActionHierarchyNode);
            EditorHierarchyNode unityActionHierarchyNode = new EditorHierarchyNode("Unity Actions", typeof(UnityAction));
            unityActionHierarchyNode.Childs.AddRange(groups.Values);
            unityActionHierarchyNode.Childs.AddRange(ungroupedActionNodes);
            ActionHierarchy.Childs.Add(unityActionHierarchyNode);
            ActionHierarchy.Childs.Add(requestActionHierarchyNode);
        }

        private void BuildPerceptionHierarchy(List<Type> perceptionTypes, List<Type> compoundPerceptionTypes)
        {
            PerceptionHierarchy = new EditorHierarchyNode("Perceptions", typeof(Perception));
            PerceptionHierarchy.Childs.Add(new EditorHierarchyNode(typeof(CustomPerception)));

            EditorHierarchyNode compoundPerceptionHierarchyNode = new EditorHierarchyNode("Compound perceptions", null);
            compoundPerceptionHierarchyNode.Childs = compoundPerceptionTypes.FindAll(typeof(CompoundPerception).IsAssignableFrom)
                .Select(compoundPerceptionType => new EditorHierarchyNode(compoundPerceptionType)).ToList();

            PerceptionHierarchy.Childs.Add(compoundPerceptionHierarchyNode);

            Dictionary<string, EditorHierarchyNode> groups = new Dictionary<string, EditorHierarchyNode>();
            List<EditorHierarchyNode> ungroupedPerceptionsNodes = new List<EditorHierarchyNode>();

            for (int i = 0; i < perceptionTypes.Count; i++)
            {
                var groupAttributes = perceptionTypes[i].GetCustomAttributes<SelectionGroupAttribute>();
                EditorHierarchyNode actionTypeNode = new EditorHierarchyNode(perceptionTypes[i]);

                foreach (var groupAttribute in groupAttributes)
                {
                    if (!groups.TryGetValue(groupAttribute.name, out EditorHierarchyNode groupNode))
                    {
                        groupNode = new EditorHierarchyNode(groupAttribute.name, null);
                        groups[groupAttribute.name] = groupNode;
                    }
                    groupNode.Childs.Add(actionTypeNode);
                }

                if (groups.Count == 0)
                {
                    ungroupedPerceptionsNodes.Add(actionTypeNode);
                }
            }
            EditorHierarchyNode unityPerceptionHierarchyNode = new EditorHierarchyNode("Unity Perceptions", typeof(UnityPerception));
            unityPerceptionHierarchyNode.Childs.AddRange(groups.Values);
            unityPerceptionHierarchyNode.Childs.AddRange(ungroupedPerceptionsNodes);
            PerceptionHierarchy.Childs.Add(unityPerceptionHierarchyNode);
        }

        public EditorHierarchyNode GetActionHierarchy(Type m_RootType)
        {
            if (m_RootType == null || m_RootType == typeof(Action)) return ActionHierarchy;

            if (!typeof(Action).IsAssignableFrom(m_RootType)) return null;

            Stack<Type> typeStack = new Stack<Type>();
            var baseType = m_RootType;
            while(baseType != typeof(Action))
            {
                typeStack.Push(baseType);
                baseType = baseType.BaseType;
            }

            EditorHierarchyNode currentHierarchyNode = ActionHierarchy;
            while(currentHierarchyNode.Type != m_RootType)
            {
                var type = typeStack.Pop();
                currentHierarchyNode = currentHierarchyNode.FindSubNode(type);
            }

            return currentHierarchyNode;
        }    

        public EditorHierarchyNode GetPerceptionHierarchy(Type m_RootType)
        {
            if (m_RootType == null || m_RootType == typeof(Perception)) return PerceptionHierarchy;

            if (!typeof(Perception).IsAssignableFrom(m_RootType)) return null;

            Stack<Type> typeStack = new Stack<Type>();
            var baseType = m_RootType;
            while (baseType != typeof(Perception))
            {
                typeStack.Push(baseType);
                baseType = baseType.BaseType;
            }

            EditorHierarchyNode currentHierarchyNode = PerceptionHierarchy;
            while (currentHierarchyNode.Type != m_RootType)
            {
                var type = typeStack.Pop();
                currentHierarchyNode = currentHierarchyNode.FindSubNode(type);
            }

            return currentHierarchyNode;
        }
    }
}
