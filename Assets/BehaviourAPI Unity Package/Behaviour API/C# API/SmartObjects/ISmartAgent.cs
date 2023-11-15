namespace BehaviourAPI.SmartObjects
{
    /// <summary>
    /// A behaviour agent that has some needs and can use smart objects. 
    /// </summary>
    public interface ISmartAgent
    {
        /// <summary> 
        /// Gets a need value of the agent. 
        /// </summary>
        /// <param name="name"> The name of the need. </param>
        /// <returns> The value of the need. </returns>
        float GetNeed(string name);

        /// <summary> 
        /// Sets a need value of the agent. 
        /// </summary>
        /// <param name="name"> The name of the need. </param>
        /// <returns> The value of the need. </returns>
        void CoverNeed(string name, float value);
    }
}
