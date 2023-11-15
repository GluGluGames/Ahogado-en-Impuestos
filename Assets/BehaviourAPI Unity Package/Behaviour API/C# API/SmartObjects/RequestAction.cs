namespace BehaviourAPI.SmartObjects
{
    using Core;
    using Core.Actions;

    /// <summary>   
    /// Action that request a behaviour to a smart object. 
    /// </summary>
    public abstract class RequestAction<T> : Action where T : ISmartAgent
    {
        /// <summary> 
        /// The current interaction that this action is executing. 
        /// </summary>
        SmartInteraction m_CurrentInteraction;

        /// <summary> 
        /// The agent used in the interactions. 
        /// </summary>
        public T Agent { get; set; }

        /// <summary>
        /// The context that the request action will propagate throught the provided actions.
        /// </summary>
        protected ExecutionContext context;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected RequestAction()
        {
        }

        /// <summary> 
        /// Specialized constructor for use only by derived class. 
        /// </summary>
        /// <param name="agent"> The agent used in the interactions. </param>
        protected RequestAction(T agent)
        {
            Agent = agent;
        }

        /// <summary>
        /// Get the provider that this request action will use to find the smart objects.
        /// </summary>
        /// <returns>The smart object provider.</returns>
        protected abstract ISmartObject<T> GetSmartObject();

        /// <summary> 
        /// Request interaction. 
        /// </summary>
        /// <param name="smartObject"> The smart object. </param>
        /// <param name="agent"> The agent used in the interactions. </param>
        /// <returns> A SmartInteraction. </returns>
        protected abstract RequestData GetRequestData();

        /// <summary>   
        /// <inheritdoc/>
        /// Searches for an interaction and initializes it. If it doesn't find any, it will return failure on the next <see cref="Update"/> execution.
        ///  </summary>
        public override void Start()
        {
            if (Agent == null)
                throw new MissingAgentException<T>(this, "Can't send request to a smart object without smart agent");

            ISmartObject<T> obj = GetSmartObject();

            if (obj != null && obj.ValidateAgent(Agent))
            {
                RequestData requestData = GetRequestData();
                m_CurrentInteraction = obj.RequestInteraction(Agent, requestData);
                m_CurrentInteraction?.Initialize(context);
            }
        }

        /// <summary>   
        /// <inheritdoc/>
        /// If has an active interaction, stops it and then discards it.
        ///  </summary>
        public override void Stop()
        {
            m_CurrentInteraction?.Release();
            m_CurrentInteraction = null;
        }

        /// <summary>   
        /// <inheritdoc/>
        /// If has an active interaction, stops it and then discards it.
        /// </summary>
        /// <returns><inheritdoc/></returns>
        public override Status Update()
        {
            if (m_CurrentInteraction != null)
            {
                var status = m_CurrentInteraction.Update();
                return status;
            }
            else
            {
                return Status.Failure;
            }
        }

        public override void Pause()
        {
            m_CurrentInteraction?.Pause();
        }

        public override void Unpause()
        {
            m_CurrentInteraction?.Unpause();
        }

        public override void SetExecutionContext(ExecutionContext context)
        {
            this.context = context;
        }
    }
}
