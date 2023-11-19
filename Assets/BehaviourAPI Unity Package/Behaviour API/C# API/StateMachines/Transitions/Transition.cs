using System;
using System.Collections.Generic;

namespace BehaviourAPI.StateMachines
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;

    /// <summary>
    /// Base class for transitions in fsm.
    /// </summary>
    public abstract class Transition : FSMNode, IPushActivable
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type ChildType => typeof(State);

        public override int MaxInputConnections => 1;

        /// <summary>
        /// Event triggered when the action is performed.
        /// </summary>
        public System.Action TransitionTriggered { get; set; } = delegate { };

        /// <summary>
        /// Event triggered when source state's last status changed.
        /// </summary>
        public Action<Status> SourceStateLastStatusChanged { get; set; } = delegate { };

        /// <summary>
        /// The status that the source state had when this transition was triggered
        /// </summary>
        public Status SourceStateLastStatus
        {
            get => _sourceStateLastStatus;
            set
            {
                _sourceStateLastStatus = value;
                SourceStateLastStatusChanged?.Invoke(value);
            }
        }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The perception checked by this transition.
        /// </summary>
        public Perception Perception;

        /// <summary>
        /// The action executed by the transition when is performed.
        /// </summary>
        public Action Action;

        /// <summary>
        /// The status flags that the source state must match to check the transition.
        /// </summary>
        public StatusFlags StatusFlags;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        /// <summary>
        /// The fsm of this transition.
        /// </summary>
        protected FSM _fsm;

        /// <summary>
        /// The source state of this transition.
        /// </summary>
        protected State _sourceState;

        Status _sourceStateLastStatus;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the fsm of the transition.
        /// </summary>
        /// <param name="fsm">The fsm.</param>
        protected internal void SetFSM(FSM fsm) => _fsm = fsm;

        /// <summary>
        /// Set the source state of the transition.
        /// </summary>
        /// <param name="source">The source state.</param>
        protected internal void SetSourceState(State source) => _sourceState = source;

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            _fsm = BehaviourGraph as FSM;

            if (parents.Count > 0 && parents[0] is State from)
                _sourceState = from;
            else
                throw new ArgumentException();
        }

        public override object Clone()
        {
            var node = (Transition)base.Clone();
            node.Action = (Action)Action?.Clone();
            node.Perception = (Perception)Perception?.Clone();

            if (SourceStateLastStatusChanged != null)
                node.SourceStateLastStatusChanged = (Action<Status>)SourceStateLastStatusChanged.Clone();

            if (TransitionTriggered != null)
                node.TransitionTriggered = (System.Action)TransitionTriggered.Clone();

            return node;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Initialize the perception
        /// </summary>
        public void Start() => Perception?.Initialize();

        /// <summary>
        /// Reset the perception
        /// </summary>
        public void Stop() => Perception?.Reset();

        /// <summary>
        /// Pauses the perception
        /// </summary>
        public void OnPaused() => Perception?.Pause();

        /// <summary>
        /// Unpauses the perception
        /// </summary>
        public void OnUnpaused() => Perception?.Unpause();
        /// <summary>
        /// Check the perception if exists.
        /// </summary>
        /// <returns>The value of the perception or true if doesn't exist.</returns>
        public virtual bool Check()
        {
            return Perception?.Check() ?? true;
        }

        /// <summary>
        /// If the source state is the current state of the fsm, executes the action.
        /// </summary>
        /// <returns>True if the source state is the current state and the action is performed, false otherwise.</returns>
        public virtual bool Perform()
        {
            if (_fsm != null && !_fsm.IsCurrentState(_sourceState)) return false;

            if (Action != null)
            {
                Action.Start();
                Action.Update();
                Action.Stop();
            }

            TransitionTriggered?.Invoke();

            if (_sourceState != null)
                SourceStateLastStatus = _sourceState.Status;

            return true;
        }

        /// <summary>
        /// Perform the transition externally.
        /// </summary>
        public void Fire(Status status) => Perform();

        public override void SetExecutionContext(ExecutionContext context)
        {
            Action?.SetExecutionContext(context);
            Perception?.SetExecutionContext(context);
        }

        #endregion
    }
}
