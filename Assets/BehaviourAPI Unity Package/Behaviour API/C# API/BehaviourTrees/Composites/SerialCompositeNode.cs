namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// Composite node that executes its children sequencially.
    /// </summary>
    public abstract class SerialCompositeNode : CompositeNode
    {
        #region ------------------------------------------ Properties -----------------------------------------

        int currentChildIdx = 0;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Starts the first node execution.
        /// </summary>
        public override void OnStarted()
        {
            currentChildIdx = 0;
            base.OnStarted();
            GetCurrentChild().OnStarted();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stops the current executed child.
        /// </summary>
        public override void OnStopped()
        {
            base.OnStopped();
            GetCurrentChild().OnStopped();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the current executed child.
        /// </summary>
        public override void OnPaused()
        {
            base.OnPaused();
            GetCurrentChild().OnPaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the current executed child.
        /// </summary>
        public override void OnUnpaused()
        {
            base.OnUnpaused();
            GetCurrentChild().OnUnpaused();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Update the execution of the current child. If it returned status is not the final
        /// status, stop the child and starts the next. If there are no more childs, return the final status.
        /// </summary>
        /// <returns>Running if no child has returned the target status and all childs were not executed yet. Else returns the status of the last node.</returns>
        protected override Status UpdateStatus()
        {
            BTNode currentChild = GetCurrentChild();
            currentChild.OnUpdated();
            var status = currentChild.Status;

            if (KeepExecutingNextChild(status) && currentChildIdx < m_children.Count - 1)
            {
                currentChild.OnStopped();
                currentChildIdx++;
                currentChild = GetCurrentChild();
                currentChild.OnStarted();
                return Status.Running;
            }
            else
            {
                return GetFinalStatus(status);
            }
        }

        /// <summary>
        /// Return if the execution should jump to the next child if exists.
        /// </summary>
        /// <param name="status">The current status of the child.</param>
        /// <returns>true if <paramref name="status"/> is not the target value, false otherwise. </returns>
        protected abstract bool KeepExecutingNextChild(Status status);

        /// <summary>
        /// Get the final status of the composite node (must be success or failure).
        /// </summary>
        /// <param name="status">The current status of the node.</param>
        /// <returns>The final execution status of the node.</returns>
        protected abstract Status GetFinalStatus(Status status);

        private BTNode GetCurrentChild() => GetBTChildAt(currentChildIdx);

        #endregion
    }
}