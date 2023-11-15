namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// Node that execute its child node until returns a given value.
    /// </summary>
    public class LoopUntilNode : DirectDecoratorNode
    {
        #region ----------------------------------------- Properties -----------------------------------------

        int _currentIterations;

        #endregion

        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// The status that the child node must reach to end the loop.
        /// </summary>
        public Status TargetStatus = Status.Success;

        /// <summary>
        /// The maximum number of times that the child node can end its execution without end the decorator.
        /// if its value is -1 this number is infinite.
        /// </summary>
        public int MaxIterations = -1;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------
        
        /// <summary>
        /// Set the <see cref="TargetStatus"/> value to <paramref name="status"/>.
        /// </summary>
        /// <param name="iterations">The new target status value.</param>
        /// <returns>The <see cref="LoopUntilNode"/> itself.</returns>
        public LoopUntilNode SetTargetStatus(Status status)
        {
            TargetStatus = status;
            return this;
        }

        /// <summary>
        /// Set the <see cref="MaxIterations"/> value to <paramref name="maxIterations"/>.
        /// </summary>
        /// <param name="maxIterations">The new max iterations value.</param>
        /// <returns>The <see cref="LoopUntilNode"/> itself.</returns>
        public LoopUntilNode SetMaxIterations(int maxIterations)
        {
            MaxIterations = maxIterations;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Reset the current iterations.
        /// </summary>
        public override void OnStarted()
        {
            base.OnStarted();
            _currentIterations = 0;
        }

        /// <summary>
        /// <inheritdoc/> If the child execution ends increase the current iteration count. If this count reaches <see cref="MaxIterations"/>
        /// or if the child status is equal to <see cref="TargetStatus"/> return the status of the child, otherwise restart the child and return Running.
        /// </summary>
        /// <param name="childStatus">The current child status.</param>
        /// <returns>Running if the iteration count and the target value are not reached, <paramref name="childStatus"/> otherwise.</returns>
        protected override Status ModifyStatus(Status childStatus)
        {
            // If child execution ends without the target value, restart until currentIterations == MaxIterations
            if (childStatus == TargetStatus.Inverted())
            {
                _currentIterations++;
                if (_currentIterations != MaxIterations)
                {
                    // Restart the node execution
                    childStatus = Status.Running;
                    m_childNode.OnStopped();
                    m_childNode.OnStarted();
                }
            }
            return childStatus;
        }

        #endregion
    }
}