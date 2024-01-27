using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Shared;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GGG.Components.Achievements;
using GGG.Components.UI.Buttons;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GGG.Components.Buildings.Laboratory
{
    public class LaboratoryUI : MonoBehaviour
    {
        private GameManager _gameManager;
        private LaboratoryExitButton _exitButton;
        private GameObject _viewport;
        
        private bool _open;

        public static Action<Laboratory> OnLaboratoryOpen;
        public static Action OnLaboratoryClose;
        
        private void Awake()
        {
            _exitButton = GetComponentInChildren<LaboratoryExitButton>();
            _exitButton.OnExit += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        private void OnDisable()
        {
            _exitButton.OnExit -= OnCloseButton;
        }

        public void Open(Laboratory laboratory)
        {
            if (_open) return;
            _open = true;
            _gameManager.OnUIOpen();

            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            
            OnLaboratoryOpen?.Invoke(laboratory);
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
                OnLaboratoryClose?.Invoke();
            };
        }
    }
}
