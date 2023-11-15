namespace BehaviourAPI.SmartObjects
{
    /// <summary>
    /// Class that stores the information sent to smart object to request an interaction.
    /// </summary>
    [System.Serializable]
    public class RequestData
    {
        /// <summary>
        /// The name of the need requested.
        /// </summary>
        public string Need;

        /// <summary>
        /// Create a new request data without specify the interaction name.
        /// </summary>
        public RequestData()
        {
        }

        /// <summary>
        /// Create a new request interaction.
        /// </summary>
        /// <param name="need">The name of the need requested.</param>
        public RequestData(string need)
        {
            Need = need;
        }

        public static implicit operator RequestData(string interactionName) => new RequestData(interactionName);
    }
}