using DG.DemiEditor;
using System.Security.Cryptography;
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

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.gameObject.layer != LayerMask.NameToLayer("Enemy") && other.transform.gameObject.layer != LayerMask.NameToLayer("Berserker")) return;
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