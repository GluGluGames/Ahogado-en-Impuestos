namespace BehaviourAPI.StateMachines
{
    using Core;
    /// <summary>
    /// Transition that end the execution of the fsm.
    /// </summary>
    public class ExitTransition : Transition
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxOutputConnections => 0;

        #endregion

        #region -------------------------------------------- Fields ------------------------------------------

        /// <summary>
        /// The value that the state machine will end up with when this transition performs.
        /// </summary>
        public Status ExitStatus;

        #endregion


        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Finish the fsm execution with <see cref="ExitStatus"/>. 
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override bool Perform()
        {
            bool canBePerformed = base.Perform();
            if (canBePerformed) _fsm.Finish(ExitStatus);
            return canBePerformed;
        }

        #endregion
    }
}
