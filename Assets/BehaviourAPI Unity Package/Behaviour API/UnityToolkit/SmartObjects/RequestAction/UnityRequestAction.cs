using BehaviourAPI.Core;
using BehaviourAPI.SmartObjects;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Request action specified for Unity
    /// </summary>
    public abstract class UnityRequestAction : RequestAction<SmartAgent>
    {
        /// <summary>
        /// The execution context
        /// </summary>
        protected new UnityExecutionContext context;

        public override string ToString() => "Request action";

        protected UnityRequestAction()
        {
        }

        protected UnityRequestAction(SmartAgent agent) : base(agent)
        {
        }

        public sealed override void SetExecutionContext(ExecutionContext ctx)
        {
            base.SetExecutionContext(ctx);
            context = (UnityExecutionContext)ctx;

            if (Agent == null)
                Agent = context.SmartAgent;

            OnSetContext(context);
        }

        protected sealed override ISmartObject<SmartAgent> GetSmartObject() => GetRequestedSmartObject();

        /// <summary>
        /// Select the smart object to send the request
        /// </summary>
        /// <returns>The smart object selected</returns>
        protected abstract SmartObject GetRequestedSmartObject();

        /// <summary>
        /// Override this method to use the Unity execution context in this request action
        /// </summary>
        /// <param name="context">The execution context</param>
        protected virtual void OnSetContext(UnityExecutionContext context)
        {
        }
    }
}
