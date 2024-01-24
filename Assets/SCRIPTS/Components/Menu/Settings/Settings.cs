using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(GraphicRaycaster))]
    public class Settings : MonoBehaviour
    {
        private GameObject _viewport;
        private GraphicRaycaster _raycaster;
        private SoundSlider[] _sliders;

        #region Unity Methods

        private void Start()
        {
            _raycaster = GetComponent<GraphicRaycaster>();
            _raycaster.enabled = false;

            _sliders = GetComponentsInChildren<SoundSlider>();
            foreach (SoundSlider soundSlider in _sliders)
                soundSlider.Initialize();
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        #endregion

        #region Methods

        public void Open()
        {
            _viewport.SetActive(true);
            _raycaster.enabled = true;
        }

        public void Close()
        {
            _viewport.SetActive(false);
            _raycaster.enabled = false;
        }

        #endregion
    }
}