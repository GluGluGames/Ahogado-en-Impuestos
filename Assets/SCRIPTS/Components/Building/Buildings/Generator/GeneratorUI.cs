using GGG.Components.Core;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace GGG.Components.Generator
{
    public class GeneratorUI : MonoBehaviour
    {
        [SerializeField] private Button CloseButton;
        
        public static Action OnGeneratorOpen;

        private GameManager _gameManager;
        
        private GameObject _viewport;
        private bool _open;

        private void Awake()
        {
            CloseButton.onClick.AddListener(Close);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        public void Open()
        {
            if(_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            OnGeneratorOpen?.Invoke();

            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            if (!_open) return;

            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
            };
            
            _gameManager.OnUIClose();
        }
    }
}
