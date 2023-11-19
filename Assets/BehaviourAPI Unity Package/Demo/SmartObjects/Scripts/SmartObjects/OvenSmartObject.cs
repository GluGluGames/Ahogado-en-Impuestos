using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class OvenSmartObject : DirectSmartObject
    {
        [SerializeField] float useTime = 3f;
        [SerializeField] Light _light;

        float lieTime;

        private void Awake()
        {
            _light.enabled = false;
        }

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            return new FunctionalAction(StartUsing, () => OnUpdate(agent), StopUsing);
        }

        void StartUsing()
        {
            lieTime = Time.time;
            _light.enabled = true;
        }

        void StopUsing()
        {
            if (_light != null)
                _light.enabled = false;
        }

        Status OnUpdate(SmartAgent smartAgent)
        {
            if (Time.time > lieTime + useTime)
            {
                return Status.Success;
            }
            return Status.Running;
        }
    }

}