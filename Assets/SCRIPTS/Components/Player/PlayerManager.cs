using GGG.Shared;

using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace GGG.Components.Player
{
    public class PlayerManager : MonoBehaviour {
        #region Singleton

        public static PlayerManager Instance;

        private void Awake() {
            
            if (Instance == null)
                Instance = this;
        }

        #endregion
        
        private static List<Resource> _resources;
        private static Dictionary<string, int> _resourcesCount = new();
        private static readonly Dictionary<string, Resource> _resourcesDictionary = new();

        private void Start()
        {
            _resources = Resources.LoadAll<Resource>("SeaResources").
                Concat(Resources.LoadAll<Resource>("ExpeditionResources")).
                Concat(Resources.LoadAll<Resource>("FishResources")).ToList();

            if (_resourcesDictionary.Count <= 0)
            {
                foreach (Resource i in _resources)
                    _resourcesDictionary.Add(i.GetKey(), i);
            }

            if (_resourcesCount.Count > 0) return;
            
            HandleResourceDictionary();
        }

        public int GetResourceCount(string key) => _resourcesCount[key];
        public Dictionary<string, int> ResourcesCount() => _resourcesCount;
        public void SetResourcesCount(Dictionary<string, int> x) => _resourcesCount = x;

        public void HandleResourceDictionary()
        {
            foreach (string i in _resourcesDictionary.Keys)
                _resourcesCount.Add(i, 0);
        }

        public List<Resource> GetResources() => _resources;

        public Resource GetResource(string resourceKey) => 
            _resourcesDictionary.TryGetValue(resourceKey, out Resource value) ? value : null;
        
        public void AddResource(string resourceKey, int amount) {
            if (!_resourcesCount.ContainsKey(resourceKey))
                throw new KeyNotFoundException("No resource found");
            
            _resourcesCount[resourceKey] += amount;
        }

        public int GetResourceNumber() => _resourcesDictionary.Count;
    }
}
