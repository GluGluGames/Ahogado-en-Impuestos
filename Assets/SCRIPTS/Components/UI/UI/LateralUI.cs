using System;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Input;
using UnityEngine;
using UnityEngine.UI;
using GGG.Components.Achievements;
using GGG.Components.HexagonalGrid;
using GGG.Shared;
using Random = UnityEngine.Random;

namespace GGG.Components.UI
{
    public class LateralUI : MonoBehaviour
    {
        [SerializeField] private Button OpenButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button InventoryButton;
        [SerializeField] private Button ExpeditionButton;

        private InputManager _input;
        private GameManager _gameManager;
        private SceneManagement _sceneManagement;
        private InventoryUI _inventory;

        private GameObject _viewport;
        private bool _open;

        public static Action OnLateralUiOpen;

        private void Start()
        {
            _input = InputManager.Instance;
            _sceneManagement = SceneManagement.Instance;
            _gameManager = GameManager.Instance;
            _inventory = FindObjectOfType<InventoryUI>();
            _viewport = transform.GetChild(0).gameObject;
            
            OpenButton.onClick.AddListener(ToggleMenu);
            InventoryButton.onClick.AddListener(OpenInventory);
            SettingsButton.onClick.AddListener(OpenSettings);
            ExpeditionButton.onClick.AddListener(OnExpeditionButton);
        }

        private void Update() {
            if (!_input.Escape() || _gameManager.OnTutorial()) return;
            
            ToggleMenu();
        }

        private void OnDisable()
        {
            OpenButton.onClick.RemoveAllListeners();
            InventoryButton.onClick.RemoveAllListeners();
            SettingsButton.onClick.RemoveAllListeners();
            ExpeditionButton.onClick.RemoveAllListeners();
        }

        private void OnExpeditionButton()
        {
            if (_gameManager.OnTutorial()) return;
            
            TileManager.Instance.SaveTilesState();
            
            int randMiniGame = Random.Range(1, 5);
            SceneIndexes sceneIndex;
            
            switch(randMiniGame)
            {
                case 1:
                    sceneIndex = SceneIndexes.MINIGAME_LEVEL1;
                    break;
                case 2:
                    sceneIndex = SceneIndexes.MINIGAME_LEVEL2;
                    break;
                case 3:
                    sceneIndex = SceneIndexes.MINIGAME_LEVEL3;
                    break;
                case 4:
                    sceneIndex = SceneIndexes.MINIGAME_LEVEL4;
                    break;
                default:
                    sceneIndex = SceneIndexes.MINIGAME_LEVEL1;
                    break;
            }
            
            _sceneManagement.AddSceneToLoad(sceneIndex);
            _sceneManagement.AddSceneToUnload(SceneIndexes.GAME_SCENE);
            _sceneManagement.UpdateScenes();

            int x = PlayerPrefs.HasKey("Achievement09") ? PlayerPrefs.GetInt("Achievement09") + 1 : 1;
            PlayerPrefs.SetInt("Achievement09", x);

            if (PlayerPrefs.GetInt("Achievement09") >= 5)
                StartCoroutine(AchievementsManager.Instance.UnlockAchievement("09"));
            
            _gameManager.OnUIClose();
        }

        private void ToggleMenu()
        {
            if ((GameManager.Instance.IsOnUI() || GameManager.Instance.OnTutorial()) && !_open) return;
            
            _open = !_open;

            if (_open)
            {
                _viewport.transform.DOMoveX(Screen.width * 0.4f, 0.75f).SetEase(Ease.InCubic);
                OpenButton.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                OnLateralUiOpen?.Invoke();
                _gameManager.OnUIOpen();
            }
            else
            {
                _viewport.transform.DOMoveX(Screen.width * 0.58f, 0.75f).SetEase(Ease.OutCubic);
                OpenButton.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
               _gameManager.OnUIClose();
            }
        }

        public void ToggleOpenButton() => OpenButton.transform.parent.gameObject.SetActive(
            !OpenButton.transform.parent.gameObject.activeInHierarchy);

        private void OpenInventory()
        {
            if (_gameManager.OnTutorial()) return;
            
            ToggleMenu();
            _inventory.OpenInventory();
        }

        private void OpenSettings()
        {
            if (_gameManager.OnTutorial()) return;
            
            _sceneManagement.OpenSettings();
            ToggleMenu();
            _gameManager.OnUIOpen();
        }
    }
}
