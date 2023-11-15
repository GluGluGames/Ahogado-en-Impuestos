namespace BehaviourAPI.UnityToolkit
{
    using Core;
    using Core.Perceptions;

    /// <summary>
    /// Perception type specific for Unity environment.
    /// </summary>
    public abstract class UnityPerception : Perception
    {
        /// <summary>
        /// The execution context of the perception. Use it to get component references to the 
        /// object that executes the perception.
        /// </summary>
        protected UnityExecutionContext context;

        public sealed override void SetExecutionContext(ExecutionContext context)
        {
            this.context = (UnityExecutionContext)context;
            OnSetContext();
        }

        /// <summary>
        /// Override this method to store component references from <see cref="context"/>.
        /// </summary>
        protected virtual void OnSetContext()
        {
            return;
        }

        public override string ToString()
        {
            return "Unity Perception";
        }
    }
}
