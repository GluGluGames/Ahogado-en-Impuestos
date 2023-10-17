using GGG.Shared;
using GGG.Components.Core;

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button CreditsButton;

        private SceneManagement _sceneManagement;

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;
            
            PlayButton.onClick.AddListener(OnStartButton);
            SettingsButton.onClick.AddListener(OnSettingsButton);
            CreditsButton.onClick.AddListener(OnCreditsButton);
        }

        private void OnStartButton()
        {
            _sceneManagement.AddSceneToUnload(SceneIndexes.MAIN_MENU);
            _sceneManagement.AddSceneToLoad(SceneIndexes.GAME_SCENE);
            _sceneManagement.UpdateScenes();
            
            GameManager.Instance.SetGameState(GameState.PLAYING);
        }

        private void OnSettingsButton()
        {
            _sceneManagement.OpenSettings();
        }

        private void OnCreditsButton()
        {
            _sceneManagement.OpenCredits();
        }
    }
}
