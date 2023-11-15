using UnityEngine;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core.Actions;
    using SmartObjects;

    public class ChickenSmartObject : SmartObject
    {
        [SerializeField] FridgeSmartObject _fridge;
        [SerializeField] OvenSmartObject _oven;

        [SerializeField, Range(0f, 1f)]
        float hungerCapability = 0.1f;

        public override bool ValidateAgent(SmartAgent agent)
        {
            return _fridge.ValidateAgent(agent);
        }

        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            SequenceAction sequence = new SequenceAction();

            sequence.SubActions.Add(new TargetRequestAction(agent, _fridge, requestData));
            sequence.SubActions.Add(new TargetRequestAction(agent, _oven, requestData));
            sequence.SubActions.Add(new SeatRequestAction(agent));
            return new SmartInteraction(sequence, agent, GetCapabilities());
        }

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
    }
}