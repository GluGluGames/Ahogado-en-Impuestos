using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Editor
{
    public class EditorHierarchyNode
    {

        public string name;
        public Type Type;
        public List<EditorHierarchyNode> Childs = new List<EditorHierarchyNode>();

        public EditorHierarchyNode(Type type)
        {
            this.name = type.Name.CamelCaseToSpaced();
            Type = type;
        }

        public EditorHierarchyNode(string name, Type type = null)
        {
            this.name = name;
            Type = type;
        }

        public static EditorHierarchyNode CreateGroupedHierarchyNode(List<System.Type> allTypes, Type baseType,
            string hierarchyName, bool grouped = false)
        {
            var types = GetValidSubTypes(baseType, allTypes);
            var hierarchyNode = new EditorHierarchyNode(hierarchyName, baseType);

            if (grouped)
            {
                Dictionary<string, EditorHierarchyNode> groups = new Dictionary<string, EditorHierarchyNode>();
                List<EditorHierarchyNode> ungroupedTypeNodes = new List<EditorHierarchyNode>();

                foreach (var type in types)
                {
                    IEnumerable<SelectionGroupAttribute> group = type.GetCustomAttributes<SelectionGroupAttribute>();
                    EditorHierarchyNode typeNode = new EditorHierarchyNode(type);

                    if (group.Count() == 0)
                    {
                        ungroupedTypeNodes.Add(typeNode);
                    }
                    else
                    {
                        foreach (var attribute in group)
                        {
                            var groupName = attribute.name;
                            var groupNode = new EditorHierarchyNode(groupName, null);
                            groups.TryAdd(groupName, groupNode);
                            groups[groupName].Childs.Add(typeNode);
                        }
                    }
                }
                IEnumerable<EditorHierarchyNode> allNodes = groups.Values.Union(ungroupedTypeNodes);
                hierarchyNode.Childs = allNodes.ToList();
            }
            else
            {
                IEnumerable<EditorHierarchyNode> allNodes = types.Select(t => new EditorHierarchyNode(t));
                hierarchyNode.Childs = allNodes.ToList();
            }
            return hierarchyNode;
        }

        public static EditorHierarchyNode CreateCustomHierarchyNode(List<System.Type> allTypes, Type baseType,
            string hierarchyName, List<Type> mainTypes, List<Type> bannedTypes)
        {
            List<EditorHierarchyNode> mainNodes = new List<EditorHierarchyNode>();
            foreach (var mainType in mainTypes)
            {
                var nodeSubTypes = GetValidSubTypes(mainType, allTypes).ToList();
                if (!mainType.IsAbstract) nodeSubTypes.Add(mainType);

                nodeSubTypes = nodeSubTypes.Except(bannedTypes).ToList();

                if (nodeSubTypes.Count == 1)
                {
                    EditorHierarchyNode mainTypeNode = new EditorHierarchyNode(nodeSubTypes.First());
                    mainNodes.Add(mainTypeNode);
                }
                else if (nodeSubTypes.Count > 1)
                {
                    EditorHierarchyNode mainTypeNode = new EditorHierarchyNode(mainType.Name.CamelCaseToSpaced() + "(s)", mainType);
                    mainTypeNode.Childs = nodeSubTypes.Select(t => new EditorHierarchyNode(t)).ToList();
                    mainNodes.Add(mainTypeNode);
                }
            }
            EditorHierarchyNode rootNode = new EditorHierarchyNode(hierarchyName, baseType);
            rootNode.Childs = mainNodes;
            return rootNode;
        }

        public static IEnumerable<System.Type> GetValidSubTypes(System.Type type, List<System.Type> allTypes)
        {
            return allTypes.FindAll(t => t.IsSubclassOf(type) &&
                !t.IsAbstract &&
                t.GetConstructors().Any(c => c.GetParameters().Length == 0));
        }

        public EditorHierarchyNode FindSubNode(Type type)
        {
            return Childs.Find(e => e.Type == type);
        }
    }
}
