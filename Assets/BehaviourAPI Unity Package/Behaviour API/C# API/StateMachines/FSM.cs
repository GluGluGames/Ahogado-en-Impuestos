using System;

namespace BehaviourAPI.StateMachines
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;

    /// <summary>
    /// Decision system builded as a State machine. Each frame, the current state is executed and check its
    /// transitions.
    /// </summary>
    public class FSM : BehaviourGraph
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override Type NodeType => typeof(FSMNode);

        public override bool CanRepeatConnection => false;

        public override bool CanCreateLoops => true;

        /// <summary>
        /// The last triggered transition in the state machine.
        /// </summary>
        public Transition LastPerformedTransition { get; private set; }

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The current active state of the graph
        /// </summary>
        protected State _currentState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> named <paramref name="name"/> from <paramref name="from"/> state. 
        /// The transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed.
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source status of the transition.</param>
        /// <param name="perception">The perception that the transition checks.</param>
        /// <param name="action">The action performed by the transition.</param>
        /// <param name="flags">The status flags that <paramref name="from"/> must match to check the transition.</param>
        /// <returns>The created <typeparamref name="T"/>.</returns>
        protected T CreateInternalTransition<T>(string name, State from, Perception perception, Action action, StatusFlags flags) where T : Transition, new()
        {
            T transition = CreateNode<T>(name);
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.StatusFlags = flags;
            Connect(from, transition);
            transition.SetSourceState(from);
            from.AddTransition(transition);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="Transition"/> of type <typeparamref name="T"/> named from <paramref name="from"/> state. 
        /// <para>The transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true. </para>
        /// <para>When the transition is triggered, <paramref name="action"/> is performed.</para>
        /// </summary>
        /// <typeparam name="T">The type of the transition.</typeparam>
        /// <param name="from">The source status of the transition.</param>
        /// <param name="perception">The perception that the transition checks.</param>
        /// <param name="action">The action performed by the transition.</param>
        /// <param name="flags">The status flags that <paramref name="from"/> must match to check the transition.</param>
        /// <returns>The created <typeparamref name="T"/>.</returns>
        protected T CreateInternalTransition<T>(State from, Perception perception, Action action, StatusFlags flags) where T : Transition, new()
        {
            T transition = CreateNode<T>();
            transition.SetFSM(this);
            transition.Perception = perception;
            transition.Action = action;
            transition.StatusFlags = flags;
            Connect(from, transition);
            transition.SetSourceState(from);
            from.AddTransition(transition);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="State"/> that executes <paramref name="action"/> when is the current state.
        /// </summary>
        /// <param name="action">The action this state executes.</param>
        /// <returns>The <see cref="State"/> created.</returns>
        public State CreateState(Action action = null)
        {
            State state = CreateNode<State>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Create a new <see cref="State"/> named <paramref name="name"/> that executes <paramref name="action"/> when is the current state of the.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action executed by the state.</param>
        /// <returns>The <see cref="State"/> created.</returns>
        public State CreateState(string name, Action action = null)
        {
            State state = CreateNode<State>(name);
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Use this method to create custom types of states.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns>The state of type <typeparamref name="T"/> created.</returns>
        public T CreateState<T>(Action action = null) where T : State, new()
        {
            T state = CreateNode<T>();
            state.Action = action;
            return state;
        }

        /// <summary>
        /// Use this method to create custom type of states with name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name of the node</param>
        /// <param name="action">The action executed by the state.</param>
        /// <returns>The state of type <typeparamref name="T"/> created.</returns>
        public T CreateState<T>(string name, Action action = null) where T : State, new()
        {
            T state = CreateNode<T>(name);
            state.Action = action;
            return state;
        }


        /// <summary>
        /// Create a new <see cref="ProbabilisticState"/> that executes <paramref name="action"/> when is the current state of the fsm.
        /// </summary>
        /// <param name="action">The action executed by the state.</param>
        /// <returns>The <see cref="ProbabilisticState"/> created.</returns>
        public ProbabilisticState CreateProbabilisticState(Action action = null)
        {
            return CreateState<ProbabilisticState>(action);
        }

        /// <summary>
        /// Create a new <see cref="ProbabilisticState"/> named <paramref name="name"/> that executes <paramref name="action"/> when is the current state of the fsm.
        /// </summary>
        /// <param name="name">The name of this node.</param>
        /// <param name="action">The action executed by the state.</param>
        /// <returns>The <see cref="ProbabilisticState"/> created.</returns>
        public ProbabilisticState CreateProbabilisticState(string name, Action action = null)
        {
            return CreateState<ProbabilisticState>(name, action);
        }


        /// <summary>
        /// Create a new <see cref="StateTransition"/> named <paramref name="name"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and fsm current status changes to <paramref name="to"/> state.
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="StateTransition"/> created.</returns>

        public StateTransition CreateTransition(string name, State from, State to, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            StateTransition transition = CreateInternalTransition<StateTransition>(name, from, perception, action, statusFlags);
            Connect(transition, to);
            transition.SetTargetState(to);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="StateTransition"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches <paramref name="flags"/>. 
        /// If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and fsm current status changes to <paramref name="to"/> state.
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="StateTransition"/> created.</returns>
        public StateTransition CreateTransition(State from, State to, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            StateTransition transition = CreateInternalTransition<StateTransition>(from, perception, action, statusFlags);
            Connect(transition, to);
            transition.SetTargetState(to);
            return transition;
        }



        /// <summary>
        /// Create a new <see cref="ExitTransition"/> named <paramref name="name"/> from the state <paramref name="from"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches <paramref name="flags"/>. 
        /// If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and the fsm execution will end with <paramref name="exitStatus"/>.
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="ExitTransition"/> created.</returns>
        public ExitTransition CreateExitTransition(string name, State from, Status exitStatus, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            ExitTransition transition = CreateInternalTransition<ExitTransition>(name, from, perception, action, statusFlags);
            transition.ExitStatus = exitStatus;
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="ExitTransition"/> from the state <paramref name="from"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches <paramref name="flags"/>. 
        /// If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and the fsm execution will end with <paramref name="exitStatus"/>.
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="ExitTransition"/> created.</returns>
        public ExitTransition CreateExitTransition(State from, Status exitStatus, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            ExitTransition transition = CreateInternalTransition<ExitTransition>(from, perception, action, statusFlags);
            transition.ExitStatus = exitStatus;
            return transition;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Change the entry state of the fsm.
        /// </summary>
        /// <param name="state">The new entry state.</param>
        public void SetEntryState(State state)
        {
            StartNode = state;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Set the entry state as the current state and start its execution.
        /// </summary>
        /// <exception cref="EmptyGraphException">If this graph has no nodes or the start node is not a state. </exception>
        protected override void OnStarted()
        {
            if (!(StartNode is State state))
                throw new EmptyGraphException(this);

            _currentState = state;
            _currentState.OnStarted();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Update the current state.
        /// </summary>
        protected override void OnUpdated()
        {
            _currentState?.OnUpdated();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stop the current state.
        /// </summary>
        protected override void OnStopped()
        {
            _currentState?.OnStopped();
        }

        protected override void OnPaused()
        {
            _currentState?.OnPaused();
        }

        protected override void OnUnpaused()
        {
            _currentState?.OnUnpaused();
        }

        /// <summary>
        /// Change the current state of the fsm.
        /// </summary>
        /// <param name="state">The selected state.</param>
        /// <param name="transition">The transition that triggered the change.</param>
        public virtual void SetCurrentState(State state, Transition transition)
        {
            if (LastPerformedTransition != null)
                LastPerformedTransition.SourceStateLastStatus = Status.None;

            LastPerformedTransition = transition;
            _currentState?.OnStopped();
            _currentState = state;
            _currentState?.OnStarted();
        }

        /// <summary>
        /// Check if <paramref name="state"/> is the current state of the fsm.
        /// </summary>
        /// <param name="state">The state checked.</param>
        /// <returns>true if <paramref name="state"/> is the current state, false otherwise.</returns>
        public bool IsCurrentState(State state) => state != null && _currentState == state;

        #endregion
    }
}
