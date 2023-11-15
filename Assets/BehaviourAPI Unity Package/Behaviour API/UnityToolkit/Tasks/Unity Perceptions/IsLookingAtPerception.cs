using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Check if the agent is looking an object
    /// </summary>
    public class IsLookingAtPerception : UnityPerception
    {
        /// <summary>
        /// The transform that the agent is looking at.
        /// </summary>
        public Transform OtherTransform;

        /// <summary>
        /// The minimum distance the object is visible.
        /// </summary>
        public float minDist;

        /// <summary>
        /// The maximum distance the object is visible.
        /// </summary>            
        public float maxDist;

        /// <summary>
        /// The angle field of view.
        /// </summary>
        public float maxAngle;

        public override bool Check()
        {
            var delta = OtherTransform.position - context.Transform.position;

            if (delta.magnitude < minDist || delta.magnitude > maxDist) return false;

            var lookAt = context.Transform.forward;

            return Vector3.Angle(lookAt, delta) < maxAngle;
        }

        public override string ToString() => $"Is looking at {OtherTransform}";
    }
}
