using GGG.Components.HexagonalGrid;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Achievements;
using GGG.Shared;
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

        public Dictionary<Resource, int> resourcesCollected = new ();
        public List<ResourceComponent> resourcesOnScene = new List<ResourceComponent>();

        private Dictionary<Resource, ResourceType> _achievementResources = new();

        private int _nResourcesCollected = 0;
        private List<HexTile> _tiles;
        
        public enum ResourceType
        {
            Fish,
            Other
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            List<Resource> resources = UnityEngine.Resources.LoadAll<Resource>("SeaResources").Concat(
                UnityEngine.Resources.LoadAll<Resource>("FishResources")).Concat(
                UnityEngine.Resources.LoadAll<Resource>("ExpeditionResources")).ToList();

            foreach (Resource resource in resources)
                resourcesCollected.Add(resource, 0);

            Invoke("SpawnAllResources", Time.deltaTime * 5);

        }

        private void SpawnAllResources()
        {
            _tiles = FindObjectsOfType<HexTile>().ToList();
            for (int i = 0; i < _nResourcesMax; ++i)
            {
                SpawnRandomResource();
            }
        }

        private bool SpawnRandomResource()
        {
            HexTile hex = _tiles[Random.Range(0, _tiles.Count)];
            bool spawned = false;
            
            foreach (ResourceComponent resource in resourcesOnScene)
            {
                if (resource.currentTile != hex && hex.GetTileType() != TileType.Mountain && hex.selectable == true) continue;
                
                spawned = SpawnRandomResource();
                break;
            }

            if (_resourcePrefabs == null || spawned ) return true;
            
            int rand = Random.Range(0, _resourcePrefabs.Count);

            ResourceComponent newResource = Instantiate(_resourcePrefabs[rand], transform);
            newResource.transform.position = new Vector3(hex.transform.position.x, hex.transform.position.y + 1f, hex.transform.position.z);
            newResource.currentTile = hex;

            resourcesOnScene.Add(newResource);

            return true;
        }

        public int GetResourceAmount(Resource resource) =>
            resourcesOnScene.Find(x => x.GetResource() == resource).GetAmount();

        public void sumResourceCollected()
        {
            _nResourcesCollected++;
        }

        public void AddAchievementResource(Resource resource, ResourceType type)
        {
            if (_achievementResources.ContainsKey(resource)) return;
            
            _achievementResources.Add(resource, type);

            int x;
            if (type == ResourceType.Fish)
            {
                x = PlayerPrefs.HasKey("04") ? PlayerPrefs.GetInt("04") + 1 : 1;
                PlayerPrefs.SetInt("04", x);

                if (PlayerPrefs.GetInt("04") >= 5)
                    StartCoroutine(AchievementsManager.Instance.UnlockAchievement("04"));
            }
            else
            {
                x = PlayerPrefs.HasKey("03") ? PlayerPrefs.GetInt("03") + 1 : 1;
                PlayerPrefs.SetInt("03", x);

                if (PlayerPrefs.GetInt("03") >= 5)
                    StartCoroutine(AchievementsManager.Instance.UnlockAchievement("03"));
            }
            
        }

    }
}
