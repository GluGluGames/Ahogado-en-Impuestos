using GGG.Components.Player;
using GGG.Shared;

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GGG.Components.Core;
using UnityEngine.Networking;

namespace GGG.Components.UI
{
    public class HUDManager : MonoBehaviour {
        #region Singleton

        public static HUDManager Instance;
        
        private void Awake() {
            if (Instance != null) return;

            Instance = this;
        }

        #endregion

        [SerializeField] private List<GameObject> ResourceContainers;
        [SerializeField] private List<TMP_Text> ResourcesText;
        [SerializeField] private List<Image> ResourcesIcons;

        [Serializable]
        private class ShownResource
        {
            public Resource Resource;
            public int Index;
        }
        
        private PlayerManager _player;
        private GameManager _gameManager;
        private readonly List<Resource> _shownResource = new(2);
        private bool _initialized;
        private bool _resourcesLoad;

        private int _currentIdx;
        private bool _dirtyFlag;

        private void Start() {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;

            foreach(GameObject go in ResourceContainers)
                go.SetActive(false);

            _player.OnPlayerInitialized += () => _initialized = true;
        }

        private void Update()
        {
            if (!_resourcesLoad || _gameManager.GetCurrentTutorial() is Tutorials.InitialTutorial or Tutorials.BuildTutorial) return;
            
            ResourcesText[0].SetText(_player.GetResourceCount("Seaweed").ToString());
            
            for (int i = 0; i < _shownResource.Count; i++)
            {
                if(!_shownResource[i]) continue;
                
                ResourcesText[i + 1].SetText(_player.GetResourceCount(_shownResource[i].GetKey()).ToString());
            }
        }

        private void OnDisable()
        {
            SaveShownResources();
        }

        public bool ResourceBeingShown(Resource resource) => _shownResource.Find((x) => x == resource);

        public bool ShowResource(Resource resource)
        {
            if (_shownResource.Count >= 2) return false;
            
            _shownResource.Insert(_shownResource.Count <= 0 ? 0 : _currentIdx - 1, resource);
            ResourceContainers[_currentIdx].gameObject.SetActive(true);
            
            ResourcesIcons[_currentIdx].sprite = resource.GetSprite();
            ResourcesText[_currentIdx].SetText(_player.GetResourceCount(resource.GetKey()).ToString());
            
            if (_currentIdx + 1 >= ResourceContainers.Count) return true;
            _currentIdx += ResourceContainers[_currentIdx + 1].gameObject.activeInHierarchy ? 2 : 1;
            
            return true;
        }

        public bool HideResource(Resource resource)
        {
            if (_shownResource.Count <= 0) return false;
            
            int idx = ResourcesIcons.FindIndex(x => x.sprite == resource.GetSprite());

            print(resource.GetKey());
            if (idx == -1) throw new Exception("-1 in idx");
            
            ResourceContainers[idx].gameObject.SetActive(false);
            ResourcesIcons[idx].sprite = null;
            
            _shownResource.Remove(resource);
            _currentIdx = _shownResource.Count == 0 ? 1 : ResourceContainers.FindIndex(x => !x.activeInHierarchy);
            print(_currentIdx);

            return true;
        }

        public void SaveShownResources()
        {
            if (!SceneManagement.InGameScene() || _gameManager.OnTutorial()) return;
            
            ShownResource[] resourcesData = new ShownResource[_shownResource.Count];
            int i = 0;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "shown_resources.json");

            foreach (Resource resource in _shownResource)
            {
                ShownResource resourceData = new()
                {
                    Resource = resource,
                    Index = ResourcesIcons.FindIndex(x => x.sprite == resource.GetSprite())
                };

                resourcesData[i] = resourceData;
                i++;
            }

            string jsonData = JsonHelper.ToJson(resourcesData);
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadShownResource()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "shown_resources.json");
            string data;

            if (!File.Exists(filePath))
            {
                ResourceContainers[0].gameObject.SetActive(true);
                
                ResourcesIcons[0].sprite = _player.GetResource("Seaweed").GetSprite();
                ResourcesText[0].SetText(_player.GetResourceCount("Seaweed").ToString());

                _currentIdx = 1;
                _initialized = true;
                
                yield break;
            }
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }

            yield return new WaitWhile(() => !_initialized);

            ResourceContainers[0].gameObject.SetActive(true);
                
            ResourcesIcons[0].sprite = _player.GetResource("Seaweed").GetSprite();
            ResourcesText[0].SetText(_player.GetResourceCount("Seaweed").ToString());
            
            ShownResource[] resources = JsonHelper.FromJson<ShownResource>(data);
            
            foreach (ShownResource resource in resources)
            {
                _shownResource.Add(resource.Resource);

                ResourceContainers[resource.Index].SetActive(true);
                
                ResourcesIcons[resource.Index].sprite = resource.Resource.GetSprite();
                ResourcesText[resource.Index].SetText(_player.GetResourceCount(resource.Resource.GetKey()).ToString());
                _currentIdx = resource.Index;
            }
            
            _currentIdx = _shownResource.Count == 0 ? 0 : ResourcesIcons.FindIndex(x => !x.gameObject.activeInHierarchy);
            _resourcesLoad = true;
        }
    }
}
