using System.Collections.Generic;

namespace BehaviourAPI.StateMachines.StackFSMs
{
    using Core;
    using Core.Actions;
    using Core.Perceptions;

    /// <summary>
    /// Subclass of fsm that stores its actived states in a stack, and can push and pop these states.
    /// </summary>
    public class StackFSM : FSM
    {
        #region -------------------------------------- Private variables -------------------------------------

        Stack<State> _stateStack = new Stack<State>();

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Create a new <see cref="PopTransition"/> named <paramref name="name"/> from <paramref name="from"/> state. 
        /// The transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and the fsm returns to the last state in the stack..
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="PopTransition"/> created.</returns>
        public PopTransition CreatePopTransition(string name, State from, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            PopTransition transition = CreateInternalTransition<PopTransition>(name, from, perception, action, statusFlags);
            transition.SetStackFSM(this);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="PushTransition"/> named <paramref name="name"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and fsm current status changes to <paramref name="to"/> state, pushing the last state in the stack.
        /// </summary>
        /// <param name="name">The name of the transition.</param>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="PushTransition"/> created.</returns>
        public PushTransition CreatePushTransition(string name, State from, State to, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            PushTransition transition = CreateInternalTransition<PushTransition>(name, from, perception, action, statusFlags);
            Connect(transition, to);
            transition.SetTargetState(to);
            transition.SetStackFSM(this);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="PopTransition"/> from <paramref name="from"/> state. 
        /// The transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and the fsm returns to the last state in the stack..
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="PopTransition"/> created.</returns>
        public PopTransition CreatePopTransition(State from, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            PopTransition transition = CreateInternalTransition<PopTransition>(from, perception, action, statusFlags);
            transition.SetStackFSM(this);
            return transition;
        }

        /// <summary>
        /// Create a new <see cref="PushTransition"/> that goes from the state <paramref name="from"/> to the state <paramref name="to"/>.
        /// This transition will check <paramref name="perception"/> when <paramref name="from"/> Status matches
        /// <paramref name="flags"/>. If <paramref name="perception"/> is null, check always return true.
        /// When the transition is triggered, <paramref name="action"/> is performed and fsm current status changes to <paramref name="to"/> state, pushing the last state in the stack.
        /// </summary>
        /// <param name="from">The source state of the transition and it's parent node.</param>
        /// <param name="to">The target state of the transition and it's child node.</param>
        /// <param name="perception">The perception checked by the transition.</param>
        /// <param name="action">The action executed by the transition.</param>
        /// <param name="statusFlags">The status that the source state can have to check the perception. If none, the transition will never be checked.</param>
        /// <returns>The <see cref="PushTransition"/> created.</returns>
        public PushTransition CreatePushTransition(State from, State to, Perception perception = null, Action action = null, StatusFlags statusFlags = StatusFlags.Active)
        {
            PushTransition transition = CreateInternalTransition<PushTransition>(from, perception, action, statusFlags);
            Connect(transition, to);
            transition.SetTargetState(to);
            transition.SetStackFSM(this);
            return transition;
        }

        public override object Clone()
        {
            var fsm = (StackFSM)base.Clone();
            fsm._stateStack = new Stack<State>();
            return fsm;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Change the current state of the fsm and save the last in the stack.
        /// </summary>
        /// <param name="targetState">The new current state.</param>
        /// <param name="pushTransition">The push transition that triggers this method.</param>
        public void Push(State targetState, PushTransition pushTransition)
        {
            _stateStack.Push(_currentState);
            SetCurrentState(targetState, pushTransition);
        }

        /// <summary>
        /// Return to the last state saved in the stack, if exists.
        /// </summary>
        /// <param name="popTransition">The pop transition that triggers this method.</param>
        public void Pop(PopTransition popTransition)
        {
            if (_stateStack.Count == 0) return;

            var targetState = _stateStack.Pop();
            SetCurrentState(targetState, popTransition);
        }

        /// <summary>
        /// <inheritdoc/>
        /// Clear the stack.
        /// </summary>
        protected override void OnStopped()
        {
            base.OnStopped();
            _stateStack.Clear();
        }

        #endregion
    }
}
