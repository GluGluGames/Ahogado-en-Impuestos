using System;
using UnityEngine;

namespace GGG.Components.Scenes
{
    public class LoadingScreen : MonoBehaviour
    {
        private SceneManagement _sceneManagement;
        private GameObject _viewport;

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;
            _sceneManagement.OnStartLoading += Open;
            _sceneManagement.OnEndLoading += Close;

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
        }

        private void Open()
        {
            _viewport.SetActive(true);
        }

        private void Close()
        {
            _viewport.SetActive(false);
        }
    }
}
