using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class PickResourceProgressionUI : MonoBehaviour
    {

        public int minimum = 0;
        public int maximum = 3;
        public int current = 0;
        public Image mask;
        [SerializeField] private GameObject camera;

        private void Update()
        {
            transform.LookAt(camera.transform.position);
            GetCurrentFill();
        }

        private void GetCurrentFill()
        {
            float currentOffset = current - minimum;
            float maximumOffset = maximum - minimum;
            float fillAmount = currentOffset / maximumOffset;
            mask.fillAmount = fillAmount;
        }
    }
}