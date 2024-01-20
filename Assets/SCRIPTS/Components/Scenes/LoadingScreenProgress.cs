using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Scenes
{
    [RequireComponent(typeof(Image))]
    public class LoadingScreenProgress : MonoBehaviour
    {
        private SceneManagement _sceneManagement;
        private Image _fill;

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;
            _sceneManagement.OnLoadProgress += UpdateFill;
            
            _fill = GetComponent<Image>();
            _fill.fillAmount = 0;
        }

        private void UpdateFill(float amount) => _fill.fillAmount = amount;
    }
}
