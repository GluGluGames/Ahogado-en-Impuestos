namespace BehaviourAPI.BehaviourTrees
{
    using Core;
    

    /// <summary>
    /// Node that inverts the result returned by its child node (Success/Failure).
    /// </summary>

    public class InverterNode : DirectDecoratorNode
    {
        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Get the inverted value of <paramref name="childStatus"/>.
        /// </summary>
        /// <param name="childStatus"><inheritdoc/></param>
        /// <returns>Success if <paramref name="childStatus"/> is Failure, Failure if <paramref name="childStatus"/> is success, Running otherwise.</returns>
        protected override Status ModifyStatus(Status childStatus)
        {
            return childStatus.Inverted();
        }

        #endregion
    }
}