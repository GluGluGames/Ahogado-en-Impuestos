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

        [SerializeField] private TMP_Text SeaweedText;
        [SerializeField] private List<GameObject> ResourceContainers;
        [SerializeField] private List<TMP_Text> ResourcesText;
        [SerializeField] private List<Image> ResourcesIcons;
        
        private PlayerManager _player;
        private GameManager _gameManager;
        private readonly List<Resource> _shownResource = new(2);

        private int _currentIdx;
        private bool _dirtyFlag;

        private void Start() {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;

            foreach(GameObject go in ResourceContainers)
                go.SetActive(false);
        }

        private void Update()
        {
            if (!_player.GetResource("Seaweed")) return;
            if (_gameManager.GetCurrentTutorial() is Tutorials.InitialTutorial or Tutorials.BuildTutorial) return;
            
            SeaweedText.SetText(_player.GetResourceCount("Seaweed").ToString());
            
            for (int i = 0; i < _shownResource.Count; i++)
            {
                if(!_shownResource[i]) continue;
                
                ResourcesText[i].SetText(_player.GetResourceCount(_shownResource[i].GetKey()).ToString());
            }
        }

        public List<Resource> ShownResources() => _shownResource;
        public int GetIndex(Resource resource) => ResourcesIcons.FindIndex(x => x.sprite == resource.GetSprite());
        public void SetCurrentIndex(int idx) => _currentIdx = idx;
        public List<Image> GetResourceIcons() => ResourcesIcons;
        public void AddShownResource(Resource resource) => _shownResource.Add(resource);
        public void ShowResource(int idx) => ResourceContainers[idx].SetActive(true);
        public void SetResourceIcon(int idx, Sprite sprite) => ResourcesIcons[idx].sprite = sprite;
        public void SetResourceText(int idx, string text) => ResourcesText[idx].SetText(text);

        public bool ResourceBeingShown(Resource resource) => _shownResource.Find((x) => x == resource);

        public bool ShowResource(Resource resource)
        {
            if (_shownResource.Count >= 2) return false;
            
            _shownResource.Insert(_currentIdx, resource);
            ResourceContainers[_currentIdx].gameObject.SetActive(true);
            
            ResourcesIcons[_currentIdx].sprite = resource.GetSprite();
            ResourcesText[_currentIdx].SetText(_player.GetResourceCount(resource.GetKey()).ToString());
            
            if (_currentIdx + 1 >= ResourceContainers.Count) return true;
            _currentIdx += ResourceContainers[_currentIdx + 1].gameObject.activeInHierarchy ? 0 : 1;
            
            return true;
        }

        public bool HideResource(Resource resource)
        {
            if (_shownResource.Count <= 0) return false;
            
            int idx = ResourcesIcons.FindIndex(x => x.sprite == resource.GetSprite());
            
            ResourceContainers[idx].gameObject.SetActive(false);
            ResourcesIcons[idx].sprite = null;
            
            _shownResource.Remove(resource);
            _currentIdx = _shownResource.Count == 0 ? 0 : ResourceContainers.FindIndex(x => !x.activeInHierarchy);

            return true;
        }
    }
}
