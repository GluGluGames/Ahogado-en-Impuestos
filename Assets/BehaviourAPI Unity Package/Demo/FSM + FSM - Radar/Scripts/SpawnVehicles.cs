using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourAPI.UnityToolkit.Demos
{
    public class SpawnVehicles : MonoBehaviour
    {
        #region variables

        [SerializeField] private List<GameObject> _vehicles = new List<GameObject>();

        [SerializeField] List<Transform> _spawnPoints;

        #endregion variables

        // Start is called before the first frame update
        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(2f, 4f));
                SpawnVehicle();
            }
        }

        private void SpawnVehicle()
        {
            int vehicleIndex = Random.Range(0, _vehicles.Count);
            var tf = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            Instantiate(_vehicles[vehicleIndex], tf.position, tf.rotation);
        }
    }
}