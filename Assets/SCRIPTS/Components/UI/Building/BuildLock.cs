using GGG.Components.Buildings;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Buildings
{
    [RequireComponent(typeof(Image))]
    public class BuildLock : MonoBehaviour
    {
        private Image _image;
        private BuildButton _button;

        private void OnEnable()
        {
            if (!_image) _image = GetComponent<Image>();
            if (!_button) _button = GetComponentInParent<BuildButton>();

            _image.color = BuildingListener.CanBuild(_button.Building()) ? new Color(1, 1, 1, 0) : Color.white;
            _image.raycastTarget = !BuildingListener.CanBuild(_button.Building());
        }
    }
}
