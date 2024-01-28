using System;
using GGG.Classes.Buildings;
using TMPro;
using UnityEngine;

namespace GGG.Components.Buildings.Generator
{
    [RequireComponent(typeof(TMP_Text))]
    public class GeneratorBoostText : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void OnDisable()
        {
            SetText("");
        }

        public void InitializeText(Building building)
        {
            SetText(building.GetName());
        }

        private void SetText(string text) => _text.SetText(text);
    }
}
