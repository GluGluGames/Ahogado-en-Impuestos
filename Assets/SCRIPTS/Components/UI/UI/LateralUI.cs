using System;
using DG.Tweening;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class LateralUI : MonoBehaviour
    {
        [SerializeField] private Button OpenButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button InventoryButton;
        [SerializeField] private Button ExpeditionButton;

        private InventoryUI _inventory;

        private bool _open;

        private void Start()
        {
            _inventory = FindObjectOfType<InventoryUI>();
            
            OpenButton.onClick.AddListener(ToggleMenu);
            InventoryButton.onClick.AddListener(OpenInventory);
            SettingsButton.onClick.AddListener(OpenSettings);
        }

        private void ToggleMenu()
        {
            if (GameManager.Instance.IsOnUI()) return;
            
            _open = !_open;

            if (_open) transform.DOMoveX(Screen.width * 0.5f + 660, 1.5f).SetEase(Ease.InQuad);
            else transform.DOMoveX(Screen.width * 0.5f + 970, 1.5f).SetEase(Ease.OutCubic);
        }

        private void OpenInventory()
        {
            _inventory.OpenInventory();
            ToggleMenu();
        }

        private void OpenSettings()
        {
            SceneManagement.Instance.OpenSettings();
            ToggleMenu();
            GameManager.Instance.OnUIOpen();
        }
    }
}
