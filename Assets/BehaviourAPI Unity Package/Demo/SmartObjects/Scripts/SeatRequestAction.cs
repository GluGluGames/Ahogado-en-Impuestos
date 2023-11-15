using BehaviourAPI.SmartObjects;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class SeatRequestAction : UnityRequestAction
    {
        [Tooltip("Find the closest seat?")]
        public bool closest = false;

        [Tooltip("The time we want to use the seat")]
        public float useTime = 5f;

        public SeatRequestAction()
        {
        }

        public SeatRequestAction(SmartAgent agent, bool closest = false, float useTime = 5f) : base(agent)
        {
            this.closest = closest;
            this.useTime = useTime;
        }

        /// <summary>
        /// Sent the use time in the request.
        /// </summary>
        /// <returns>Create an specific request for seat smart objects.</returns>
        protected override RequestData GetRequestData()
        {
            return new SeatRequestData(useTime);
        }

        protected override SmartObject GetRequestedSmartObject()
        {
            return SeatManager.Instance.GetClosestSeat(Agent.transform.position);
        }
    }
}