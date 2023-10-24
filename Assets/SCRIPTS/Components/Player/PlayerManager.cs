using System;
using System.Collections;
using GGG.Shared;

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using GGG.Components.Core;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace GGG.Components.Player
{
    public class PlayerManager : MonoBehaviour {
        #region Singleton

        public static PlayerManager Instance;

        private void Awake() {

            if(_PlayerModel != null)
            {
                GameObject.Instantiate(_PlayerModel, transform);

            }
            if (Instance != null) return;

            Instance = this;
        }

        #endregion

        [SerializeField] private List<Resource> Resources;
        [SerializeField] private Resource TempMainResource;
        [SerializeField] private GameObject _PlayerModel;

        private Dictionary<string, int> _resourcesCount = new();
        private Dictionary<string, Resource> _resources = new();

        public Action OnPlayerInitialized;

        [Serializable]
        public class ResourceData
        {
            public string Name;
            public int Count;
        }

        private void Start()
        {
            foreach (Resource i in Resources)
                _resources.Add(i.GetKey(), i);

            foreach (string i in _resources.Keys) 
                _resourcesCount.Add(i, 0);
        }

        private void OnEnable() {
            StartCoroutine(LoadResourcesCount());
        }

        private void OnDisable() {
            SaveResourcesCount();
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

        private void SaveResourcesCount()
        {
            ResourceData[] resourceDataList = new ResourceData[_resourcesCount.Count];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "resources_data.json");
            int i = 0;
            
            foreach (var pair in _resourcesCount)
            {
                ResourceData data = new ResourceData();
                data.Name = pair.Key;
                data.Count = pair.Value;
                resourceDataList[i] = data;
                i++;
            }
            
            string jsonData = JsonHelper.ToJson(resourceDataList, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadResourcesCount()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "resources_data.json");
#if UNITY_EDITOR
            filePath = "file://" + filePath;
#endif
            string data;
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }

            if (data != "") {
                ResourceData[] resources = JsonHelper.FromJson<ResourceData>(data);
                _resourcesCount = resources.ToDictionary(item => item.Name, item => item.Count);
            }
            else {
                SaveResourcesCount();
            }

            // DEBUG - DELETE LATER
            if(_resourcesCount["Pearl"] <= 0) _resourcesCount["Pearl"] += 1;
            
            OnPlayerInitialized?.Invoke();
        }
    }
}
