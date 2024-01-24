using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using GGG.Classes.Buildings;
using GGG.Components.UI.Buttons;
using Project.Component.UI.Containers;

namespace GGG.Components.Buildings.Museum
{
    public class MuseumUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private List<ContainerButton> _containerButtons;
        private MuseumExitButton _exitButton;
        private GameObject _viewport;
        
        private bool _open;

        public static Action OnMuseumOpen;
        public static Action OnMuseumClose;
        
        #region Unity functions

        private void Awake()
        {
            _exitButton = GetComponentInChildren<MuseumExitButton>();
            _exitButton.OnExit += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        #endregion

        public void Open()
        {
            if (_open) return;
            
            _open = true;
            _viewport.SetActive(true);
            
            OnMuseumOpen?.Invoke();
            _gameManager.OnUIOpen();
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                OnMuseumClose?.Invoke();
            };
            
        }
    }
}
