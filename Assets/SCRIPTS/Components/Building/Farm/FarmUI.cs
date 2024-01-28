using GGG.Shared;
using GGG.Components.Core;

using System;
using DG.Tweening;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class FarmUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private GameObject _viewport;
        
        private bool _open;

        public static Action<Farm> OnFarmUIOpen;
        public static Action OnFarmClose;
        
        private void Awake()
        {
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            FarmCloseButton.OnClose += OnCloseButton;
        }

        private void OnDisable()
        {
            FarmCloseButton.OnClose -= OnCloseButton;
        }
        
       
        public void Open(Farm farm)
        {
            if(_open) return;
            _open = true;

            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            OnFarmUIOpen?.Invoke(farm);
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
                _gameManager.OnUIClose();
                
                OnFarmClose?.Invoke();
            };
        }
    }
}
