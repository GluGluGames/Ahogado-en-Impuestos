using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{ 
    using Core;
    using Framework;

    /// <summary>
    /// Component used to debug a behaviour system created directly by code.
    /// </summary>   
    public class BSRuntimeDebugger : MonoBehaviour, IBehaviourSystem
    {
        [SerializeField] BSRuntimeEventHandler _eventHandler = new BSRuntimeEventHandler();

        public SystemData Data { get; private set; }

        public Object ObjectReference => null;

        private void Awake()
        {
            _eventHandler.Context = this;
        }

        public void RegisterGraph(BehaviourGraph behaviourGraph, string name = "")
        {
            if(Data == null) Data = new SystemData();

            GraphData graphData = new GraphData(behaviourGraph, name);
            Data.graphs.Add(graphData);
            _eventHandler.RegisterEvents(behaviourGraph, name);
        }

        public void UnregisterGraph(BehaviourGraph behaviourGraph)
        {
            if (Data == null) return;
        }
    }
}
