using System;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Buildings
{
    public class FarmSelectedResource : MonoBehaviour
    {
        [SerializeField] private Image Icon;
        [SerializeField] private CanvasGroup Group;

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
            if (farm.GetResource())
            {
                Show();
                Icon.sprite = farm.GetResource().GetSprite();
            }
            else Hide();
        }

        private void OnClose()
        {
            Hide();
        }

        private void OnResourceSelected(Resource resource)
        {
            Icon.sprite = resource.GetSprite();
        }

        private void Hide() => Group.alpha = 0;
        private void Show() => Group.alpha = 1;
    }
}
