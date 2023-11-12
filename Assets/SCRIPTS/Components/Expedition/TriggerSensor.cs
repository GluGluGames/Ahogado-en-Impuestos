using UnityEngine;
using GGG.Components.UI;

namespace GGG.Components.Player
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
            if (other.transform.gameObject.layer == 9) return;
            Lost();
        }

        private void OnTriggerExit(Collider other)
        {
            _overlaps--;
        }

        private void Lost()
        {
            FindObjectOfType<Timer>().Pause();
            FindObjectOfType<EndExpeditionUI>().OnEndGame(false);
        }

    }
}