using BehaviourAPI.Core;
using BehaviourAPI.Core.Actions;
using BehaviourAPI.SmartObjects;
using BehaviourAPI.UnityToolkit;
using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class BedSmartObject : DirectSmartObject
    {
        [SerializeField] Transform _useTransform;
        [SerializeField] float useTime = 5f;

        float lieTime;
        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            var liedown = new FunctionalAction(() => BedDown(agent), Wait, () => BedUp(agent));
            return liedown;
        }

        void BedDown(SmartAgent smartAgent)
        {
            lieTime = Time.time;
            smartAgent.gameObject.GetComponent<NavMeshAgent>().enabled = false;
            smartAgent.transform.SetLocalPositionAndRotation(_useTransform.position, _useTransform.rotation);
        }

        Status Wait()
        {
            if (Time.time > lieTime + useTime)
            {
                return Status.Success;
            }
            return Status.Running;
        }

        void BedUp(SmartAgent smartAgent)
        {
            smartAgent.transform.SetLocalPositionAndRotation(_placeTarget.position, _placeTarget.rotation);
            smartAgent.gameObject.GetComponent<NavMeshAgent>().enabled = enabled;
        }
    }

}