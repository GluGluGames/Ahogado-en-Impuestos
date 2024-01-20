using GGG.Components.Core;
using GGG.Components.Scenes;
using GGG.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGG.Components.Menus
{
    public class SettingsEndExpedition : MonoBehaviour
    {
        private SceneManagement _sceneManagement;
        private Settings _settings;
        
        private void OnEnable()
        {
            if (!_sceneManagement) _sceneManagement = SceneManagement.Instance;
            if (!_settings) _settings = FindObjectOfType<Settings>();
            
            gameObject.SetActive(SceneManagement.InMiniGameScene());
        }

        public void OnEndExpedition()
        {
            Scene currentScene = SceneManager.GetSceneAt(1);

            SceneIndexes currentSceneIndex = currentScene.name switch
            {
                "Minigame_Level1" => SceneIndexes.MINIGAME_LEVEL1,
                "Minigame_Level2" => SceneIndexes.MINIGAME_LEVEL2,
                "Minigame_Level3" => SceneIndexes.MINIGAME_LEVEL3,
                "Minigame_Level4" => SceneIndexes.MINIGAME_LEVEL4,
                _ => SceneIndexes.MINIGAME_LEVEL1
            };

            _sceneManagement.LoadScene(SceneIndexes.GAME_SCENE, currentSceneIndex);
            _settings.Close();
            
            GameManager.Instance.OnUIClose();
        }
    }
}
