using System;
using System.Collections;
using GGG.Shared;

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using GGG.Components.Core;
using UnityEngine.Networking;

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
        
        private GameManager _gameManager;
        
        private static List<Resource> _resources;
        private static Dictionary<string, int> _resourcesCount = new();
        private static readonly Dictionary<string, Resource> _resourcesDictionary = new();

        public Action OnPlayerInitialized;

        [Serializable]
        public class ResourceData
        {
            public string Name;
            public int Count;
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            
            _resources = Resources.LoadAll<Resource>("SeaResources").
                Concat(Resources.LoadAll<Resource>("ExpeditionResources")).
                Concat(Resources.LoadAll<Resource>("FishResources")).ToList();

            if (_resourcesDictionary.Count <= 0)
            {
                foreach (Resource i in _resources)
                    _resourcesDictionary.Add(i.GetKey(), i);
            }

            if (_resourcesCount.Count > 0) return;
            
            foreach (string i in _resourcesDictionary.Keys)
                _resourcesCount.Add(i, 0);
        }

        public int GetResourceCount(string key) => _resourcesCount[key];

        public List<Resource> GetResources() => _resources;

        public Resource GetResource(string resourceKey) => 
            _resourcesDictionary.TryGetValue(resourceKey, out Resource value) ? value : null;
        
        public void AddResource(string resourceKey, int amount) {
            if (!_resourcesCount.ContainsKey(resourceKey))
                throw new KeyNotFoundException("No resource found");
            

            _resourcesCount[resourceKey] += amount;
            SaveResourcesCount();
        }

        public int GetResourceNumber() => _resourcesDictionary.Count;

        public void SaveResourcesCount()
        {
            ResourceData[] resourceDataList = new ResourceData[_resourcesCount.Count];
            string filePath = Path.Combine(Application.persistentDataPath, "resources_data.json");
            int i = 0;
            
            foreach (var pair in _resourcesCount)
            {
                ResourceData data = new()
                {
                    Name = pair.Key,
                    Count = pair.Value
                };
                
                resourceDataList[i] = data;
                i++;
            }
            
            string jsonData = JsonHelper.ToJson(resourceDataList, true);
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadResourcesCount()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "resources_data.json");
            string data;

            if (!File.Exists(filePath))
            {
                if (_resourcesCount.Count <= 0)
                    foreach (string i in _resourcesDictionary.Keys) _resourcesCount.Add(i, 0);
                
                OnPlayerInitialized?.Invoke();
                yield break;
            }
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = File.ReadAllText(filePath);
            
            ResourceData[] resources = JsonHelper.FromJson<ResourceData>(data); 
            _resourcesCount = resources.ToDictionary(item => item.Name, item => item.Count);

            if (_resourcesCount.Count <= 0) {
                foreach (string i in _resourcesDictionary.Keys) 
                    _resourcesCount.Add(i, 0);
            }
            
            OnPlayerInitialized?.Invoke();
        }
    }
}
