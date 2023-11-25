using System;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Input;
using UnityEngine;
using UnityEngine.UI;
using GGG.Shared;

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
        private Vector3 _initialPosition;
        private bool _open;

        private void Start()
        {
            _input = InputManager.Instance;
            _sceneManagement = SceneManagement.Instance;
            _gameManager = GameManager.Instance;
            _inventory = FindObjectOfType<InventoryUI>();
            _viewport = transform.GetChild(0).gameObject;
            _initialPosition = transform.position;
            
            OpenButton.onClick.AddListener(ToggleMenu);
            InventoryButton.onClick.AddListener(OpenInventory);
            SettingsButton.onClick.AddListener(OpenSettings);
            ExpeditionButton.onClick.AddListener(() =>
            {
                _sceneManagement.AddSceneToLoad(SceneIndexes.MINIGAME_LEVEL1);
                _sceneManagement.AddSceneToUnload(SceneIndexes.GAME_SCENE);
                _sceneManagement.UpdateScenes();
                _gameManager.OnUIClose();
            });
        }

        private void Update() {
            if (!_input.Escape()) return;
            
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if ((GameManager.Instance.IsOnUI() || GameManager.Instance.OnTutorial()) && !_open) return;
            
            _open = !_open;

            if (_open)
            {
                _viewport.transform.DOMoveX(Screen.width * 0.4f, 0.75f).SetEase(Ease.InCubic);
                OpenButton.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
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
            ToggleMenu();
            _inventory.OpenInventory();
        }

        private void OpenSettings()
        {
            _sceneManagement.OpenSettings();
            ToggleMenu();
            _gameManager.OnUIOpen();
        }
    }
}
