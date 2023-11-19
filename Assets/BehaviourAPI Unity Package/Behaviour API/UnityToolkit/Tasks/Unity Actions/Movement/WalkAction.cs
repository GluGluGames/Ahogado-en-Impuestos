using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that moves the agent to a determined position.
    /// </summary>
    [SelectionGroup("MOVEMENT")]
    public class WalkAction : UnityAction
    {
        /// <summary>
        /// The target position.
        /// </summary>
        public Vector3 Target;

        /// <summary>
        /// Create a new WalkAction
        /// </summary>
        public WalkAction()
        {
        }

        /// <summary>
        /// Create a new WalkAction
        /// </summary>
        /// <param name="target">The target position.</param>
        /// <param name="speed">The movement speed of the agent. </param>
        public WalkAction(Vector3 target)
        {
            Target = target;
        }

        public override void Start()
        {
            context.Movement.SetTarget(Target);
        }

        public override Status Update()
        {
            if (context.Movement.HasArrived())
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
        }

        public override string ToString() => $"Walk to {Target}";
    }
}

