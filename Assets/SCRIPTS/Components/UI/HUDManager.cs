using System.Collections.Generic;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Shared;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        [SerializeField] private List<TMP_Text> ResourcesText;
        [SerializeField] private List<Image> ResourcesIcons;
        
        private PlayerManager _player;
        private readonly List<Resource> _shownResource = new(3);

        private int _currentIdx = 1;
        private bool _playerInitialized;

        private void Start() {
            _player = PlayerManager.Instance;

            foreach(Image image in ResourcesIcons)
                image.gameObject.SetActive(false);
            
            foreach(TMP_Text text in ResourcesText)
                text.gameObject.SetActive(false);
            
            _player.OnPlayerInitialized += () => {
                _playerInitialized = true;
                _shownResource.Add(_player.GetResource("Seaweed"));
                
                ResourcesIcons[0].gameObject.SetActive(true);
                ResourcesIcons[0].sprite = _shownResource[0].GetSprite();
                
                ResourcesText[0].gameObject.SetActive(true);
                ResourcesText[0].SetText(_player.GetResourceCount("Seaweed").ToString());
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

        public bool ResourceBeingShown(Resource resource) => _shownResource.Find((x) => x == resource);

        public bool ShowResource(Resource resource)
        {
            if (_shownResource.Count >= 3) return false;
            
            _shownResource.Add(resource);
            ResourcesIcons[_currentIdx].gameObject.SetActive(true);
            ResourcesIcons[_currentIdx].sprite = resource.GetSprite();
            
            ResourcesText[_currentIdx].gameObject.SetActive(true);
            ResourcesText[_currentIdx].SetText(_player.GetResourceCount(resource.GetKey()).ToString());
            _currentIdx++;
            
            return true;
        }

        public bool HideResource(Resource resource)
        {
            if (_shownResource.Count <= 0) return false;
            int idx = _shownResource.FindIndex((x) => x == resource);
            
            ResourcesIcons[idx].gameObject.SetActive(false);
            ResourcesText[idx].gameObject.SetActive(false);
            _shownResource.Remove(resource);
            _currentIdx = idx;

            return true;
        }
    }
}
