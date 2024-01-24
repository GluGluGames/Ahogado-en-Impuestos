using GGG.Components.Core;

using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GGG.Components.Buildings.CityHall
{
    public class CityHallUi : MonoBehaviour
    {
        private GameManager _gameManager;
        private CityHallExitButton _exitButton;
        private GameObject _viewport;
        
        private bool _open;

        public static Action OnCityHallOpen;
        public static Action OnCityHallClose;

        private void Awake()
        {
            _exitButton = GetComponentInChildren<CityHallExitButton>();
            _exitButton.OnExit += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen()) return;

            Close();
        }

        public void Open()
        {
            if (_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            OnCityHallOpen?.Invoke();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _viewport.SetActive(false);
                _open = false;
                _gameManager.OnUIClose();
                OnCityHallClose?.Invoke();
            };
        }
    }
}
