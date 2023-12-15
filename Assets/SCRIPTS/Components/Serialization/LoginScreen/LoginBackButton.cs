using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Serialization.Login
{
    public class LoginBackButton : MonoBehaviour
    {
        [SerializeField] private Button BackButton;
        [SerializeField] private GameObject CurrentPanel;
        [SerializeField] private GameObject LastPanel;

        public Action OnButtonClick;

        private void OnValidate()
        {
            BackButton = GetComponent<Button>();
        }

        private void OnEnable()
        {
            if (!BackButton) BackButton = GetComponent<Button>();
            
            BackButton.onClick.AddListener(OnClick);
        }

        public void SetLastPanel(GameObject panel) => LastPanel = panel;

        public void OnClick()
        {
            CurrentPanel.SetActive(false);
            LastPanel.SetActive(true);

            OnButtonClick?.Invoke();
        }
    }
}
