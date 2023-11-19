using UnityEngine;

namespace BehaviourAPI.UnityToolkit
{
    public class TransformMovementComponent : MonoBehaviour, IMovementComponent
    {
        [SerializeField] float speed;
        [SerializeField] float distanceThreshold;
        public float Speed { get => speed; set => speed = value; }

        private Vector3 target;

        bool isMoving;

        public void CancelMove()
        {
            target = Vector3.zero;
            isMoving = false;
        }

        public bool CanMove(Vector3 targetPos)
        {
            return true;
        }

        public Vector3 GetTarget()
        {
           return target;
        }

        public bool HasArrived()
        {
           return Vector3.Distance(transform.position, target) < distanceThreshold;
        }

        public void MoveInstant(Vector3 targetPos, Quaternion targetRot = default)
        {
            transform.SetLocalPositionAndRotation(targetPos, targetRot);
        }
         
        public void SetTarget(Vector3 targetPos)
        {
            target = targetPos;
            isMoving = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(isMoving)
            {
                transform.LookAt(target);
                transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            }
        }
    }
}
