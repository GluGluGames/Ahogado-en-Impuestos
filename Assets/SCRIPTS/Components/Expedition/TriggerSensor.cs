using UnityEngine;

namespace GGG.Components.Expedition
{
    public class TriggerSensor : MonoBehaviour
    {
        private int _overlaps;

        public bool isOverlapping
        {
            get
            {
                return _overlaps > 0;
            }
        }

        // Count how many colliders are overlapping this trigger.
        // If desired, you can filter here by tag, attached components, etc.
        // so that only certain collisions count. Physics layers help too.
        private void OnTriggerEnter(Collider other)
        {
            //_overlaps++;
            if (other.transform.gameObject.layer != LayerMask.NameToLayer("Enemy") || other.transform.gameObject.layer != LayerMask.NameToLayer("Berserker")) return;
            Lost();
        }

        private void OnTriggerExit(Collider other)
        {
            _overlaps--;
        }

        private void Lost()
        {
            FindObjectOfType<Timer>(true).Win(false);
        }

    }
}