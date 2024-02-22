using System;
using TMPro;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    [RequireComponent(typeof(TMP_Text))]
    public class BuildName : MonoBehaviour
    {
        [SerializeField] private Color Color;
        
        private TMP_Text _text;
        private BuildButton _button;
        
        private void OnEnable()
        {
            if (!_text) _text = GetComponent<TMP_Text>();
            if (!_button) _button = GetComponentInParent<BuildButton>();
            
            _text.SetText(_button.Building().GetName());
            _text.color = BuildingListener.CanBuild(_button.Building()) ? Color : new Color(1, 1, 1, 0);
        }
    }
}
