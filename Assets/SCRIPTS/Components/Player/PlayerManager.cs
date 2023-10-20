using GGG.Shared;

using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GGG.Components.Player
{
    public class PlayerManager : MonoBehaviour {
        #region Singleton

        public static PlayerManager Instance;

        private void Awake() {
            if (Instance != null) return;

            Instance = this;
        }

        #endregion

        [SerializeField] private List<Resource> Resources;
        [SerializeField] private Resource TempMainResource;

        private Dictionary<string, int> _resourcesCount = new();
        private Dictionary<string, Resource> _resources = new();

        private void Start()
        {
            foreach (Resource i in Resources)
                _resources.Add(i.GetName(), i);
            
            foreach (string i in _resources.Keys)
                _resourcesCount.Add(i, 0);
        }

        private void OnValidate()
        {
            Resources = UnityEngine.Resources.LoadAll<Resource>("SeaResources").
                Concat(UnityEngine.Resources.LoadAll<Resource>("ExpeditionResources")).
                Concat(UnityEngine.Resources.LoadAll<Resource>("FishResources")).ToList();
        }

        public int GetResourceCount(string key)
        {
            return _resourcesCount[key];
        }

        public Resource GetResource(string resourceKey) => _resources[resourceKey];

        public void AddResource(string resourceKey, int amount) {
            if (!_resourcesCount.ContainsKey(resourceKey))
                throw new KeyNotFoundException("No resource found");
            

            _resourcesCount[resourceKey] += amount;
        }

        public int GetResourceNumber() => _resources.Count;

        public Resource GetMainResource() => TempMainResource;
    }
}
