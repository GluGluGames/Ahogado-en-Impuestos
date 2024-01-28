using System;
using GGG.Shared;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings
{
    public class FarmHeader : MonoBehaviour
    {
        [SerializeField] private TMP_Text Text;
        [SerializeField] private CanvasGroup CanvasGroup;

        private void OnEnable()
        {
            FarmUI.OnFarmUIOpen += OnOpen;
            FarmUI.OnFarmClose += OnClose;

            FarmListener.OnResourceSelected += OnResourceSelected;
        }

        private void OnDisable()
        {
            FarmUI.OnFarmClose -= OnClose;
            FarmListener.OnResourceSelected -= OnResourceSelected;
        }

        private void OnOpen(Farm farm)
        {
            if (farm.GetResource()) OnResourceSelected(farm.GetResource());
            else Hide();
        }

        private void OnClose()
        {
            Hide();
        }

        private void OnResourceSelected(Resource resource)
        {
            Show();
            Text.SetText(resource.GetName());
        }

        private void Hide() => CanvasGroup.alpha = 0;
        private void Show() => CanvasGroup.alpha = 1;
    }
}
