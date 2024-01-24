using System;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class SettingsButton : MonoBehaviour
    {
        private Settings _settings;
        
        private void Start()
        {
            _settings = FindObjectOfType<Settings>();
        }

        public void OnSettingsButton()
        {
            _settings.Open();
        }
    }
}