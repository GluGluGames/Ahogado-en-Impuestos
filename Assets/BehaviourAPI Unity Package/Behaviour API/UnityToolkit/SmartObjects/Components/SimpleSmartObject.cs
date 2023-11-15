using BehaviourAPI.SmartObjects;

namespace BehaviourAPI.UnityToolkit
{
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Smart object that provide always the same interaction
    /// </summary>
    public class SimpleSmartObject : SmartObject
    {
        [SerializeField] SmartInteractionProvider interactionProvider;

        public Dictionary<string, float> GetCapabilities()
        {
            return interactionProvider.GetCapabilityMap();
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            return interactionProvider.GetCapabilityMap().GetValueOrDefault(capabilityName);
        }

        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            SmartInteraction interaction = interactionProvider.CreateInteraction(agent);
            SetInteractionEvents(interaction);
            return interaction;
        }

        public virtual void SetInteractionEvents(SmartInteraction interaction) 
        {
            return;
        }

        /// <summary>
        /// <inheritdoc/>
        /// Override this method on subclases to specify when the agent is not valid.
        /// </summary>
        /// <param name="agent"><inheritdoc/></param>
        /// <returns><inheritdoc/></returns>
        public override bool ValidateAgent(SmartAgent agent)
        {
            return true;
        }
    }
}
