using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using BehaviourAPI.SmartObjects;
    using Core.Actions;
    using System.Collections.Generic;
    using UnityToolkit;

    /// <summary> 
    /// A subtype of the SmartObject that the agent navigates to in order to use it. 
    /// Also, the item can only be used by one agent at a time.
    /// The interaction that the object provides can cover one need defined by capabilityName and capabilityValue;
    /// </summary>

    public abstract class DirectSmartObject : SmartObject
    {
        [Tooltip("The target where the agent must be placed to use the item")]
        [SerializeField] protected Transform _placeTarget;

        [Header("Capability")]
        [SerializeField] string capabilityName;
        [SerializeField, Range(0f, 1f)] float capabilityValue;
        /// <summary> 
        /// Gets or sets the owner. 
        /// The current agent using the object. If the property has value
        /// the object is not selectable for other agents.
        /// </summary>
        /// <value> The owner. </value>
        public SmartAgent Owner { get; protected set; }

        public override bool ValidateAgent(SmartAgent agent)
        {
            return Owner == null;
        }

        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            Action action = GenerateAction(agent, requestData);
            SmartInteraction interaction = new SmartInteraction(action, agent, GetCapabilities());
            SetInteractionEvents(interaction, agent);
            return interaction;
        }

        private Action GenerateAction(SmartAgent agent, RequestData requestData)
        {
            // Simple way to make an action sequence
            SequenceAction sequence = new SequenceAction();

            // First step: move to smart object
            sequence.SubActions.Add(new WalkAction(_placeTarget.position));

            // Second step: use the smart object
            sequence.SubActions.Add(GetUseAction(agent, requestData));

            return sequence;
        }

        protected abstract Action GetUseAction(SmartAgent agent, RequestData requestData);

        protected virtual void SetInteractionEvents(SmartInteraction interaction, SmartAgent agent)
        {
            interaction.OnInitialize += () => OnInitInteraction(agent);
            interaction.OnRelease += () => OnReleaseInteraction(agent);
        }

        void OnInitInteraction(SmartAgent agent)
        {
            if (_registerOnManager)
                SmartObjectManager.Instance?.UnregisterSmartObject(this);

            if (Owner != null)
                Debug.LogError("Error: Trying to init a smart object interaction when the agent is not null", this);

            Owner = agent;
        }

        void OnReleaseInteraction(SmartAgent agent)
        {
            if (_registerOnManager)
                SmartObjectManager.Instance?.RegisterSmartObject(this);

            if (Owner != agent)
                Debug.LogError("Error: Trying to release a smart object interaction from an Agent that is not the owner", this);

            Owner = null;
        }

        private Dictionary<string, float> GetCapabilities()
        {
            Dictionary<string, float> capabilities = new Dictionary<string, float>();

            if(!string.IsNullOrEmpty(capabilityName))
                capabilities[capabilityName] = capabilityValue;

            return capabilities;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            if (this.capabilityName == capabilityName) return capabilityValue;
            else return 0f;
        }
    }
}
