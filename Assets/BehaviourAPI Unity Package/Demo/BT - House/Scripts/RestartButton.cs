using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class RestartButton : MonoBehaviour
    {

        #region variables

        [SerializeField] private GameObject testBoyPrefab;
        [SerializeField] private Door door;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private KeyRotation key;

        #endregion variables

        private void Start()
        {
            // The first iteration is bugged for some reason
            Restart();
        }

        public void Restart()
        {
            if (key.toggle.isOn) key.gameObject.SetActive(true);
            door.OnReset();

            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            if (oldPlayer != null)
            {
                Destroy(oldPlayer);
            }

            Instantiate(testBoyPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}