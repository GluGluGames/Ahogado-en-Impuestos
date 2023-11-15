using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Perception that checks the distance to another object.
    /// </summary>
    public class DistancePerception : UnityPerception
    {
        /// <summary>
        /// The transform of the other object.
        /// </summary>
        public Transform OtherTransform;

        /// <summary>
        /// The distance to trigger the perception.
        /// </summary>
        public float MaxDistance;

        /// <summary>
        /// Create a new Distance perception.
        /// </summary>
        /// <param name="otherTransform">The transform of the other object.</param>
        /// <param name="maxdistance">The distance to trigger the perception.</param>
        public DistancePerception(Transform otherTransform, float maxdistance)
        {
            OtherTransform = otherTransform;
            MaxDistance = maxdistance;
        }

        /// <summary>
        /// Create a new Distance perception.
        /// </summary>
        public DistancePerception()
        {
        }

        public override bool Check()
        {
            return Vector3.Distance(context.Transform.position, OtherTransform.position) < MaxDistance;
        }

        public override string ToString() => $"If dist to {OtherTransform} < {MaxDistance}";
    }
}
