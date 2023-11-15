using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that moves an agent away from a transform, returning success when the position is arrived.
    /// </summary>

    [SelectionGroup("MOVEMENT")]
    public class FleeAction : UnityAction
    {
        /// <summary>
        /// The transformation from which the agent flees.
        /// </summary>
        public Transform OtherTransform;

        /// <summary>
        /// The movement speed of the agent.
        /// </summary>
        public float speed;

        /// <summary>
        /// The distance of the target point.
        /// </summary>
        public float distance;

        /// <summary>
        /// The maximum time the agent will run.
        /// </summary>
        public float maxTimeRunning;

        float _timeRunning;

        Vector3 _target;

        /// <summary>
        /// Create a new flee action.
        /// </summary>
        public FleeAction() { }

        /// <summary>
        /// Create a new flee action
        /// </summary>
        /// <param name="otherTransform">The transformation from which the agent flees.</param>
        /// <param name="speed">The movement speed of the agent.</param>
        /// <param name="distance">The distance of the target point.</param>
        /// <param name="maxTimeRunning">The maximum time the agent will run.</param>
        public FleeAction(Transform otherTransform, float speed, float distance, float maxTimeRunning)
        {
            OtherTransform = otherTransform;
            this.speed = speed;
            this.distance = distance;
            this.maxTimeRunning = maxTimeRunning;
        }

        public override void Start()
        {
            _timeRunning = Time.time;
            context.Movement.Speed *= speed;
        }

        public override void Stop()
        {
            context.Movement.CancelMove();
            context.Movement.Speed /= speed;
        }

        public override Status Update()
        {
            if (_timeRunning + maxTimeRunning < Time.time) return Status.Failure;

            if (Vector3.Distance(context.Transform.position, OtherTransform.position) > distance)
            {
                return Status.Success;
            }
            else
            {
                var dir = (context.Transform.position - OtherTransform.position).normalized;
                var targetPos = context.Transform.position + dir;
                context.Movement.SetTarget(targetPos);
                return Status.Running;
            }
        }
        public override string ToString() => $"Flee from {OtherTransform}";
    }
}