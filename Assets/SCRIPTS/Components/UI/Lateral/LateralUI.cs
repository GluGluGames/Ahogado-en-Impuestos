using System;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Input;
using UnityEngine;
using GGG.Components.UI.Inventory;
using GGG.Components.Scenes;

namespace GGG.Components.UI.Lateral
{
    public class LateralUI : MonoBehaviour
    {
        private LateralCloseButton _openButton;
        private LateralExpeditionButton _expeditionButton;
        private LateralInventoryButton _inventoryButton;
        private LateralSettingsButton _settingsButton;
        
        private InputManager _input;
        private GameManager _gameManager;

        private GameObject _viewport;
        private bool _open;

        public static Action OnLateralUiOpen;

        private void Awake()
        {
            _openButton = GetComponentInChildren<LateralCloseButton>();
            _openButton.OnLateralUI += ToggleMenu;
            
            _inventoryButton = GetComponentInChildren<LateralInventoryButton>();
            _inventoryButton.OnInventoryButton += ToggleMenu;
            
            _expeditionButton = GetComponentInChildren<LateralExpeditionButton>();
            _expeditionButton.OnExpedition += ToggleMenu;

            _settingsButton = GetComponentInChildren<LateralSettingsButton>();
            _settingsButton.OnSettingsButton += ToggleMenu;
            
            _viewport = transform.GetChild(0).gameObject;
        }

        private void Start()
        {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
        }

        private void Update() {
            if (!_input.Escape() || _gameManager.OnTutorial()) return;
            
            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if ((GameManager.Instance.IsOnUI() || GameManager.Instance.OnTutorial()) && !_open) return;
            
            _open = !_open;
            
            _openButton.ChangeRotation(Quaternion.Euler(0, 0, _open ? 180 : 0));

            if (_open)
            {
                _viewport.transform.DOMoveX(Screen.width * 0.4f, 0.75f).SetEase(Ease.InCubic);
                OnLateralUiOpen?.Invoke();
                _gameManager.OnUIOpen();
            }
            else
            {
                _viewport.transform.DOMoveX(Screen.width * 0.58f, 0.75f).SetEase(Ease.OutCubic);
               _gameManager.OnUIClose();
            }
        }

        public void ToggleOpenButton() => _openButton.transform.parent.gameObject.SetActive(
            !_openButton.transform.parent.gameObject.activeInHierarchy);
    }
}
