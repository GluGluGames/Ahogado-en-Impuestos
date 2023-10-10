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
            foreach(string i in Enum.GetNames(typeof(BasicResources))) {
                _resourcesCount.Add(i, 0);
            }
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

        public Resource GetResource(BasicResources resourceType)
        {
            Resource resource = null;

            bool found = false;
            int i = 0;

            while (!found && i < BasicResources.Count)
            {
                if (BasicResources[i].GetResource() == resourceType)
                {
                    resource = BasicResources[i];
                    found = true;
                }

                i++;
            }

            if (resource == null)
                throw new Exception("No resource found");

            return resource;
        }

        public Resource GetResource(AdvanceResources resourceType)
        {
            Resource resource = null;

            bool found = false;
            int i = 0;

            while (!found && i < BasicResources.Count)
            {
                if (AdvanceResources[i].GetResource() == resourceType)
                {
                    resource = BasicResources[i];
                    found = true;
                }

                i++;
            }

            if (resource == null)
                throw new Exception("No resource found");

            return resource;
        }

        public int GetSeaweedsCount() { return _resourcesCount[Shared.BasicResources.SEAWEED.ToString()]; }

        public void AddResource(string resource, int amount) {
            if (!_resourcesCount.ContainsKey(resource))
                throw new KeyNotFoundException("No resource found");
            

            _resourcesCount[resource] += amount;
        }
    }
}
