using System;
using System.Collections;
using GGG.Classes.Shop;
using GGG.Components.Core;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings.Shop
{
    public class ShopUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private GameObject _viewport;
        private ShopExitButton _exitButton;

        private bool _open;

        public static Action OnShopOpen;
        public static Action OnShopClose;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _exitButton = GetComponentInChildren<ShopExitButton>();
            _exitButton.OnExit += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        public void Open()
        {
            if(_open) return;
            _open = true;

            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            
            _gameManager.OnUIOpen();
            OnShopOpen?.Invoke();
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f, true).SetEase(Ease.OutCubic).onComplete += () => { 
                _viewport.SetActive(false);
                _open = false;
            };
            
            _gameManager.OnUIClose();
            OnShopClose?.Invoke();
        }
    }
}
