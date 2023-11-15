using UnityEngine;
using UnityEngine.AI;

public class NPCPoseController : MonoBehaviour
{
    private static readonly Vector3 k_SittingRot = new Vector3(-45, 0, 0);
    private static readonly Vector3 k_LayDownRot = new Vector3(-90, 0, 0);
    [SerializeField] Transform _tfBody;
    [SerializeField] Transform _tfModel;

    private NavMeshAgent m_NavMeshAgent;

    private void Awake()
    {
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
    }

    public void ChangeToSittingPose()
    {
        m_NavMeshAgent.enabled = false;
        _tfBody.rotation = Quaternion.Euler(k_SittingRot);
    }

    public void ChangeToReleasePose()
    {
        _tfBody.rotation = Quaternion.identity;
        m_NavMeshAgent.enabled = true;
    }

    public void ChangeToStaticPose()
    {
        _tfBody.rotation = Quaternion.Euler(k_SittingRot);
        m_NavMeshAgent.enabled = false;
    }
}
