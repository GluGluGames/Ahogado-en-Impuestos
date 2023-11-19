namespace BehaviourAPI.BehaviourTrees
{
    using Core;  

    /// <summary>
    /// Composite node that executes all its children in all execution frames. It can be configured to stop the execution when
    /// any anction ends, when ends with success or failure, or when all action ends.
    /// By default, the action stops when all action ends and return the result of the last actiom.
    /// </summary>
    public class ParallelCompositeNode : CompositeNode
    {
        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// If true, the parallel action will stop when any action ends with success.
        /// </summary>
        public bool finishOnSuccess;

        /// <summary>
        /// If true, the parallel action will stop when any action ends with success.
        /// </summary>
        public bool finishOnFailure;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// Set the target status flags of the parallel node.
        /// </summary>
        /// <param name="finishOnSuccess">If true, the parallel action will stop when any action ends with success.</param>
        /// <param name="finishOnFailure"></param>
        /// <returns></returns>
        public ParallelCompositeNode SetTargetStatusFlags(bool finishOnSuccess, bool finishOnFailure)
        {
            this.finishOnSuccess = finishOnSuccess;
            this.finishOnFailure = finishOnFailure;
            return this;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Starts all its children.
        /// </summary>
        public override void OnStarted()
        {
            base.OnStarted();
            m_children.ForEach(c => c?.OnStarted());
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stop all its children.
        /// </summary>
        public override void OnStopped()
        {
            base.OnStopped();
            m_children.ForEach(c => c?.OnStopped());
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses all its children.
        /// </summary>
        public override void OnPaused()
        {
            base.OnPaused();
            m_children.ForEach(c =>
            {
                if (c.Status == Status.Running)
                    c.OnPaused();
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses all its children.
        /// </summary>
        public override void OnUnpaused()
        {
            base.OnUnpaused();
            m_children.ForEach(c =>
            {
                if (c.Status == Status.Paused)
                    c.OnUnpaused();
            });
        }

        /// <summary>
        /// <inheritdoc/>
        /// Update all its children node. The returned <see cref="Status"/> value depends on the children status and the value in <see cref="TriggerStatus"/>.
        /// </summary>
        /// <returns><see cref="TriggerStatus"/> if any of the nodes end with <see cref="TriggerStatus"/>, else if all of the nodes end, return the oposite status, else return running.</returns>
        /// <exception cref="MissingChildException">If the child list is empty.</exception>
        protected override Status UpdateStatus()
        {
            if (m_children.Count == 0) throw new MissingChildException(this, "This composite has no childs");

            int currentChildId = 0;

            Status returnedStatus = Status.Running;
            Status currentChildStatus = Status.Running;
            bool anyChildRunning = false;

            while(currentChildId < m_children.Count && returnedStatus == Status.Running)
            {
                var child = m_children[currentChildId];
                if(child.Status == Status.Running)
                {
                    child.OnUpdated();
                    currentChildStatus = child.Status;
                    if (currentChildStatus == Status.Running) anyChildRunning |= true;

                    if (finishOnSuccess && currentChildStatus == Status.Success || finishOnFailure && currentChildStatus == Status.Failure)
                    {
                        returnedStatus = currentChildStatus;
                    }
                }
                currentChildId++;
            }

            if (!anyChildRunning && returnedStatus == Status.Running)
            {
                return currentChildStatus;
            }
            else
            {
                return returnedStatus;
            }
        }


        #endregion
    }
}
