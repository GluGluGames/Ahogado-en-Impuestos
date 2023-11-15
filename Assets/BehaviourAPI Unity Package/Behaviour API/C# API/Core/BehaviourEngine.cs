using System;

namespace BehaviourAPI.Core
{
    /// <summary>
    /// Basic class for all behaviour systems. 
    /// </summary>
    public abstract class BehaviourEngine : IStatusHandler
    {
        #region ----------------------------------------- Properties -------------------------------------------

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

        public Action<Status> StatusChanged { get; set; } = delegate { };

        /// <summary>
        /// Gets if the graph is paused.
        /// </summary>
        /// <value>True if the graph is paused, false otherwise.</value>
        public bool IsPaused => _status == Status.Paused;

        /// <summary>
        /// Gets if the graph execution was started and is not over yet.
        /// </summary>
        public bool IsExecuting => _status == Status.Running || _status == Status.Paused;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        Status _status;

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// Starts the execution and set the status value to Running.
        /// </summary>
        /// <exception cref="ExecutionStatusException">If the graph is already in execution.</exception>
        public void Start()
        {
            if (Status != Status.None)
                throw new ExecutionStatusException(this, "ERROR: This behaviour engine is already been executed");

            Status = Status.Running;
            OnStarted();
        }

        /// <summary>
        /// Stops the execution and sets the status value to None.
        /// </summary>
        /// <exception cref="ExecutionStatusException">If the graph is not in execution.</exception>
        public void Stop()
        {
            if (Status == Status.None)
                throw new ExecutionStatusException(this, "ERROR: This behaviour engine is already been stopped");

            Status = Status.None;
            OnStopped();
        }

        /// <summary>
        /// Update the execution if not finished yet.
        /// </summary>
        public void Update()
        {
            if (Status != Status.Running) return; // Graph already finished or paused
            OnUpdated();
        }

        /// <summary>
        /// Pauses the execution of the graph if is not paused yet.
        /// </summary>
        public void Pause()
        {
            if (Status == Status.Running)
            {
                Status = Status.Paused;
                OnPaused();
            }
        }

        /// <summary>
        /// Unpauses the execution of the graph is was paused before.
        /// </summary>
        public void Unpause()
        {
            if (Status == Status.Paused)
            {
                Status = Status.Running;
                OnUnpaused();
            }
        }

        /// <summary>
        /// Finish the graph execution with the status given. 
        /// </summary>
        /// <param name="executionResult">The final value for <see cref="Status"></see>. Must be <see cref="Status.Success"/> or <see cref="Status.Failure"/>. </param>
        /// <exception cref="ExecutionStatusException">If the value passed as argument is not success or failure.</exception>
        public void Finish(Status executionResult)
        {
            if (executionResult == Status.Running || executionResult == Status.None)
                throw new ExecutionStatusException(this, $"Error: BehaviourEngine execution result can't be {executionResult}");

            Status = executionResult;
        }

        /// <summary>
        /// Set the execution context of the system.
        /// </summary>
        /// <param name="context">The <see cref="ExecutionContext"/> used.</param>
        public abstract void SetExecutionContext(ExecutionContext context);

        /// <summary>
        /// Stop the execution and start it right after.
        /// </summary>
        public void Restart()
        {
            Stop();
            Start();
        }

        /// <summary>
        /// Called when the graph starts the execution. 
        /// </summary>
        protected abstract void OnStarted();

        /// <summary>
        /// Called every frame while the graph is executing, until its finished or stopped. 
        /// </summary>
        protected abstract void OnUpdated();

        /// <summary>
        /// Called when the graph is stopped. 
        /// </summary>
        protected abstract void OnStopped();

        /// <summary>
        /// Called when the graph is paused. 
        /// </summary>
        protected abstract void OnPaused();

        /// <summary>
        /// Called when the graph is unpaused. 
        /// </summary>
        protected abstract void OnUnpaused();

        #endregion
    }
}
