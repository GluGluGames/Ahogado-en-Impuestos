using System;

namespace BehaviourAPI.BehaviourTrees
{
    using Core;

    /// <summary>
    /// The base node in the <see cref="BehaviourTree"/>.
    /// </summary>
    public abstract class BTNode : Node, IStatusHandler, IPushActivable
    {
        #region ------------------------------------------ Properties -----------------------------------------
        public override int MaxInputConnections => 1;
        public override Type ChildType => typeof(BTNode);
        public override Type GraphType => typeof(BehaviourTree);

        /// <summary>
        /// The execution status of the node.
        /// </summary>
        public Status Status
        {
            get => _status;
            protected set
            {
                if (_status != value)
                {
                    _status = value;
                    StatusChanged?.Invoke(_status);
                }
            }
        }

        /// <summary>
        /// The final status of this node in the current iteration loop. 
        /// This value can only reset by loops.
        /// </summary>
        public Status LastExecutionStatus
        {
            get => _lastExecutionStatus;
            protected set
            {
                _lastExecutionStatus = value;
                LastExecutionStatusChanged?.Invoke(value);
            }
        }

        /// <summary>
        /// Event called when current status changed.
        /// </summary>
        public Action<Status> StatusChanged { get; set; }

        /// <summary>
        /// Event called when last execution status changed.
        /// </summary>
        public Action<Status> LastExecutionStatusChanged { get; set; }

        #endregion

        Status _status;
        Status _lastExecutionStatus;

        #region ----------------------------------------- Build methods --------------------------------------

        public override object Clone()
        {
            var btNode = (BTNode)base.Clone();
            btNode.StatusChanged = (Action<Status>)StatusChanged?.Clone();
            btNode.LastExecutionStatusChanged = (Action<Status>)LastExecutionStatusChanged?.Clone();
            return btNode;
        }
        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Starts the node execution, changing <see cref="Status"/> to Running. 
        /// Reset the <see cref="LastExecutionStatus"/> value if its necessary.
        /// </summary>
        /// <exception cref="ExecutionStatusException">If the node execution already started. </exception>
        public virtual void OnStarted()
        {
            if (Status != Status.None)
                throw new ExecutionStatusException(this, "ERROR: This node is already been executed");

            Status = Status.Running;
            ResetLastStatus();
        }

        /// <summary>
        /// Update the execution of the node.
        /// </summary>
        /// <exception cref="ExecutionStatusException">If the node is not executing.</exception>
        public void OnUpdated()
        {
            if (Status == Status.None)
                throw new ExecutionStatusException(this, "ERROR: This node must be started before update.");

            if (Status != Status.Running) return;

            Status = UpdateStatus();
        }

        /// <summary>
        /// Stop the node execution, changing <see cref="Status"/> to None. 
        /// Save the final execution status in <see cref="LastExecutionStatus"/>
        /// </summary>
        /// <exception cref="Exception">If was already stopped.</exception>
        public virtual void OnStopped()
        {
            if (Status == Status.None)
                throw new ExecutionStatusException(this, "ERROR: This node is already been stopped");

            LastExecutionStatus = Status;
            Status = Status.None;
        }

        /// <summary>
        /// Called when the node is in a running branch and the graph is paused.
        /// </summary>
        public virtual void OnPaused()
        {
            if (Status != Status.Running)
                throw new ExecutionStatusException(this, "ERROR: This node can't be paused. It's status is not running");

            Status = Status.Paused;
        }

        /// <summary>
        /// Called when the node is in a running branch and the graph is unpaused.
        /// </summary>
        public virtual void OnUnpaused()
        {
            if (Status != Status.Paused)
                throw new ExecutionStatusException(this, "ERROR: This node can't be unpaused. It's status is not paused");

            Status = Status.Running;
        }

        /// <summary>
        /// Get the updated status of the node.
        /// </summary>
        /// <returns>The new status of the node.</returns>
        protected abstract Status UpdateStatus();

        /// <summary>
        /// Set the current last execution status to none and return
        /// true if the previous value was different from none.
        /// </summary>
        /// <returns>True if the <see cref="LastExecutionStatus"/> value changed.</returns>
        public virtual bool ResetLastStatus()
        {
            if (LastExecutionStatus != Status.None)
            {
                LastExecutionStatus = Status.None;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// End the execution of the node externally.
        /// </summary>
        public void Fire(Status status)
        {
            if (Status == Status.Running)
            {
                if (status != Status.None)
                    Status = status;
            }
        }

        #endregion
    }
}