namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Utility node that finish the execution of the utility system when is selected.
    /// </summary>
    public class UtilityExitNode : UtilityExecutableNode
    {
        #region ------------------------------------------ Properties ----------------------------------------

        /// <summary>
        /// The value that the utility system will end up with when this node is executed.
        /// </summary>
        public Status ExitStatus;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Exit the utility system with <see cref="ExitStatus"/> value.
        /// </summary>
        public override void OnStarted()
        {
            if (ExitStatus != Status.None) Status = ExitStatus;
            else ExitStatus = Status.Running;
            BehaviourGraph.Finish(ExitStatus);
        }

        /// <summary>
        /// <inheritdoc/>
        /// This method is empty because is only executed the frame the utility system exits.
        /// </summary>
        public override void OnUpdated()
        {
            return;
        }

        #endregion
    }
}
