using GGG.Components.Core;
using GGG.Components.HexagonalGrid;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using GGG.Shared;
using TMPro;
using UnityEngine.Networking;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private GameObject _viewport;
        
        private bool _open;
        
        public static Action<Generator> OnGeneratorOpen;
        public static Action OnGeneratorClose;

        private void Awake()
        {
            _viewport = transform.GetChild(0).gameObject;
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            _viewport.SetActive(false);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnEnable()
        {
            GeneratorCloseButton.OnClose += OnCloseButton;
        }

        private void OnDisable()
        {
            GeneratorCloseButton.OnClose -= OnCloseButton;
        }

        public void Open(Generator generator)
        {
            if(_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            OnGeneratorOpen?.Invoke(generator);
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
                
                OnGeneratorClose?.Invoke();
            };
        }
    }
}
