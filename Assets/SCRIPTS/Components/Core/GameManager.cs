using System;
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
        private Tutorials _currentTutorial;
        private bool _tutorialOpen;

        public static Action OnGameStart;

        #region Unity Events

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;

            _currentState = DebugMode ? GameState.PLAYING : GameState.MENU;
            _currentTutorial = Tutorials.None;
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
        
        public void StartGame()
        {
            _currentState = GameState.PLAYING;
            OnGameStart?.Invoke();
        }
        
        #endregion

        #region Getters & Setters
        public Language GetCurrentLanguage() => _language;
        public void SetLanguage(Language language) => _language = language;
        public GameState GetGameState() => _currentState;
        public Tutorials GetCurrentTutorial() => _currentTutorial;
        public void SetCurrentTutorial(Tutorials tutorial) => _currentTutorial = tutorial;
        public void OnUIOpen() => _currentState = GameState.ON_UI;
        public void OnUIClose() => _currentState = GameState.PLAYING;
        public bool OnTutorial() => _currentState == GameState.ON_TUTORIAL;
        public void SetTutorialOpen(bool open) => _tutorialOpen = open;
        public bool TutorialOpen() => _tutorialOpen;
        public bool IsOnUI() => _currentState == GameState.ON_UI;
        public bool PlayingGame() => _currentState is GameState.PLAYING or GameState.MINIGAME;

        #endregion
    }
}
