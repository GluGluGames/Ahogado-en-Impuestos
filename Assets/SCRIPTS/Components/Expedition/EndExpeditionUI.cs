using System.Collections.Generic;
using System.Linq;
using GGG.Components.Core;
using GGG.Components.Resources;
using GGG.Components.UI.Buttons;
using UnityEngine.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GGG.Shared;
using GGG.Components.Player;
using System.Linq.Expressions;

namespace GGG.Components.UI
{
    public class EndExpeditionUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI OutcomeText;
        [SerializeField] private LocalizedString[] OutcomeString;
        [SerializeField] private TMP_Text PagesText;
        [SerializeField] private Button LeftArrow;
        [SerializeField] private Button RightArrow;
        [SerializeField] private GameObject Viewport;
        [SerializeField] private Button Button;

        private SceneManagement _sceneManagement;
        private ResourceManager _resourceManager;
        private List<ResourceButton> _outcomeResources = new();
        private readonly List<RecollectedResource> _recollectedResources = new();
        
        private int _pagesNumber;
        private int _currentPage = 1;
        private int _currentResourceIndex;

        private class RecollectedResource
        {
            public Resource Resource;
            public int Amount;

            public RecollectedResource(Resource resource, int amount)
            {
                Resource = resource;
                Amount = amount;
            }
        }

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;
            _resourceManager = ResourceManager.Instance;
            _outcomeResources = GetComponentsInChildren<ResourceButton>(true).ToList();
            
            foreach(ResourceButton button in _outcomeResources)
                button.gameObject.SetActive(false);
            
            Button.onClick.AddListener(() =>
            {
                _sceneManagement.AddSceneToLoad(SceneIndexes.GAME_SCENE);
                _sceneManagement.AddSceneToUnload(SceneIndexes.MINIGAME_LEVEL1);
                _sceneManagement.UpdateScenes();
                GameManager.Instance.OnUIClose();
            });
            
            RightArrow.onClick.AddListener(() => ChangePage(1));
            LeftArrow.onClick.AddListener(() => ChangePage(-1));
        }

        private void ChangePage(int direction)
        {
            if (_currentPage + direction <= 0 || _currentPage + direction > _pagesNumber) return;

            _currentResourceIndex = (_currentPage - 1 + direction) * 4;
            _currentPage += direction;
            SetButtons();
        }

        private void SetButtons()
        {
            foreach (ResourceButton button in _outcomeResources)
            {
                if (_currentResourceIndex >= _recollectedResources.Count)
                {
                    button.gameObject.SetActive(false);
                    continue;
                }
                

                RecollectedResource resource = _recollectedResources[_currentResourceIndex];
                button.SetResourceAmount(resource.Amount);
                button.SetIcon(resource.Resource.GetSprite());
                button.gameObject.SetActive(true);
                PagesText.SetText($"{_currentPage}/{_pagesNumber}");
                _currentResourceIndex++;
            }
        }

        public void OnEndGame(bool isWin)
        {
            Viewport.gameObject.SetActive(true);
            OutcomeText.text = OutcomeString[isWin ? 0 : 1].GetLocalizedString();
            GameManager.Instance.OnUIOpen();

            foreach (Resource resource in _resourceManager.resourcesCollected.Keys)
            {
                if (_resourceManager.resourcesCollected[resource] == 0) continue;
                int amount = _resourceManager.resourcesCollected[resource];
                _recollectedResources.Add(new RecollectedResource(resource, amount));
                PlayerManager.Instance.AddResource(resource.GetKey(), amount);
            }

            _pagesNumber = _recollectedResources.Count <= 4 ? 1 : _recollectedResources.Count / 4 + 1;
            SetButtons();
        }
    }
}