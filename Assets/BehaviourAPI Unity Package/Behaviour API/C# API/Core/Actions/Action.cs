using System;

namespace BehaviourAPI.Core.Actions
{
    /// <summary>
    /// Represent a task that a behaviour agent can perform.
    /// </summary>
    public abstract class Action : ICloneable
    {
        /// <summary>
        /// Starts the action. 
        /// </summary>
        public virtual void Start() { }

        /// <summary>
        /// Updates the action. 
        /// </summary>
        public abstract Status Update();

        /// <summary>
        /// Stops the action. 
        /// </summary>
        public virtual void Stop() { }

        /// <summary>
        /// Pauses the action. 
        /// </summary>
        public virtual void Pause() { }

        /// <summary>
        /// Unpauses the action. 
        /// </summary>
        public virtual void Unpause() { }

        /// <summary>
        /// Specifies the action execution context
        /// </summary>
        /// <param name="context">The execution context.</param>
        public virtual void SetExecutionContext(ExecutionContext context) { }

        /// <summary>
        /// Create a shallow copy of the action.
        /// </summary>
        /// <returns>The action copy.</returns>
        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}