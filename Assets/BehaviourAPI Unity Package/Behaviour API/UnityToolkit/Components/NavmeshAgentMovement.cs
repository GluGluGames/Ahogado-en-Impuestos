using UnityEngine;
using UnityEngine.AI;

namespace BehaviourAPI.UnityToolkit
{
    public class NavmeshAgentMovement : MonoBehaviour, IMovementComponent
    {
        [SerializeField] float speed;
        [SerializeField] float targetThreshold = 1f;
        [SerializeField] float minDistanceToTarget = 0.3f;

        [Header("Debug")]
        [SerializeField] bool drawGizmos;
        [SerializeField] Color targetColor = Color.red;

        NavMeshAgent m_NavMeshAgent;

        public float Speed
        {
            get => m_NavMeshAgent.speed;
            set => m_NavMeshAgent.speed = value;
        }

        void Awake()
        {
            m_NavMeshAgent = GetComponent<NavMeshAgent>();
        }

        void Start()
        {
            m_NavMeshAgent.speed = speed;
        }

        public void SetTarget(Vector3 targetPos)
        {
            m_NavMeshAgent.isStopped = false;
            m_NavMeshAgent.SetDestination(targetPos);
        }

        public bool HasArrived()
        {
            if (Vector3.Distance(transform.position, m_NavMeshAgent.destination) < minDistanceToTarget)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanMove(Vector3 targetPos)
        {
            return NavMesh.SamplePosition(targetPos, out var _, targetThreshold, NavMesh.AllAreas);
        }

        public Vector3 GetTarget()
        {
            return m_NavMeshAgent.destination;
        }

        public void CancelMove()
        {
            if (m_NavMeshAgent != null && m_NavMeshAgent.isOnNavMesh)
                m_NavMeshAgent.isStopped = true;
        }

        public void MoveInstant(Vector3 targetPos, Quaternion targetRot = default)
        {
            transform.SetPositionAndRotation(targetPos, targetRot);
            m_NavMeshAgent.isStopped = true;
        }

        void OnDrawGizmos()
        {
            if (m_NavMeshAgent != null && drawGizmos)
            {
                Gizmos.color = targetColor;
                var target = GetTarget();
                Gizmos.DrawLine(transform.position, target);
                Gizmos.DrawSphere(target, 0.5f);
            }
        }
    }
}
