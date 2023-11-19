using UnityEngine;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core.Actions;
    using SmartObjects;

    public class AppleSmartObject : SmartObject
    {
        [SerializeField]
        FridgeSmartObject _fridge;

        [SerializeField, Range(0f, 1f)]
        float hungerCapability = 0.1f;

        private Dictionary<string, float> GetCapabilities()
        {
            Dictionary<string, float> capabilities = new Dictionary<string, float>();
            capabilities["hunger"] = hungerCapability;
            return capabilities;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            if (capabilityName == "hunger") return hungerCapability;
            else return 0f;
        }

        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            Action action = new TargetRequestAction(agent, _fridge, requestData);
            return new SmartInteraction(action, agent, GetCapabilities());
        }

        public override bool ValidateAgent(SmartAgent agent)
        {
            return _fridge.ValidateAgent(agent);
        }
    }
}