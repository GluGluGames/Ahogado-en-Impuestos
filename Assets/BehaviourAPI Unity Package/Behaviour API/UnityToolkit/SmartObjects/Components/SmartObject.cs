using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using BehaviourAPI.SmartObjects;

    /// <summary> 
    /// Unity component to implement a Smart Object.
    /// </summary>
    public abstract class SmartObject : MonoBehaviour, ISmartObject<SmartAgent>
    {
        [SerializeField] List<string> tags = new List<string>();

        /// <summary> 
        /// Flag to register the smart object in the <see cref="SmartObjectManager"/>. 
        /// </summary>
        public bool _registerOnManager = true;

        void OnEnable()
        {
            if (_registerOnManager)
                SmartObjectManager.Instance.RegisterSmartObject(this);
        }

        void OnDisable()
        {
            if (_registerOnManager)
                SmartObjectManager.Instance.UnregisterSmartObject(this);
        }

        /// <summary>
        /// Validate a agent used to make a request.
        /// </summary>
        /// <param name="agent">The smart agent.</param>
        /// <returns>True if the agent is valid, false otherwise.</returns>
        public abstract bool ValidateAgent(SmartAgent agent);

        /// <summary>
        /// Get the interaction requested.
        /// </summary>
        /// <param name="agent">The smart agent.</param>
        /// <param name="requestData">The data used to specify the interaction requested.</param>
        /// <returns>The interaction generated</returns>
        public abstract SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData);

        public abstract float GetCapabilityValue(string capabilityName);

        public bool HasTag(string tag)
        {
            return tags.Contains(tag);
        }
    }
}
