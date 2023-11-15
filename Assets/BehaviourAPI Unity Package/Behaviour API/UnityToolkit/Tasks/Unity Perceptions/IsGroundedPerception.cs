using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    /// <summary>
    /// Check if the agent is grounded. 
    /// <para>Requires <see cref="CharacterController"/> component.</para>
    /// </summary>
    public class IsGroundedPerception : UnityPerception
    {
        public override string ToString() => "Is grounded";

        public override bool Check()
        {
            return context.CharacterController.isGrounded;
        }
    }
}
