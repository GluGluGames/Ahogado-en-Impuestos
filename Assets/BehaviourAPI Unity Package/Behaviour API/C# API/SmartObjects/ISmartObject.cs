    namespace BehaviourAPI.SmartObjects
{

    /// <summary> 
    /// An object that can provide behaviour to a smart agent and cover some of its needs.
    /// </summary>
    public interface ISmartObject<T> where T : ISmartAgent
    {
        /// <summary>   
        /// Request the interaction. 
        /// </summary>
        /// <param name="agent">The agent who request the interaction. </param>
        /// <param name="requestData">The data used to generate the interaction.</param>
        /// <returns> The interaction provided. </returns>
        SmartInteraction RequestInteraction(T agent, RequestData requestData);

        /// <summary>
        /// Validates the agent described by agent. 
        /// </summary>
        /// <param name="agent"> The agent who request the interaction. </param>
        /// <returns> True if it succeeds, false if it fails. </returns>
        bool ValidateAgent(T agent);

        /// <summary> 
        /// Gets the capability that this object has to cover a determined need with the same name. 
        /// </summary>
        /// <param name="capabilityName"> Name of the capability. </param>
        /// <returns> The capability. </returns>
        float GetCapabilityValue(string capabilityName);
    }
}
