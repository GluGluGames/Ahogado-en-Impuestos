using System;
using GGG.Components.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class SettingsCloseButton : MonoBehaviour
    {
        private Settings _settings;

        private void OnEnable()
        {
            if (!_settings) _settings = FindObjectOfType<Settings>();
        }

        public void OnCloseButton()
        {
            if(GameManager.Instance.IsOnUI()) GameManager.Instance.OnUIClose();
            _settings.Close();

        }
    }
}
