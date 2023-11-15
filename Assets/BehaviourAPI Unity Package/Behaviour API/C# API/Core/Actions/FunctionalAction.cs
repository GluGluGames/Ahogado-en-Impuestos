using System;

namespace BehaviourAPI.Core.Actions
{
    /// <summary>
    /// Represents an action created with custom methods.
    /// </summary>
    public class FunctionalAction : Action
    {
        public Func<Status> onUpdated;

        public System.Action onStarted;
        public System.Action onStopped;
        public System.Action onPaused;
        public System.Action onUnpaused;

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a delegate on Start, Update and stop.
        /// </summary>
        /// <param name="start">The delegate executed in <see cref="Start"/> event.</param>
        /// <param name="update">The function executed in <see cref="Update"/> event.</param>
        /// <param name="stop">The delegate executed in <see cref="Stop"/> event.</param>
        public FunctionalAction(System.Action start, Func<Status> update, System.Action stop = null)
        {
            onStarted = start;
            onUpdated = update;
            onStopped = stop;
        }

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a delegate on Start, Update and stop.
        /// </summary>
        /// <param name="onStarted">The delegate executed in <see cref="Start"/> event.</param>
        /// <param name="onUpdated">The function executed in <see cref="Update"/> event.</param>
        /// <param name="onStopped">The delegate executed in <see cref="Stop"/> event.</param>
        /// <param name="onPaused">The delegate executed in <see cref="Pause"/> event.</param>
        /// <param name="onUnpaused">The delegate executed in <see cref="Unpause"/> event.</param>
        public FunctionalAction(System.Action onStarted, Func<Status> onUpdated, System.Action onStopped, System.Action onPaused, System.Action onUnpaused)
        {
            this.onStarted = onStarted;
            this.onUpdated = onUpdated;
            this.onStopped = onStopped;
            this.onPaused = onPaused;
            this.onUnpaused = onUnpaused;
        }

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a delegate on Update and optionally, a method on stop.
        /// </summary>
        /// <param name="update">The function executed in <see cref="Update"/> event.</param>
        /// <param name="stop">The delegate executed in <see cref="Stop"/> event.</param>
        public FunctionalAction(Func<Status> update, System.Action stop = null)
        {
            onUpdated = update;
            onStopped = stop;
        }

        /// <summary>
        /// Create a <see cref="FunctionalAction"/> that executes a method when started and only returns <see cref="Status.Running"/> on Update.
        /// </summary>
        /// <param name="start">The delegate executed in <see cref="Start"/> event.</param>
        public FunctionalAction(System.Action start)
        {
            onStarted = start;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public FunctionalAction()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="onStarted"/>.
        /// </summary>
        public override void Start() => onStarted?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="onUpdated"/>.and returns its given value.
        /// </summary>
        public override Status Update() => onUpdated?.Invoke() ?? Status.Running;

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="onStopped"/>.
        /// </summary>
        public override void Stop() => onStopped?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="onPaused"/>.
        /// </summary>
        public override void Pause()
        {
            onPaused?.Invoke();
        }

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="onUnpaused"/>.
        /// </summary>
        public override void Unpause()
        {
            onUnpaused?.Invoke();
        }
    }
}
