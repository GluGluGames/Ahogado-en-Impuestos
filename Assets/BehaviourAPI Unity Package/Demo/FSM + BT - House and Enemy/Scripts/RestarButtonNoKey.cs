using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class RestarButtonNoKey : MonoBehaviour
    {

        #region variables

        [SerializeField] private GameObject testBoy;
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
            GameObject oldPlayer = GameObject.FindGameObjectWithTag("Player");
            key.gameObject.SetActive(true);

            if (oldPlayer != null)
            {
                Destroy(oldPlayer);
            }
            Instantiate(testBoy, spawnPoint.position, spawnPoint.rotation);
        }
    }
}