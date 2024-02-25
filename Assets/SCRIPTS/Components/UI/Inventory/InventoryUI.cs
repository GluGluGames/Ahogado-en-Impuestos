using System;
using GGG.Components.Player;

using UnityEngine;
using DG.Tweening;
using GGG.Components.Core;
using GGG.Input;

namespace GGG.Components.UI.Inventory
{
    public class InventoryUI : MonoBehaviour
    {
        #region Public variables

        public static Action OnInventoryOpen;
        public static Action OnInventoryClose;

        #endregion

        #region Private variables
        
        private InputManager _input;
        private GameManager _gameManager;
        private InventoryCloseButton _closeButton;
            
        private GameObject _viewport;
        private bool _open;

        #endregion

        #region Unity functions

        private void Awake()
        {
            _closeButton = GetComponentInChildren<InventoryCloseButton>();
            _closeButton.OnCloseButton += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
        }

        private void Update()
        {
            if (!_open) return;
            
            if (!_input.Escape() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void OnDisable()
        {
            _closeButton.OnCloseButton -= OnCloseButton;
        }

        #endregion

        #region Methods
        
        public void Open()
        {
            if (_open) return;
            _open = true;
            _gameManager.OnUIOpen();
            
            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.5f).SetEase(Ease.InCubic);
            
            OnInventoryOpen?.Invoke();
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.5f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                _open = false;
            };
            
            OnInventoryClose?.Invoke();
        }

        #endregion
    }
}
