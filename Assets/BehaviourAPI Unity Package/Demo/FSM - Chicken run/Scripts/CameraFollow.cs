using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class CameraFollow : MonoBehaviour
    {

        #region variables

        [SerializeField] private float offsetY;
        [SerializeField] private float offsetZ;
        [SerializeField] private GameObject targetToFollow;

        #endregion variables

        // Update is called once per frame
        private void Update()
        {
            transform.position = new Vector3(targetToFollow.transform.position.x, offsetY, targetToFollow.transform.position.z - offsetZ);
        }
    }
}