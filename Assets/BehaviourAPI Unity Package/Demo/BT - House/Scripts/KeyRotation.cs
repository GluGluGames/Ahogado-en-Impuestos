using UnityEngine;
using UnityEngine.UI;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class KeyRotation : MonoBehaviour
    {
        public Toggle toggle;

        private void Start()
        {
            if (toggle != null)
                gameObject.SetActive(toggle.isOn);
        }

        private void Update()
        {
            transform.Rotate(new Vector3(0, 1.5f, 0));
        }
    }
}