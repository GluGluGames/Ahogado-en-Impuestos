using System;
using DG.Tweening;
using GGG.Components.Core;
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

        private InventoryUI _inventory;

        private bool _open;

        private void Start()
        {
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

        private void ToggleMenu()
        {
            if (GameManager.Instance.IsOnUI() && !_open) return;
            
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
