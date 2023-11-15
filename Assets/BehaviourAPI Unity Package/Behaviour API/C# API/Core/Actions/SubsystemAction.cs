namespace BehaviourAPI.Core.Actions
{
    /// <summary>
    /// Represents an action that executes a sub behaviour engine.
    /// </summary>
    public class SubsystemAction : Action
    {
        /// <summary>
        /// The subsystem executed by the action.
        /// </summary>
        public BehaviourEngine SubSystem;

        /// <summary>
        /// True if the subsystem will restart after finish.
        /// </summary>
        public bool ExecuteOnLoop;

        /// <summary>
        /// True if the subsystem won't restart if is interrupted.
        /// </summary>
        public ExecutionInterruptOptions InterruptOptions;

        bool _isInterrupted;

        /// <summary>
        /// Create a new <see cref="SubsystemAction"/> with the specified subsystem and configuration flags.
        /// </summary>
        /// <param name="subSystem">The subsystem executed by the action.</param>
        /// <param name="executeOnLoop">True if the subsystem will restart after finish.</param>
        /// <param name="dontStopOnInterrupt">True if the subsystem won't restart if is interrupted</param>
        public SubsystemAction(BehaviourEngine subSystem, bool executeOnLoop = false, ExecutionInterruptOptions interruptOptions = ExecutionInterruptOptions.Stop)
        {
            SubSystem = subSystem;
            ExecuteOnLoop = executeOnLoop;
            InterruptOptions = interruptOptions;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Starts the <see cref="SubSystem"/> execution. 
        /// If <see cref="DontStopOnInterrupt"/> is true and the execution already started, unpauses the subgraph instead of starts it.
        /// to success or failure, the subsystem is restarted.
        /// </summary>
        /// <exception cref="MissingSubsystemException">If subsystem is null.</exception>
        public override void Start()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            if(_isInterrupted)
            {
                if (InterruptOptions == ExecutionInterruptOptions.Pause)
                {
                    SubSystem.Unpause();
                }
                else if (InterruptOptions == ExecutionInterruptOptions.Stop)
                {
                    SubSystem.Start();
                }
            }
            else
            {
                SubSystem.Start();
            }            
        }

        /// <summary>
        /// <inheritdoc/>
        /// Update the <see cref="SubSystem"/> execution.
        /// If <see cref="ExecuteOnLoop"/> is true and subsystem execution change its status 
        /// to success or failure, the subsystem is restarted.
        /// </summary>
        /// <returns>The status of the subsystem.</returns>
        /// <exception cref="MissingSubsystemException">If subsystem is null.</exception>
        public override Status Update()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            SubSystem.Update();

            if (ExecuteOnLoop && (SubSystem.Status == Status.Success || SubSystem.Status == Status.Failure))
                SubSystem.Restart();

            return SubSystem.Status;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stop the <see cref="SubSystem"/> execution.
        /// If <see cref="DontStopOnInterrupt"/> is true and the execution not finished, pauses the subgraph instead of stops it.
        /// </summary>
        /// <exception cref="MissingSubsystemException">If subsystem is null.</exception>
        public override void Stop()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            _isInterrupted = SubSystem.Status == Status.Running || SubSystem.Status == Status.Paused;

            if (_isInterrupted)
            {
                if (InterruptOptions == ExecutionInterruptOptions.Pause && SubSystem.Status == Status.Running)
                {
                    SubSystem.Pause();
                }
                else if (InterruptOptions == ExecutionInterruptOptions.Stop)
                {
                    SubSystem.Stop();
                }
            }
            else
            {
                SubSystem.Stop();
            }
        }

        /// <summary>
        /// Passes the execution context to the <see cref="SubSystem"/> .
        /// </summary>
        /// <param name="context"><inheritdoc/></param>
        public override void SetExecutionContext(ExecutionContext context)
        {
            SubSystem?.SetExecutionContext(context);
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the <see cref="SubSystem"/> execution.
        /// </summary>
        /// <exception cref="MissingSubsystemException">If subsystem is null.</exception>
        public override void Pause()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            SubSystem?.Pause();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the <see cref="SubSystem"/> execution.
        /// </summary>
        /// <exception cref="MissingSubsystemException">If subsystem is null.</exception>
        public override void Unpause()
        {
            if (SubSystem == null)
                throw new MissingSubsystemException(this, "Subsystem cannot be null");

            SubSystem?.Unpause();
        }
    }
}
