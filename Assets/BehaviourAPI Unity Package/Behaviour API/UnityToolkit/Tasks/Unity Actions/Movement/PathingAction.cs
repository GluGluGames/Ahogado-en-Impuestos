using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;

    /// <summary>
    /// Action that moves the agent along a path.
    /// </summary>
    [SelectionGroup("MOVEMENT")]
    public class PathingAction : UnityAction
    {
        /// <summary>
        /// The points that forms the path.
        /// </summary>
        public List<Vector3> positions;

        /// <summary>
        /// The distance the agent must be from one point to go to the next.
        /// </summary>
        public float distanceThreshold;

        int currentTargetPosId;

        /// <summary>
        /// Create a new PathingAction.
        /// </summary>
        public PathingAction() { }

        /// <summary>
        /// Create a new PathingAction.
        /// </summary>
        /// <param name="positions">The points that forms the path.</param>
        /// <param name="speed">The movement speed of the agent.</param>
        /// <param name="distanceThreshold">The distance the agent must be from one point to go to the next.</param>
        public PathingAction(List<Vector3> positions, float distanceThreshold)
        {
            this.positions = positions;
            this.distanceThreshold = distanceThreshold;
        }

        public override string ToString() => $"Move between ({positions.Count}) positions.";

        public override void Start()
        {
            currentTargetPosId = 0;

            if (positions.Count > 0)
            {
                context.Movement.SetTarget(positions[currentTargetPosId]);
            }
        }

        public override Status Update()
        {
            if (positions.Count == 0) return Status.Failure;

            if (context.Movement.HasArrived())
            {
                currentTargetPosId++;

                if (currentTargetPosId >= positions.Count)
                {
                    currentTargetPosId = 0;
                    return Status.Success;
                }
                else
                {
                    context.Movement.SetTarget(positions[currentTargetPosId]);
                }
            }

            return Status.Running;
        }
    }
}
