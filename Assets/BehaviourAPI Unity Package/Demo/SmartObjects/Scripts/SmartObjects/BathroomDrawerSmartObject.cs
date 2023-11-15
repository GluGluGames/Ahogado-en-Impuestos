using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BathroomDrawerSmartObject : DirectSmartObject
    {
        [SerializeField] ParticleSystem _particleSystem;
        [SerializeField] float useTime = 5f;

        [SerializeField, Range(0f, 1f)]
        float hygieneCapability = 0.8f;

        float startTime;

        private Dictionary<string, float> GetCapabilities()
        {
            Dictionary<string, float> capabilities = new Dictionary<string, float>();
            capabilities["hygiene"] = hygieneCapability;
            return capabilities;
        }

        public override float GetCapabilityValue(string capabilityName)
        {
            if (capabilityName == "hygiene") return hygieneCapability;
            else return 0f;
        }

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            return new FunctionalAction(() => StartUse(agent), Wait, () => StopUse(agent));
        }

        void StartUse(SmartAgent smartAgent)
        {
            smartAgent.transform.SetPositionAndRotation(_placeTarget.position, _placeTarget.rotation);
            startTime = Time.time;
            _particleSystem.Play();
        }

        void StopUse(SmartAgent smartAgent)
        {
            _particleSystem?.Stop();
        }

        Status Wait()
        {
            if (Time.time > startTime + useTime)
            {
                return Status.Success;
            }
            return Status.Running;
        }
    }

}