using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Action type specified for Unity Environment
    /// </summary>
    public abstract class UnityAction : Action
    {
        /// The execution context of the action. Use it to get component references to the 
        /// object that executes the action.
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
    }
}
