namespace BehaviourAPI.SmartObjects
{
    /// <summary>
    /// Request action that send the request to a specified smart object.
    /// </summary>
    /// <typeparam name="T">The type of the agent used in the request.</typeparam>
    public class DirectRequestAction<T> : RequestAction<T> where T : ISmartAgent
    {
        /// <summary>
        /// The data included in the request
        /// </summary>
        public RequestData RequestData { get; set; }

        public ISmartObject<T> SmartObject { get; set; }

        /// <summary>
        /// Create a new direct request action.
        /// </summary>
        /// <param name="agent">The agent used to send the request</param>
        /// <param name="smartObject">The smart object requested.</param>
        /// <param name="requestData">The data included in the request.</param>
        public DirectRequestAction(T agent, ISmartObject<T> smartObject, RequestData requestData = null) : base(agent)
        {
            SmartObject = smartObject;
            RequestData = requestData;
        }

        protected override RequestData GetRequestData() => RequestData;

        protected override ISmartObject<T> GetSmartObject() => SmartObject;
    }
}
