using BehaviourAPI.SmartObjects;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Request action that send the request to a single Smart object.
    /// </summary>
    public class TargetRequestAction : UnityRequestAction
    {
        /// <summary>
        /// The smart object that will receive the request
        /// </summary>
        public SmartObject smartObject;

        /// <summary>
        /// The data sent with the request
        /// </summary>
        public RequestData requestData;

        public TargetRequestAction(SmartAgent agent, SmartObject smartObject, RequestData requestData) : base(agent)
        {
            this.smartObject = smartObject;
            this.requestData = requestData;
        }

        public TargetRequestAction()
        {
        }

        protected override SmartObject GetRequestedSmartObject() => smartObject;

        protected override RequestData GetRequestData() => requestData;

        public override string ToString() => $"Request interaction to {smartObject?.name ?? "null"}";
    }
}
