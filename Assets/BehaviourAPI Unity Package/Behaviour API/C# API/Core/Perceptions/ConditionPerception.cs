using System;

namespace BehaviourAPI.Core.Perceptions
{
    /// <summary>
    /// Represents a perception created with custom methods.
    /// </summary>
    public class ConditionPerception : Perception
    {
        public Func<bool> onCheck;
        public Action onInit;
        public Action onReset;
        public Action onPause;
        public Action onUnpause;

        /// <summary>
        /// Create a <see cref="ConditionPerception"/> that execute a delegate on Init, Check and reset.
        /// </summary>
        /// <param name="onInit">The delegate executed in <see cref="Initialize"/> event. </param>
        /// <param name="onCheck">The function executed in <see cref="Check"/> event. </param>
        /// <param name="onReset">The delegate executed in <see cref="Reset"/> event. </param>
        public ConditionPerception(Action onInit, Func<bool> onCheck, Action onReset = null)
        {
            this.onInit = onInit;
            this.onCheck = onCheck;
            this.onReset = onReset;
        }

        /// <summary>
        /// Create a <see cref="ConditionPerception"/> that execute a delegate on Init, Check, reset, pause and unpause.
        /// </summary>
        /// <param name="onInit">The delegate executed in <see cref="Initialize"/> event. </param>
        /// <param name="onCheck">The function executed in <see cref="Check"/> event. </param>
        /// <param name="onReset">The delegate executed in <see cref="Reset"/> event. </param>
        /// <param name="onPause">The function executed in <see cref="Check"/> event. </param>
        /// <param name="onUnpause">The delegate executed in <see cref="Reset"/> event. </param>
        public ConditionPerception(Action onInit, Func<bool> onCheck, Action onReset, Action onPause, Action onUnpause)
        {
            this.onInit = onInit;
            this.onCheck = onCheck;
            this.onReset = onReset;
            this.onPause = onPause;
            this.onUnpause = onUnpause;
        }

        /// <summary>
        /// Create a <see cref="ConditionPerception"/> that execute a delegate on Check and, optionally on reset.
        /// </summary>
        /// <param name="onCheck">The function executed in <see cref="Check"/> event. </param>
        /// <param name="onReset">The delegate executed in <see cref="Reset"/> event. </param>
        public ConditionPerception(Func<bool> check, Action stop = null)
        {
            onCheck = check;
            onReset = stop;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConditionPerception()
        {
        }

        /// <summary>
        /// <inheritdoc/>
        /// Invoke the init delegate.
        /// </summary>
        public override void Initialize() => onInit?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Invoke the check function and returns its returned value.
        /// </summary>
        public override bool Check() => onCheck?.Invoke() ?? false;

        /// <summary>
        /// <inheritdoc/>
        /// Invoke the reset delegate.
        /// </summary>
        public override void Reset() => onReset?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Invoke the pause delegate.
        /// </summary>
        public override void Pause() => onPause?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Invoke the unpause delegate.
        /// </summary>
        public override void Unpause() => onUnpause?.Invoke();
    }
}
