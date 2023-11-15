using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using SmartObjects;

    /// <summary>
    /// Smart object that contains a list of interactions, each one binded to a different need.
    /// </summary>
    public class CompoundSmartObject : SmartObject
    {
        [SerializeField] InteractionMap interactionMap;
        [SerializeField] string defaultInteractionName;

        Dictionary<string, float> m_CapabilityMap;

        private void Awake()
        {
            foreach(var interaction in interactionMap.Values)
            {
                var interactionCapabilities = interaction.GetCapabilityMap();
                foreach(var capability in interactionCapabilities) 
                { 
                    if(!m_CapabilityMap.ContainsKey(capability.Key)) 
                    {
                        m_CapabilityMap[capability.Key] = capability.Value;
                    }
                    else
                    {
                        m_CapabilityMap[capability.Key] = Mathf.Max(m_CapabilityMap[capability.Key], capability.Value);
                    }
                }
            }
        }

        public override float GetCapabilityValue(string capabilityName) => m_CapabilityMap.GetValueOrDefault(capabilityName);

        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            if(!string.IsNullOrEmpty(requestData.Need) && interactionMap.TryGetValue(requestData.Need, out SmartInteractionProvider provider))
            {
                SmartInteraction interaction = provider.CreateInteraction(agent);
                SetInteractionEvents(interaction, requestData.Need);
                return interaction;
            }
            else if(interactionMap.TryGetValue(defaultInteractionName, out SmartInteractionProvider defaultProvider))
            {
                SmartInteraction interaction = defaultProvider.CreateInteraction(agent);
                SetInteractionEvents(interaction, requestData.Need);
                return interaction;
            }
            else
            {
                Debug.LogError("ERROR: Not interaction provider defined for the specified name");
                return null;
            }
        }

        protected virtual void SetInteractionEvents(SmartInteraction interaction, string name) 
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
