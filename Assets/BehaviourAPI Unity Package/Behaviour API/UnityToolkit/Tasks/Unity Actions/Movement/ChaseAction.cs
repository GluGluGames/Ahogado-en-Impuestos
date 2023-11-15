using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that makes the agent chase another object.
    /// </summary>
    [SelectionGroup("MOVEMENT")]
    public class ChaseAction : UnityAction
    {
        /// <summary>
        /// The movement speed of the agent.
        /// </summary>
        public float speed;

        /// <summary>
        /// The transform that the agent chase.
        /// </summary>
        public Transform target;

        /// <summary>
        /// The distance that the agent must be to its target to end with success.
        /// </summary>
        public float maxDistance;

        /// <summary>
        /// The max time that the agent will chase.
        /// </summary>
        public float maxTime;

        float _timeRunning;

        /// <summary>
        /// Create a new Chase Action.
        /// </summary>
        public ChaseAction() { }

        /// <summary>
        /// Create a new Chase Action.
        /// </summary>
        /// <param name="target">The transform that the agent chase.</param>
        /// <param name="speed">The movement speed of the agent.</param>
        /// <param name="maxDistance">The distance that the agent must be to its target to end with success.</param>
        /// <param name="maxTime">The max time that the agent will chase.</param>
        public ChaseAction(Transform target, float speed, float maxDistance, float maxTime)
        {
            this.speed = speed;
            this.target = target;
            this.maxTime = maxTime;
            this.maxDistance = maxDistance;
        }

        public override void Start()
        {
            _timeRunning = Time.time;
            context.Movement.Speed *= speed;
        }

        public override Status Update()
        {
            if (_timeRunning + maxTime < Time.time) return Status.Failure;

            context.Movement.SetTarget(target.position);

            if (Vector3.Distance(target.position, context.Transform.position) < maxDistance)
            {
                return Status.Success;
            }
            else
            {
                return Status.Running;
            }
        }

        public override void Stop()
        {
            context.Movement.CancelMove();
            context.Movement.Speed *= 1f / speed;
        }
        public override string ToString() => $"Chase {target} for {maxTime} second(s)";
    }

}