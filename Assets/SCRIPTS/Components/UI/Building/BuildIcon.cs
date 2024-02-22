using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Buildings
{
    [RequireComponent(typeof(Image))]
    public class BuildIcon : MonoBehaviour
    {
        private Image _image;
        private BuildButton _button;

        private void OnEnable()
        {
            if (!_image) _image = GetComponent<Image>();
            if (!_button) _button = GetComponentInParent<BuildButton>();

            _image.sprite = _button.Building().GetIcon();
            _image.color = BuildingListener.CanBuild(_button.Building()) ? Color.white : new Color(1, 1, 1, 0.5f);
        }
    }
}
