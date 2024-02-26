using System;
using GGG.Components.Core;
using GGG.Components.Menus;
using UnityEngine;

namespace GGG.Components.UI.Lateral
{
    public class LateralSettingsButton : MonoBehaviour
    {
        public Action OnSettingsButton;
        private Settings _settings;
        
        
        private void Awake()
        {
            _settings = FindObjectOfType<Settings>();
        }

        public void OpenSettings()
        {
            if (GameManager.Instance.OnTutorial()) return;
            
            _settings.Open();
            OnSettingsButton?.Invoke();
            GameManager.Instance.OnUIOpen();
        }
    }
}
