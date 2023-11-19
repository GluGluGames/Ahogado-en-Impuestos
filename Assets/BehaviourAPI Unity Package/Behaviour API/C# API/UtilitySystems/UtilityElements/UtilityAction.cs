namespace BehaviourAPI.UtilitySystems
{
    using Core;
    using Action = Core.Actions.Action;

    /// <summary>
    /// Utility node that executes an <see cref="Action"/> while selected.
    /// </summary>
    public class UtilityAction : UtilityExecutableNode
    {
        #region ------------------------------------------- Fields -------------------------------------------

        /// <summary>
        /// If true, when the <see cref="Action"/> end its execution with <see cref="Status.Success"/> or <see cref="Status.Failure"/>,
        /// end the <see cref="UtilitySystem"/> execution with this <see cref="Status"/> value.
        /// </summary>
        public bool FinishSystemOnComplete = false;

        /// <summary>
        /// The <see cref="Action"/> that this <see cref="UtilityAction"/> executes when is selected.
        /// </summary>
        public Action Action;

        #endregion

        #region -------------------------------------- Private variables -------------------------------------

        bool _isActionRunning;

        #endregion

        #region ---------------------------------------- Build methods ---------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Clone the <see cref="Action"/> too.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override object Clone()
        {
            UtilityAction action = (UtilityAction)base.Clone();
            action.Action = (Action)Action?.Clone();
            return action;
        }

        #endregion

        #region --------------------------------------- Runtime methods --------------------------------------

        /// <summary>
        /// <inheritdoc/>
        /// Start the <see cref="Action"/> execution.
        /// </summary>
        public override void OnStarted()
        {
            base.OnStarted();
            Action?.Start();
            _isActionRunning = true;
        }

        /// <summary>
        /// <inheritdoc/>.
        /// Starts the Action.
        /// </summary>
        public override void OnUpdated()
        {
            if (Status != Status.Running) return;

            var actionResult = Action?.Update() ?? Status.Running;
            if (actionResult != Status.Running)
            {
                _isActionRunning = false;
                Action?.Stop();
            }
            Status = actionResult;

            if (FinishSystemOnComplete && Status != Status.Running)
            {
                BehaviourGraph.Finish(Status);
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// Stops the action.
        /// </summary>
        public override void OnStopped()
        {
            base.OnStopped();
            if (_isActionRunning)
            {
                _isActionRunning = false;
                Action.Stop();
            }
        }

        /// <summary>
        /// <inheritdoc/>
        /// Pauses the action.
        /// </summary>
        public override void OnPaused()
        {
            base.OnPaused();

            if (!_isActionRunning) return;

            Action?.Pause();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Unpauses the action.
        /// </summary>
        public override void OnUnpaused()
        {
            base.OnUnpaused();

            if (!_isActionRunning) return;

            Action?.Unpause();
        }

        #endregion

        /// <summary>
        /// <inheritdoc/>
        /// Passes the context to <see cref="Action"/>.
        /// </summary>
        /// <param name="context"><inheritdoc/></param>
        public override void SetExecutionContext(ExecutionContext context)
        {
            Action?.SetExecutionContext(context);
        }
    }
}
