using GGG.Shared;

using System.Collections.Generic;
using UnityEngine;
using System;

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
        private Dictionary<string, Resource> _resources = new();

        private void Start()
        {
            foreach (string i in Enum.GetNames(typeof(BasicResources)))
                _resourcesCount.Add(i, 0);
            

            foreach (string i in Enum.GetNames(typeof(AdvanceResources)))
                _resourcesCount.Add(i, 0);
        }

        private void OnValidate()
        {
            if (BasicResources.Count < Enum.GetNames(typeof(BasicResources)).Length) { 
                Debug.LogWarning("There are resources basic missing inside the list");
                while (BasicResources.Count < Enum.GetNames(typeof(BasicResources)).Length)
                    BasicResources.Add(null);
            }
            else if (BasicResources.Count > Enum.GetNames(typeof(BasicResources)).Length) { 
                Debug.LogWarning("There are too many basic resources inside the list");
                while(BasicResources.Count > Enum.GetNames(typeof(BasicResources)).Length)
                    BasicResources.RemoveAt(BasicResources.Count - 1);
            }

            if (AdvanceResources.Count < Enum.GetNames(typeof(AdvanceResources)).Length)
            {
                Debug.LogWarning("There are advance resources missing inside the list");
                while (AdvanceResources.Count < Enum.GetNames(typeof(AdvanceResources)).Length)
                    AdvanceResources.Add(null);
            }
            else if (AdvanceResources.Count > Enum.GetNames(typeof(AdvanceResources)).Length)
            {
                Debug.LogWarning("There are too many advance resources inside the list");
                while (AdvanceResources.Count > Enum.GetNames(typeof(AdvanceResources)).Length)
                    AdvanceResources.RemoveAt(AdvanceResources.Count - 1);
            }
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

        public int GetResourceCount(BasicResources resourceType)
        {
            return _resourcesCount[resourceType.ToString()];
        }

        public int GetResourceCount(AdvanceResources resourceType)
        {
            return _resourcesCount[resourceType.ToString()];
        }

        public Resource GetResource(AdvanceResources resourcesType) { return _resources[resourcesType.ToString()]; }

        public void AddResource<T>(T resourceType, int amount) where T : Enum {
            if (!_resourcesCount.ContainsKey(resourceType.ToString()))
                throw new KeyNotFoundException("No resource found");
            

            _resourcesCount[resourceType.ToString()] += amount;
        }
    }
}
