using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.UnityToolkit.Demos;
using UnityEngine;
using BehaviourAPI.SmartObjects;
using System.Collections.Generic;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class FridgeSmartObject : DirectSmartObject
    {
        [SerializeField] float useTime = 3f;
        [SerializeField] Transform fridgeDoor;
        float lieTime;

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            return new FunctionalAction(StartUse, () => OnUpdate(agent), StopUse);
        }

        void StartUse()
        {
            lieTime = Time.time;
            fridgeDoor.rotation = Quaternion.Euler(0, -90, 0);
        }

        void StopUse()
        {
            fridgeDoor.rotation = Quaternion.identity;
        }

        Status OnUpdate(SmartAgent smartAgent)
        {
            if (Time.time > lieTime + useTime)
            {
                return Status.Success;
            }
            return Status.Running;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            return 0f;
        }
    }
}

