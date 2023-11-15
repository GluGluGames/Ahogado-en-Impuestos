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
        private InventoryUI _inventory;

        private bool _open;

        private void Start()
        {
            _input = InputManager.Instance;
            _inventory = FindObjectOfType<InventoryUI>();
            
            OpenButton.onClick.AddListener(ToggleMenu);
            InventoryButton.onClick.AddListener(OpenInventory);
            SettingsButton.onClick.AddListener(OpenSettings);
            ExpeditionButton.onClick.AddListener(() =>
            {
                HUDManager.Instance.ChangeScene(SceneIndexes.MINIGAME, SceneIndexes.GAME_SCENE);
                GameManager.Instance.OnUIClose();
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
                transform.DOMoveX(Screen.width * 0.85f, 0.75f).SetEase(Ease.InQuad);
                OpenButton.gameObject.transform.rotation = Quaternion.Euler(0, 0, 180);
                GameManager.Instance.OnUIOpen();
            }
            else
            {
                transform.DOMoveX(Screen.width + 5, 0.75f).SetEase(Ease.OutCubic);
                OpenButton.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
                GameManager.Instance.OnUIClose();
            }
        }

        public void ToggleOpenButton() => OpenButton.gameObject.SetActive(!OpenButton.gameObject.activeInHierarchy);

        private void OpenInventory()
        {
            ToggleMenu();
            _inventory.OpenInventory();
        }

        private void OpenSettings()
        {
            SceneManagement.Instance.OpenSettings();
            ToggleMenu();
            GameManager.Instance.OnUIOpen();
        }
    }
}
