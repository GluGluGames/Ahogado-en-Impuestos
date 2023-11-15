using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Framework
{
    using Core;

    public class BSBuildingResults
    {
        public BehaviourGraph MainGraph { get; private set; }
        public Dictionary<string, BehaviourGraph> GraphMap { get; private set; }
        public Dictionary<string, PushPerception> PushPerceptionMap { get; private set; }

        public BSBuildingResults(BehaviourGraph mainGraph, Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            MainGraph = mainGraph;
            GraphMap = graphMap;
            PushPerceptionMap = pushPerceptionMap;
        }

        public BSBuildingResults(SystemData systemData, Component runner)
        {
            GraphMap = new Dictionary<string, BehaviourGraph>();

            for(int i = 0; i < systemData.graphs.Count; i++) 
            {
                if (!string.IsNullOrWhiteSpace(systemData.graphs[i].name))
                {
                    if (!GraphMap.TryAdd(systemData.graphs[i].name, systemData.graphs[i].graph))
                        Debug.LogWarning($"BUILD WARNING: Graph \"{systemData.graphs[i].name}\" wasn't added to dictionary because a graph with the same name was added before.", runner);
                }
                else
                {
                    Debug.LogWarning($"BUILD WARNING: Graph \"{systemData.graphs[i].name}\" wasn't added to dictionary because the name is not valid", runner);
                }
            }

            PushPerceptionMap = new Dictionary<string, PushPerception>();

            for (int i = 0; i < systemData.pushPerceptions.Count; i++)
            {
                if (!string.IsNullOrWhiteSpace(systemData.pushPerceptions[i].name))
                {
                    if (!PushPerceptionMap.TryAdd(systemData.pushPerceptions[i].name, systemData.pushPerceptions[i].pushPerception))
                    {
                        Debug.LogWarning($"ERROR: Push perception \"{systemData.pushPerceptions[i].name}\" wasn't added to dictionary because a push perception with the same name was added before.", runner);
                    }
                }
                else
                {
                    Debug.LogWarning($"ERROR: Push perception \"{systemData.pushPerceptions[i].name}\" wasn't added to dictionary because the name is not valid", runner);
                }
            }

            MainGraph = systemData.graphs.FirstOrDefault().graph;
        }
    }
}
