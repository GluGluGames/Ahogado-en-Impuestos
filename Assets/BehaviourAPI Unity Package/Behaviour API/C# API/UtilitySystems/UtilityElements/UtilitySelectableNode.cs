using System;

namespace BehaviourAPI.UtilitySystems
{
    using Core;

    /// <summary>
    /// Utility node that can be selected and executed by a utility system.
    /// </summary>
    public abstract class UtilitySelectableNode : UtilityNode, IStatusHandler
    {
        #region ------------------------------------------ Properties -----------------------------------------

        public override int MaxInputConnections => 1;

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
        /// Event called when current status changed.
        /// </summary>
        public Action<Status> StatusChanged { get; set; }


        /// <summary>
        /// True if this element should be executed even if later elements have more utility:
        /// </summary>
        public bool ExecutionPriority { get; protected set; }

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        Status _status;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        public override object Clone()
        {
            UtilitySelectableNode node = (UtilitySelectableNode)base.Clone();

            if(StatusChanged != null)
                node.StatusChanged = (Action<Status>)StatusChanged.Clone();

            return node;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Is called when the <see cref="UtilitySelectableNode"/> is selected. 
        /// </summary>
        /// <exception cref="ExecutionStatusException">If it's already running.</exception>
        public virtual void OnStarted()
        {
            if (Status != Status.None)
                throw new ExecutionStatusException(this, "ERROR: This node is already been executed");

            Status = Status.Running;
        }

        /// <summary>
        /// Is called each frame the <see cref="UtilitySelectableNode"/> is selected.
        /// </summary>
        public abstract void OnUpdated();

        /// <summary>
        /// Called when the <see cref="UtilitySelectableNode"/> is no longer selected or the <see cref="UtilitySystem"/> was stopped.
        /// </summary>
        /// <exception cref="ExecutionStatusException">If it's not running.</exception>
        public virtual void OnStopped()
        {
            if (Status == Status.None)
                throw new ExecutionStatusException(this, "ERROR: This node is already been stopped");

            Status = Status.None;
        }

        /// <summary>
        /// Called when the node is being selected and the graph is paused.
        /// </summary>
        public virtual void OnPaused()
        {
            if (Status != Status.Running)
                throw new ExecutionStatusException(this, "ERROR: This node can't be paused. It's status is not running");

            Status = Status.Paused;
        }

        /// <summary>
        /// Called when the node is being selected and the graph is unpaused.
        /// </summary>
        public virtual void OnUnpaused()
        {
            if (Status != Status.Paused)
                throw new ExecutionStatusException(this, "ERROR: This node can't be unpaused. It's status is not paused");

            Status = Status.Running;
        }

        #endregion
    }
}