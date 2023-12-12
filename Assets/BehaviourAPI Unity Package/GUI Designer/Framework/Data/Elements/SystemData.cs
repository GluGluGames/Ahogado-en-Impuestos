using BehaviourAPI.Core;
using BehaviourAPI.Core.Perceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    /// <summary>
    /// Class that serializes a behaviour system
    /// </summary>
    [Serializable]
    public class SystemData
    {
        /// <summary>
        /// List of the graphs stored in the system. The first node is the main one.
        /// </summary>
        public List<GraphData> graphs = new List<GraphData>();

        /// <summary>
        /// List of push perceptions.
        /// </summary>
        public List<PushPerceptionData> pushPerceptions = new List<PushPerceptionData>();

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SystemData()
        {
        }

        /// <summary>
        /// Create a new <see cref="SystemData"/> by a collection of named <see cref="BehaviourGraph"/>.
        /// </summary>
        /// <param name="graphMap"></param>
        public SystemData(Dictionary<BehaviourGraph, string> graphMap)
        {
            foreach (var graph in graphMap)
            {
                graphs.Add(new GraphData(graph.Key, graph.Value));
            }
        }

        /// <summary>
        /// Build the main <see cref="BehaviourGraph"/> using the serialized data.
        /// </summary>
        /// <returns>The <see cref="BSBuildingResults"/>.</returns>
        public BSBuildingResults BuildSystem(Component runner)
        {
            if (graphs.Count == 0)
            {
                Debug.LogWarning("BUILD ERROR: This system has no graphs");
                return null;
            }

            BSBuildingInfo data = new BSBuildingInfo(runner, this);

            for (int i = 0; i < graphs.Count; i++)
            {
                graphs[i].Build(data);
            }

            for (int i = 0; i < pushPerceptions.Count; i++)
            {
                pushPerceptions[i].Build(data);
            }

            return new BSBuildingResults(this, runner);
        }

        public BehaviourGraph BuildSystem(BSBuildingInfo rootData, string subSystemName)
        {
            if (graphs.Count == 0)
            {
                Debug.LogWarning("BUILD ERROR: This system has no graphs");
                return null;
            }

            BSBuildingInfo data = new BSBuildingInfo(rootData, this, subSystemName);

            for (int i = 0; i < graphs.Count; i++)
            {
                graphs[i].Build(data);
                graphs[i].name = data.SubsystemName + graphs[i].name;
                data.RootSystemData.graphs.Add(graphs[i]);
            }

            for (int i = 0; i < pushPerceptions.Count; i++)
            {
                pushPerceptions[i].Build(data);
                graphs[i].name = data.SubsystemName + graphs[i].name;
                data.RootSystemData.pushPerceptions.Add(pushPerceptions[i]);
            }

            return graphs.FirstOrDefault()?.graph;
        }

        public bool CheckCyclicReferences()
        {
            BSValidationInfo validationInfo = new BSValidationInfo();
            return CheckCyclicReferences(validationInfo);
        }

        public bool CheckCyclicReferences(BSValidationInfo bSValidationInfo)
        {
            bSValidationInfo.systemStack.Add(this);
            foreach (var graph in graphs)
            {
                foreach (var node in graph.nodes)
                {
                    foreach (var reference in node.references)
                    {
                        if (reference.Value is IBuildable buildable)
                        {
                            var result = buildable.Validate(bSValidationInfo);
                            if (!result)
                            {
                                bSValidationInfo.systemStack.Remove(this);
                                return result;
                            }
                        }
                    }
                }
            }
            bSValidationInfo.systemStack.Remove(this);
            return true;
        }

        public Dictionary<string, NodeData> GetNodeIdMap()
        {
            var dict = new Dictionary<string, NodeData>();
            foreach (GraphData graph in graphs)
            {
                foreach (NodeData node in graph.nodes)
                {
                    dict.Add(node.id, node);
                }
            }
            return dict;
        }

        public bool ValidateReferences()
        {
            bool referencesChanged = false;
            foreach(GraphData graphData in graphs)
            {
                referencesChanged |= graphData.ValidateReferences();
            }
            return referencesChanged;
        }
    }
}
