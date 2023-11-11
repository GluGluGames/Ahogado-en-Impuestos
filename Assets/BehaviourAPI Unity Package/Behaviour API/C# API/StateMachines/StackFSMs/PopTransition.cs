namespace BehaviourAPI.StateMachines.StackFSMs
{
    /// <summary>
    /// Stack Transition that returns to the last state saved in the stack of the stack fsm.
    /// </summary>
    public class PopTransition : StackTransition
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxOutputConnections => 0;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Returns to the last state saved in the stack of the stack fsm.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override bool Perform()
        {
            bool canBePerformed = base.Perform();
            if (canBePerformed) _stackFSM.Pop(this);
            return canBePerformed;
        }

        #endregion
    }
}
