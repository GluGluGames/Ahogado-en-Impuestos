namespace BehaviourAPI.Core.Actions
{
    /// <summary>
    /// Action that executes a custom method when is started and always returns success.
    /// </summary>
    public class SimpleAction : Action
    {
        /// <summary>
        /// Delegate called when the action is started.
        /// </summary>
        public System.Action action;

        /// <summary>
        /// Default constructor
        /// </summary>
        public SimpleAction()
        {
        }

        /// <summary>
        /// Create a simple action that executes the given method.
        /// </summary>
        /// <param name="action"></param>
        public SimpleAction(System.Action action)
        {
            this.action = action;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Invokes <see cref="action"/>.
        /// </summary>
        public override void Start() => action?.Invoke();

        /// <summary>
        /// <inheritdoc/>
        /// Returns always success.
        /// </summary>
        /// <returns></returns>
        public override Status Update() => Status.Success;
    }
}
