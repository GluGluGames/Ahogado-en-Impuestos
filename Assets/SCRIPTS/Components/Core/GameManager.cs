using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using GGG.Shared;
using UnityEngine.Localization.Settings;

namespace GGG.Components.Core
{
    public class GameManager : MonoBehaviour {

        #region Singleton

        public static GameManager Instance;

        private Language _language;

        private void Awake() {
            if (Instance == null) Instance = this;
            
            if(!DebugMode) StartCoroutine(InitializeGame());
            StartCoroutine(initializeLanguage());
        }

        #endregion

        [SerializeField] private bool DebugMode;

        #region Functions
        private IEnumerator InitializeGame() {
            yield return null;

            for(int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if (scene == SceneManager.GetSceneByBuildIndex((int) SceneIndexes.SHARED) || !scene.isLoaded) continue;

                AsyncOperation a = SceneManager.UnloadSceneAsync(i);
                while(!a.isDone) yield return null;
            }

            AsyncOperation async = SceneManager.LoadSceneAsync((int) SceneIndexes.MAIN_MENU, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
        }
        private IEnumerator initializeLanguage()
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[
                PlayerPrefs.HasKey("LocalKey") ? PlayerPrefs.GetInt("LocalKey") : 0];
            _language = PlayerPrefs.HasKey("LocalKey") ? (Language)PlayerPrefs.GetInt("LocalKey") : Language.Spanish;
        }
        #endregion

        #region Getters & Setters
        public Language GetCurrentLanguage()
        {
            return _language;
        }
        public void SetLanguage(Language language)
        {
            _language = language;
        }
        #endregion
    }
}
