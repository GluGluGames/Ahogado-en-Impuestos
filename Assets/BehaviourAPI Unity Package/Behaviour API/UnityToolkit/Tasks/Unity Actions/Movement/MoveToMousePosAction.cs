using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    using Core;
    /// <summary>
    /// Custom action that moves an agent to a given position, returning success when the position is arrived.
    /// </summary>

    [SelectionGroup("MOVEMENT")]
    public class MoveToMousePosAction : UnityAction
    {
        public float maxRayDistance = 100f;
        public LayerMask layerMask = -1;

        /// <summary>
        /// Create a new MoveToMousePosAction
        /// </summary>
        public MoveToMousePosAction()
        {
        }

        public override void Start()
        {
            Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(cameraRay, out RaycastHit hit, maxRayDistance, layerMask))
            {
                context.Movement.SetTarget(hit.point);
            }
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
            {
                return Status.Running;
            }

        }

        public override string ToString() => "Move to mouse position";
    }
}

