using System;
using System.Collections.Generic;

namespace BehaviourAPI.StateMachines.StackFSMs
{
    using Core;

    /// <summary>
    /// Base class for stack fsm transitions 
    /// </summary>
    public abstract class StackTransition : Transition
    {
        #region ------------------------------------------ Properties ----------------------------------------

        public override Type GraphType => typeof(StackFSM);

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        /// <summary>
        /// The stack fsm of this transition.
        /// </summary>
        protected StackFSM _stackFSM;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the stack fsm of the transition
        /// </summary>
        /// <param name="stackFSM">The stack fsm.</param>
        protected internal void SetStackFSM(StackFSM stackFSM) => _stackFSM = stackFSM;

        protected override void BuildConnections(List<Node> parents, List<Node> children)
        {
            _stackFSM = BehaviourGraph as StackFSM;

            if (_stackFSM == null)
                throw new Exception("Stack transitions can only be used in StackFSMs");

            base.BuildConnections(parents, children);
        }

        #endregion
    }
}
