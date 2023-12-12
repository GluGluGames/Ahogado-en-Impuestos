using System;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.GUIDesigner.Runtime
{
    using Core;
    using Core.Perceptions;
    using Framework;

    /// <summary>
    /// Base class of components that use an editable behavior system.
    /// </summary>
    public abstract class DataBehaviourRunner : UnityToolkit.BehaviourRunner, IBehaviourSystem
    {
        #region -------------------------------- private fields ---------------------------------

        SystemData _executionSystem;

        [SerializeField] BSRuntimeEventHandler _eventHandler;

        #endregion

        #region --------------------------------- properties ----------------------------------

        /// <summary>
        /// The execution main graph
        /// </summary>
        public BehaviourGraph MainGraph { get; private set; }

        public abstract SystemData Data { get; }

        public UnityEngine.Object ObjectReference => this;

        #endregion

        #region ------------------------------ Execution Methods ------------------------------

        /// <summary>
        /// Create the graphs and register in the event handler.
        /// </summary>
        protected override void Init()
        {
            _eventHandler.Context = this;
            base.Init();

            foreach(GraphData graph in _executionSystem.graphs)
            {
                _eventHandler.RegisterEvents(graph.graph, graph.name);
            }
        }
        protected sealed override BehaviourGraph CreateGraph()
        {
            _executionSystem = GetEditedSystemData();

            BSBuildingResults buildedData = _executionSystem.BuildSystem(this);
            ModifyGraphs(buildedData.GraphMap, buildedData.PushPerceptionMap);
            return buildedData.MainGraph;
        }

        /// <summary>
        /// Overrides this method to modify the created system in code just after build it.
        /// Used to modify or assign actions, not create new nodes or graphs.
        /// </summary>
        protected virtual void ModifyGraphs(Dictionary<string, BehaviourGraph> graphMap, Dictionary<string, PushPerception> pushPerceptionMap)
        {
            return;
        }

        /// <summary>
        /// Get the system created with editor tools.
        /// </summary>
        protected abstract SystemData GetEditedSystemData();

        #endregion
    }
}
