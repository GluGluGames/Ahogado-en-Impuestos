using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class DestroyVehicles : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Car")
            {
                Destroy(other.gameObject);
            }
        }
    }
}