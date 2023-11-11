using BehaviourAPI.Core.Actions;
using UnityEngine;
using BehaviourAPI.SmartObjects;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class ComputerDesktopSmartObject : SmartObject
    {
        [SerializeField] Transform target;
        [SerializeField] SeatSmartObject seat;

        [SerializeField, Range(0f, 1f)]
        float leisureCapability = 0.4f;

        private Dictionary<string, float> GetCapabilities()
        {
            Dictionary<string, float> capabilities = new Dictionary<string, float>();
            capabilities["leisure"] = leisureCapability;
            return capabilities;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            if (capabilityName == "leisure") return leisureCapability;
            else return 0f;
        }
        public override SmartInteraction RequestInteraction(SmartAgent agent, RequestData requestData)
        {
            Action action = new TargetRequestAction(agent, seat, requestData);
            return new SmartInteraction(action, agent, GetCapabilities());
        }

        public override bool ValidateAgent(SmartAgent agent)
        {
            return seat.ValidateAgent(agent);
        }
    }
}