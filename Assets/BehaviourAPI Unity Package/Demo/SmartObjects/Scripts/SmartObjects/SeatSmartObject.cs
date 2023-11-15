using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class SeatSmartObject : DirectSmartObject
    {
        private static float k_DefaultUseTime = 5f;

        [Tooltip("The position where the agent")]
        [SerializeField] Transform _useTarget;

        float _useTime = k_DefaultUseTime;

        NPCPoseController _poseController;

        float lieTime;

        public override float GetCapabilityValue(string capabilityName)
        {
            return 0f;
        }

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            float usetime = k_DefaultUseTime;
            if(requestData is SeatRequestData seatRequestData) usetime = seatRequestData.useTime;

            Action action = new FunctionalAction(() => SitDown(agent), () => Wait(usetime), () => SitUp(agent));
            return action;
        }

        void SitDown(SmartAgent smartAgent)
        {
            lieTime = Time.time;
            _poseController = smartAgent.gameObject.GetComponent<NPCPoseController>();
            _poseController.ChangeToSittingPose();
            smartAgent.transform.SetPositionAndRotation(_useTarget.position, _useTarget.rotation);
        }

        void SitUp(SmartAgent smartAgent)
        {
            if (smartAgent == null && _placeTarget != null) return;

            smartAgent.transform.SetLocalPositionAndRotation(_placeTarget.position, _placeTarget.rotation);
            _poseController?.ChangeToReleasePose();
            _useTime = k_DefaultUseTime;
        }

        Status Wait(float useTime)
        {
            if (Time.time > lieTime + useTime)
            {
                return Status.Success;
            }
            return Status.Running;
        }
    }
}