using System.Collections.Generic;
using System.Linq;
using GGG.Components.Core;
using GGG.Components.Pickables;
using GGG.Components.UI.Buttons;
using UnityEngine.Localization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GGG.Shared;
using GGG.Components.Player;
using GGG.Components.Scenes;
using UnityEngine.SceneManagement;

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
        [SerializeField] private Sound VictorySound;
        [SerializeField] private Sound DefeatSound;

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
                Scene currentScene = SceneManager.GetSceneAt(1);
                SceneIndexes currentSceneIndex = SceneIndexes.MINIGAME_LEVEL1;

                if (currentScene.name == "Minigame_Level1")
                {
                    currentSceneIndex = SceneIndexes.MINIGAME_LEVEL1;
                }
                else if (currentScene.name == "Minigame_Level2")
                {
                    currentSceneIndex = SceneIndexes.MINIGAME_LEVEL2;
                }
                else if (currentScene.name == "Minigame_Level3")
                {
                    currentSceneIndex = SceneIndexes.MINIGAME_LEVEL3;
                }
                else if (currentScene.name == "Minigame_Level4")
                {
                    currentSceneIndex = SceneIndexes.MINIGAME_LEVEL4;
                }

                _sceneManagement.LoadScene(SceneIndexes.MAIN_MENU, currentSceneIndex);
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
                _currentResourceIndex++;
            }
        }

        public void OnEndGame(bool isWin)
        {
            Viewport.gameObject.SetActive(true);
            OutcomeText.text = OutcomeString[isWin ? 0 : 1].GetLocalizedString();
            SoundManager.Instance.Play(isWin ? VictorySound : DefeatSound);
            GameManager.Instance.OnUIOpen();

            foreach (Resource resource in _resourceManager.resourcesCollected.Keys)
            {
                if (_resourceManager.resourcesCollected[resource] == 0) continue;
                int amount = _resourceManager.resourcesCollected[resource];
                _recollectedResources.Add(new RecollectedResource(resource, amount));
                PlayerManager.Instance.AddResource(resource.GetKey(), amount);
            }
            
            if (_recollectedResources.Count == 0) _pagesNumber = 1;
            else _pagesNumber = _recollectedResources.Count < 4 ? 1 : Mathf.CeilToInt(_recollectedResources.Count * 0.25f);
            PagesText.SetText($"{_currentPage}/{_pagesNumber}");
            
            SetButtons();
        }
    }
}