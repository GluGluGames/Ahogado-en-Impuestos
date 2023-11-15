using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    using Core;
    using Core.Actions;
    using SmartObjects;

    public class BathubSmartObject : DirectSmartObject
    {

        [SerializeField] Transform _useTransform;
        [SerializeField] float useTime = 5f;

        [SerializeField] ParticleSystem _particleSystem;

        float lieTime;

        protected override Action GetUseAction(SmartAgent agent, RequestData requestData)
        {
            var liedown = new FunctionalAction(() => BedDown(agent), Wait, () => BedUp(agent));
            return liedown;
        }

        void BedDown(SmartAgent smartAgent)
        {
            _particleSystem.Play();
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
            _particleSystem?.Stop();
            smartAgent.transform.SetLocalPositionAndRotation(_placeTarget.position, _placeTarget.rotation);
            smartAgent.gameObject.GetComponent<NavMeshAgent>().enabled = enabled;
        }
    }
}