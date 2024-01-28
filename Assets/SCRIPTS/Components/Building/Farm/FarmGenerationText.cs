using System;
using GGG.Shared;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings
{
    [RequireComponent(typeof(TMP_Text), typeof(CanvasGroup))]
    public class FarmGenerationText : MonoBehaviour
    {
        private TMP_Text _text;
        private CanvasGroup _canvasGroup;
        private Farm _farm;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            FarmUI.OnFarmUIOpen += OnOpen;
            FarmUI.OnFarmClose += OnClose;
            FarmListener.OnResourceSelected += Show;
        }

        private void OnDisable()
        {
            FarmListener.OnResourceSelected -= Show;
            FarmUI.OnFarmClose -= OnClose;
        }

        private void OnOpen(Farm farm)
        {
            _farm = farm;
            
            if (farm.GetResource()) Show(farm.GetResource());
            else Hide();
        }

        private void OnClose()
        {
            Hide();

            _farm = null;
        }

        private void SetText(float amount)
        {
            _text.SetText($"{1/amount:0.00}/s");
        }

        private void Hide() => _canvasGroup.alpha = 0;
        private void Show(Resource resource)
        {
            _canvasGroup.alpha = 1;
            SetText(_farm.GetGeneration());
        }
    }
}
