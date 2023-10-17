using UnityEngine;
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
            
            StartCoroutine(initializeLanguage());
        }

        #endregion

        [SerializeField] private bool DebugMode;

        private SceneManagement _sceneManagement;
        private GameState _currentState;

        #region Unity Events

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;

            _currentState = DebugMode ? GameState.PLAYING : GameState.MENU;
            if (!DebugMode) InitializeGame();
        }

        #endregion

        #region Functions
        
        private void InitializeGame() {
            _sceneManagement.UnloadActiveScenes();
            _sceneManagement.AddSceneToLoad(SceneIndexes.MAIN_MENU);
            _sceneManagement.UpdateScenes();
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
        public Language GetCurrentLanguage() => _language;
        public void SetLanguage(Language language) => _language = language;
        public GameState GetGameState() => _currentState;
        public void SetGameState(GameState state) => _currentState = state;
        public bool PlayingGame() => _currentState is GameState.PLAYING or GameState.MINIGAME;

        #endregion
    }
}
