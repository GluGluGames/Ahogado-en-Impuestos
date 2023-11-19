using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that moves an agent to a random position.
    /// </summary>
    [SelectionGroup("MOVEMENT")]
    public class PatrolAction : UnityAction
    {
        /// <summary>
        /// The max distance of the target point.
        /// </summary>
        public float maxDistance;

        /// <summary>
        /// Create a new PatrolAction
        /// </summary>
        public PatrolAction()
        {
        }

        /// <summary>
        /// Create a new PatrolAction
        /// </summary>
        /// <param name="speed">The movement speed of the agent.</param>
        /// <param name="maxDistance">The max distance of the target point.</param>
        public PatrolAction(float maxDistance)
        {
            this.maxDistance = maxDistance;
        }

        public override void Start()
        {
            Vector2 positionToRun = Random.insideUnitCircle * maxDistance;
            Vector3 desp = new Vector3(positionToRun.x, 0, positionToRun.y);
            context.Movement.SetTarget(context.Transform.position + desp);
        }

        public override void Stop()
        {
            context.Movement.CancelMove();
        }

        public override Status Update()
        {
            if (context.Movement.HasArrived())
            {
                return Status.Success;
            }
            else
                return Status.Running;
        }

        public override string ToString() => $"Move to a random position in a {maxDistance} radius circle.";
    }
}
