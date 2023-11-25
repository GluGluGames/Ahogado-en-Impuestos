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
        private readonly List<Resource> _shownResource = new(3);

        private int _currentIdx;
        private bool _playerInitialized;
        private bool _dirtyFlag;

        private void Start() {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;

            foreach(GameObject go in ResourceContainers)
                go.SetActive(false);
            
            _player.OnPlayerInitialized += () =>
            {
                StartCoroutine(LoadShownResource());
                _playerInitialized = true;
            };
        }

        private void Update() {
            if (!_playerInitialized) return;
            
            for (int i = 0; i < _shownResource.Count; i++)
            {
                if(!_shownResource[i]) continue;
                
                ResourcesText[i].SetText(_player.GetResourceCount(_shownResource[i].GetKey()).ToString());
            }
        }

        private void OnDisable()
        {
            if (!SceneManagement.InGameScene() || _gameManager.OnTutorial()) return;
            
            SaveShownResources();
        }

        public bool ResourceBeingShown(Resource resource) => _shownResource.Find((x) => x == resource);

        public bool ShowResource(Resource resource)
        {
            if (_shownResource.Count >= 3) return false;
            
            _shownResource.Insert(_shownResource.Count <= 0 ? 0 : _currentIdx, resource);
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
            int idx = ResourceContainers.FindIndex(x => x.gameObject.activeInHierarchy
            && x.GetComponentInChildren<Image>().sprite == resource.GetSprite());
            
            ResourceContainers[idx].gameObject.SetActive(false);
            
            _shownResource.Remove(resource);
            _currentIdx = _shownResource.Count == 0 ? 0 : ResourceContainers.FindIndex(x => !x.gameObject.activeInHierarchy);

            return true;
        }

        private void SaveShownResources()
        {
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

        private IEnumerator LoadShownResource()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "shown_resources.json");
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

            if (!string.IsNullOrEmpty(data))
            {
                ShownResource[] resources = JsonHelper.FromJson<ShownResource>(data);

                foreach (ShownResource resource in resources)
                {
                    _shownResource.Add(resource.Resource);

                    ResourceContainers[resource.Index].SetActive(true);
                    
                    ResourcesIcons[resource.Index].sprite = resource.Resource.GetSprite();
                    ResourcesText[resource.Index].SetText(_player.GetResourceCount(resource.Resource.GetKey()).ToString());
                    _currentIdx = resource.Index;
                }
            }
            else
            {
                _shownResource.Add(_player.GetResource("Seaweed"));
                
                ResourceContainers[0].gameObject.SetActive(true);
                
                ResourcesIcons[0].sprite = _shownResource[0].GetSprite();
                ResourcesText[0].SetText(_player.GetResourceCount("Seaweed").ToString());
            }
            
            _currentIdx = _shownResource.Count == 0 ? 0 : ResourcesIcons.FindIndex(x => !x.gameObject.activeInHierarchy);
            //_playerInitialized = true;
        }
    }
}
