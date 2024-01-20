using System;
using GGG.Components.Core;
using GGG.Components.Scenes;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;


namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Button))]
    public class StartButton : MonoBehaviour
    {
        private SceneManagement _sceneManagement;

        private void OnEnable()
        {
            _sceneManagement = SceneManagement.Instance;
        }

        public void OnStartButton()
        {
            _sceneManagement.LoadScene(SceneIndexes.GAME_SCENE, SceneIndexes.MAIN_MENU);

            GameManager.Instance.StartGame();
        }
    }
}
