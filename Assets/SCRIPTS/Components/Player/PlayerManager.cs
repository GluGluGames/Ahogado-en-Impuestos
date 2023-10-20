using GGG.Shared;

using System.Collections.Generic;
using UnityEngine;
using System;
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

        [SerializeField] private List<BasicResource> BasicResources;
        [SerializeField] private List<AdvanceResource> AdvanceResources;

        private Dictionary<string, int> _resourcesCount = new();
        private Dictionary<string, BasicResource> _resources = new();

        private void Start()
        {
            foreach (BasicResource i in BasicResources)
                _resources.Add(i.GetName().GetLocalizedString(), i);
            
            foreach (string i in _resources.Keys)
                _resourcesCount.Add(i, 0);
        }

        private void OnValidate()
        {
            BasicResources = Resources.LoadAll<BasicResource>("SeaResources").
                Concat(Resources.LoadAll<BasicResource>("ExpeditionResources")).
                Concat(Resources.LoadAll<BasicResource>("FishResources")).ToList();
        }

        public Resource GetResource<T>(T resourceType) where T : Enum
        {
            Resource resource = null;
            bool found = false;
            int i = 0;

            while (!found && i < BasicResources.Count) {
                if (BasicResources[i].GetResource().Equals(resourceType)) {
                    resource = BasicResources[i];
                    found = true;
                }

                i++;
            }

            if(found) return resource;

            while (!found && i < AdvanceResources.Count) {
                if (AdvanceResources[i].GetResource().Equals(resourceType)) {
                    resource = AdvanceResources[i];
                    found = true;
                }

                i++;
            }

            if (resource == null)
                throw new Exception("No resource found");

            return resource;
        }

        public int GetResourceCount(string key)
        {
            return _resourcesCount[key];
        }

        public Resource GetResource(AdvanceResources resourcesType) { return _resources[resourcesType.ToString()]; }

        public void AddResource(string resourceKey, int amount) {
            if (!_resourcesCount.ContainsKey(resourceKey))
                throw new KeyNotFoundException("No resource found");
            

            _resourcesCount[resourceKey] += amount;
        }

        public int GetResourceNumber() => _resources.Count;
    }
}
