using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public interface IMovementComponent
    {
        /// <summary>
        /// Get the position that the agent is currently moving.
        /// </summary>
        /// <returns>The current target position of the agent, or the current position if its not moving.</returns>
        Vector3 GetTarget();

        /// <summary>
        /// Change the current target
        /// </summary>
        /// <param name="targetPos">The new target position.</param>
        void SetTarget(Vector3 targetPos);

        /// <summary>
        /// Change the current target and move the agent instantly to the target.
        /// </summary>
        /// <param name="targetPos"></param>
        /// <param name="targetRot"></param>
        void MoveInstant(Vector3 targetPos, Quaternion targetRot = default);

        /// <summary>
        /// Get if the agent has arrived the destination target.
        /// </summary>
        /// <returns>True if the agent position is it's target.</returns>
        bool HasArrived();

        /// <summary>
        /// Stops the current movement.
        /// </summary>
        void CancelMove();

        /// <summary>
        /// Gets if the agent can move to the specified position.
        /// </summary>
        /// <param name="targetPos"></param>
        /// <returns></returns>
        bool CanMove(Vector3 targetPos);

        /// <summary>
        /// Get or set the current agent speed.
        /// </summary>
        float Speed { get; set; }
    }
}
