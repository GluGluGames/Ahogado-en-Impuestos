using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using BehaviourAPI.Core;
    using System;

    /// <summary>
    /// Base class for all behaviour system runners
    /// </summary>
    /// 
    public abstract class BehaviourRunner : MonoBehaviour
    {

        [Tooltip("Restart execution when finished?")]
        public bool executeOnLoop;

        [Tooltip("What method execute when the runner is disabled/enabled?")]
        public ExecutionInterruptOptions interruptOptions;

        bool _systemRunning;

        BehaviourGraph _executionGraph;

        /// <summary>
        /// Override this method to set the context that the graph will use.
        /// </summary>
        /// <returns>The context created.</returns>
        protected virtual UnityExecutionContext CreateContext()
        {
            UnityExecutionContext context = new UnityExecutionContext(this);
            return context;
        }

        /// <summary>
        /// Gets the main graph that will be executed.
        /// </summary>
        /// <returns>The execution <see cref="BehaviourGraph"></see></returns>
        protected abstract BehaviourGraph CreateGraph();


        /*
         * Unity events:
         * Dont override this methods in subclasses. Otherwise the events in this script won't be called.
         */

        private void Awake() => Init();

        private void Start() => OnStarted();

        private void Update() => OnUpdated();

        private void OnEnable() => OnDisableSystem();

        private void OnDisable() => OnEnableSystem();


        /// <summary>
        /// Called in awake event.
        /// Create the behaviour graph and set the context.
        /// </summary>
        protected virtual void Init()
        {
            _executionGraph = CreateGraph();

            if (_executionGraph != null)
            {
                UnityExecutionContext context = CreateContext();
                _executionGraph.SetExecutionContext(context);
            }
        }

        /// <summary>
        /// Called in start event.
        /// Starts the graph execution.
        /// </summary>
        protected virtual void OnStarted()
        {
            if (_executionGraph != null)
            {
                _executionGraph.Start();
                _systemRunning = true;
            }
            else
            {
                Debug.LogWarning("EXECUTION ERROR: This runner has not graph attached.", this);
                Destroy(this);
            }
        }

        /// <summary>
        /// Called in update event.
        /// Update the graph executions and restart it when finish if executeOnLoop flag is raised.
        /// </summary>
        protected virtual void OnUpdated()
        {
            if (_executionGraph != null)
            {
                if (_executionGraph.Status != Status.Running) return;

                _executionGraph.Update();

                if (_executionGraph.Status != Status.Running)
                {
                    if(executeOnLoop)
                    {
                        _executionGraph.Restart();
                    }
                    else
                    {
                        _executionGraph.Stop();
                    }
                }
            }
            else
            {
                Debug.LogWarning("EXECUTION ERROR: This runner has not graphs attached.", this);
                Destroy(this);
            }
        }

        /// <summary>
        /// Called in ondisable event.
        /// The method called in this event depends on dontStopOnDisable configuration.
        /// </summary>
        protected virtual void OnEnableSystem()
        {
            if (!_systemRunning || _executionGraph == null)
                return;

            bool interrupted = _executionGraph.Status == Status.Running;

            if (interrupted)
            {
                if (interruptOptions == ExecutionInterruptOptions.Pause && _executionGraph.Status == Status.Running)
                {
                    _executionGraph.Pause();
                }
                else if (interruptOptions == ExecutionInterruptOptions.Stop)
                {
                    _executionGraph.Stop();
                }
            }
        }

        /// <summary>
        /// Called in onenable event.
        /// The method called in this event depends on dontStopOnDisable configuration.
        /// </summary>
        protected virtual void OnDisableSystem()
        {
            if (!_systemRunning || _executionGraph == null)
                return;

            bool interrupted = _executionGraph.Status == Status.Running;

            if (interrupted)
            {
                if (interruptOptions == ExecutionInterruptOptions.Pause)
                {
                    _executionGraph.Unpause();
                }
            }
            else
            {
                _executionGraph.Start();
            }
        }
    }
}
