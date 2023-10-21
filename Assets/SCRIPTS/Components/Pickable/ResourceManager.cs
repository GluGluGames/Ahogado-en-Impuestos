using GGG.Shared;
using GGG.Components.Buildings;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        #region Singleton
        public static ResourceManager Instance { get; private set; }
        #endregion

        [SerializeField] private List<ResourceComponent> _resourcePrefabs = new List<ResourceComponent>();
        [SerializeField] private int _nResourcesMax;

        public Dictionary<string, int> resourcesCollected = new Dictionary<string, int>();
        public List<ResourceComponent> resourcesOnScene = new List<ResourceComponent>();

        private int _nResourcesCollected = 0;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            for(int i = 0; i < _nResourcesMax; ++i)
            {
                SpawnRandomResource();
            }
        }

        private bool SpawnRandomResource()
        {
            HexTile hex = TileManager.instance.GetRandomHex();
            bool spawned = false;
            foreach (ResourceComponent resource in resourcesOnScene)
            {
                if (resource.currentTile == hex)
                {
                    spawned = SpawnRandomResource();
                    break;
                }
            }
            if (_resourcePrefabs != null && spawned == false)
            {
                int rand = Random.Range(0, _resourcePrefabs.Count);

                ResourceComponent newResource = Instantiate(_resourcePrefabs[rand]);
                newResource.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
                newResource.currentTile = hex;

                resourcesOnScene.Add(newResource);
            }

            return true;
        }

        public void sumResourceCollected()
        {
            _nResourcesCollected++;
        }

    }
}
