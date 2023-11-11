using System;
using System.Collections.Generic;

namespace BehaviourAPI.StateMachines
{
    using Core;

    /// <summary>
    /// Transition between two states.
    /// </summary>
    public class StateTransition : Transition
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => 1;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        State _targetState;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the target state of the transition
        /// </summary>
        /// <param name="target">The target state.</param>
        protected internal void SetTargetState(State target) => _targetState = target;

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            base.BuildConnections(parents, children);

            if (children.Count > 0 && children[0] is State to)
                _targetState = to;
            else
                throw new ArgumentException();
        }


        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Changes the current state of the fsm to the target state.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        /// <exception cref="MissingChildException">If target state is null</exception>
        public override bool Perform()
        {
            bool canBePerformed = base.Perform();
            if (canBePerformed)
            {
                if (_targetState == null) throw new MissingChildException(this, "The target state can't be null.");
                _fsm.SetCurrentState(_targetState, this);
            }
            return canBePerformed;
        }

        #endregion
    }
}
