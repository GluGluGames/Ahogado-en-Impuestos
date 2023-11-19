using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using SmartObjects;
    using Core.Actions;

    public class DrinkSmartObject : SmartObject
    {
        [SerializeField] FridgeSmartObject _fridge;

        [SerializeField, Range(0f, 1f)]
        float thirstCapability = 0.5f;

        private Dictionary<string, float> GetCapabilities()
        {
            Dictionary<string, float> capabilities = new Dictionary<string, float>();
            capabilities["thirst"] = thirstCapability;
            return capabilities;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            if (capabilityName == "thirst") return thirstCapability;
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