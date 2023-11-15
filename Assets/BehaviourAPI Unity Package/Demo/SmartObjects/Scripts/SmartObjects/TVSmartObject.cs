using UnityEngine;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core.Actions;
    using SmartObjects;
    using UnityToolkit;


    public class TVSmartObject : SmartObject
    {
        [SerializeField] float maxDistance;
        [SerializeField] float useTime;

        [SerializeField, Range(0f, 1f)] float leisureCapability = 0.5f;

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
            Action action = new TVSeatRequestAction(agent, transform, maxDistance, useTime);
            return new SmartInteraction(action, agent, GetCapabilities());
        }

        public override bool ValidateAgent(SmartAgent agent)
        {
            return true;
        }

        private class TVSeatRequestAction : UnityRequestAction
        {
            public Transform requestTf;
            public float maxDistance;
            public float useTime = 5f;

            public TVSeatRequestAction(SmartAgent agent, Transform requestTf, float maxDistance, float useTime) : base(agent)
            {
                this.requestTf = requestTf;
                this.maxDistance = maxDistance;
                this.useTime = useTime;
            }

            protected override RequestData GetRequestData()
            {
                return new SeatRequestData(useTime);
            }

            protected override SmartObject GetRequestedSmartObject()
            {
                SeatSmartObject requestedSeat;
                if (SeatManager.Instance != null)
                {
                    requestedSeat = SeatManager.Instance.GetRandomSeat(requestTf.position, maxDistance);
                }
                else
                {
                    requestedSeat = null;
                }
                return requestedSeat;
            }
        }
    }

}