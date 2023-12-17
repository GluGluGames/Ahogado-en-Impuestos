using GGG.Shared;
using GGG.Components.Core;

using UnityEngine;
using UnityEngine.UI;
using System.IO;
using GGG.Components.Serialization;

namespace GGG.Components.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private Button PlayButton;
        [SerializeField] private Button SettingsButton;
        [SerializeField] private Button ExitButton;

        private SceneManagement _sceneManagement;

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;

            SoundManager.Instance.Play("MainMenu");
            
            PlayButton.onClick.AddListener(OnStartButton);
            SettingsButton.onClick.AddListener(OnSettingsButton);
            ExitButton.onClick.AddListener(OnExitButton);
        }

        private void OnStartButton()
        {
            _sceneManagement.AddSceneToUnload(SceneIndexes.MAIN_MENU);
            _sceneManagement.AddSceneToLoad(SceneIndexes.GAME_SCENE);
            _sceneManagement.UpdateScenes();
            
            GameManager.Instance.StartGame();
        }

        private void OnSettingsButton()
        {
            _sceneManagement.OpenSettings();
        }

        private void OnCreditsButton()
        {
            _sceneManagement.OpenCredits();
        }

        public void OnDataDeleteConfirmButton()
        {
            foreach (var directory in Directory.GetDirectories(Application.persistentDataPath))
            {
                DirectoryInfo data_dir = new DirectoryInfo(directory);
                data_dir.Delete(true);
            }

            foreach (var file in Directory.GetFiles(Application.persistentDataPath))
            {
                FileInfo file_info = new FileInfo(file);
                file_info.Delete();
            }

            PlayerPrefs.DeleteAll();
            StartCoroutine(FindObjectOfType<SerializationManager>().ResetStats());
        }

        private void OnExitButton()
        {
            Application.Quit();
        }
    }
}
