using GGG.Shared;

using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GGG.Components.Core
{
    public class SceneManagement : MonoBehaviour
    {
        #region Singleton
        
        public static SceneManagement Instance;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            
            // Loading Screen
            LoadingScreenViewport.SetActive(false);
            
            // Settings Screen
            SettingsViewport.SetActive(false);
            _raycaster = GetComponent<GraphicRaycaster>();
            _raycaster.enabled = false;
            
            // Credits Screen
            CreditsViewport.SetActive(false);
            CreditsPanel.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.6f);
            ExitButton.onClick.AddListener(() => StartCoroutine(CloseCredits(0)));
        }
        
        #endregion
        
        [Header("Loading Screen")] 
        [SerializeField] private GameObject LoadingScreenViewport;
        [SerializeField] private Image LoadingBar;
        [Space(5), Header("Settings Screen")] 
        [SerializeField] private GameObject SettingsViewport;
        [Space(5), Header("Credits Screen")] 
        [SerializeField] private GameObject CreditsViewport;
        [SerializeField] private GameObject CreditsPanel;
        [SerializeField] private Button ExpeditionExitButton;
        [SerializeField] private Button ExitButton;
        [SerializeField] private float CreditsDuration = 60f;

        private readonly List<AsyncOperation> _sceneAsyncOperation = new();
        private IEnumerator _enumeratorOperations;
        private SoundManager _soundManager;
        private GraphicRaycaster _raycaster;
        private float _totalSceneProgress;
        private bool _settingsOpen;

        public Action OnGameSceneLoaded;
        public Action OnGameSceneUnloaded;
        public Action OnMinigameSceneLoaded;

        private void Start()
        {
            _soundManager = SoundManager.Instance;
            
            ExpeditionExitButton.onClick.AddListener(ExitExpedition);
            
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.GAME_SCENE)
                {
                    _soundManager.Stop("MainMenu");
                }

            };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.GAME_SCENE)
                {
                    OnGameSceneLoaded?.Invoke();
                    
                    _soundManager.Stop("MainMenu");
                    _soundManager.Play("MainTheme");
                    _soundManager.Play("AmbientSound");
                }

                if (scene.buildIndex is (int)SceneIndexes.MINIGAME_LEVEL1 or (int)SceneIndexes.MINIGAME_LEVEL2 or
                    (int)SceneIndexes.MINIGAME_LEVEL3 or (int)SceneIndexes.MINIGAME_LEVEL4)
                {
                    OnMinigameSceneLoaded?.Invoke();
                    // _soundManager.Play("MinigameTheme");
                }
            };
        }

        #region Scene Management

        public static bool InGameScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i) == SceneManager.GetSceneByBuildIndex((int)SceneIndexes.GAME_SCENE))
                    return true;
            }

            return false;
        }
        
        public static bool InMiniGameScene()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i) == SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME_LEVEL1) ||
                    SceneManager.GetSceneAt(i) == SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME_LEVEL2) ||
                    SceneManager.GetSceneAt(i) == SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME_LEVEL3) ||
                    SceneManager.GetSceneAt(i) == SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME_LEVEL4))
                    return true;
            }

            return false;
        }
        
        public void AddSceneToLoad(SceneIndexes scene)
        { 
           _sceneAsyncOperation.Add(SceneManager.LoadSceneAsync((int) scene, LoadSceneMode.Additive));
        }

        public void AddSceneToUnload(SceneIndexes scene)
        {
            if(scene == SceneIndexes.GAME_SCENE) OnGameSceneUnloaded?.Invoke();
            _sceneAsyncOperation.Add(SceneManager.UnloadSceneAsync((int) scene));
        }

        public void UnloadActiveScenes()
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if(scene != SceneManager.GetSceneByBuildIndex((int) SceneIndexes.SHARED) && scene.isLoaded)
                    AddSceneToUnload((SceneIndexes) scene.buildIndex);
            }
        }

        public void UpdateScenes()
        {
            if (_sceneAsyncOperation.Count <= 0)
                throw new Exception("No scenes to update");
            
            StartCoroutine(GetSceneLoadProgress());
        }

        public void AddEnumerators(IEnumerator enumerators) => _enumeratorOperations = enumerators;
        
        private IEnumerator GetSceneLoadProgress()
        {
            LoadingScreenViewport.SetActive(true);
            
            foreach (AsyncOperation operation in _sceneAsyncOperation)
            {
                _totalSceneProgress = 0;
                
                while (!operation.isDone)
                {
                    foreach (AsyncOperation op in _sceneAsyncOperation)
                    {
                        _totalSceneProgress += op.progress;
                    }

                    _totalSceneProgress /= _sceneAsyncOperation.Count;
                    LoadingBar.fillAmount = _totalSceneProgress;
                    
                    yield return null;
                }
            }

            if (_enumeratorOperations != null) 
                yield return _enumeratorOperations;
            
            LoadingScreenViewport.SetActive(false);
            _sceneAsyncOperation.Clear();
        }
        
        #endregion

        #region Settings

        public void OpenSettings()
        {
            SettingsViewport.SetActive(true);
            
            ExpeditionExitButton.gameObject.SetActive(InMiniGameScene());
            ExpeditionExitButton.interactable = InMiniGameScene();
            
            ExitButton.gameObject.SetActive(!InMiniGameScene());
            ExitButton.interactable = !InMiniGameScene();
            
            _raycaster.enabled = true;
            _settingsOpen = true;
        }

        public void CloseSettings()
        {
            SettingsViewport.SetActive(false);
            _raycaster.enabled = false;
            _settingsOpen = false;
        }

        private void ExitExpedition()
        {
            Scene currentScene = SceneManager.GetSceneAt(1);
            SceneIndexes currentSceneIndex = SceneIndexes.MINIGAME_LEVEL1;

            if (currentScene.name == "Minigame_Level1")
            {
                currentSceneIndex = SceneIndexes.MINIGAME_LEVEL1;
            }
            else if (currentScene.name == "Minigame_Level2")
            {
                currentSceneIndex = SceneIndexes.MINIGAME_LEVEL2;
            }
            else if (currentScene.name == "Minigame_Level3")
            {
                currentSceneIndex = SceneIndexes.MINIGAME_LEVEL3;
            }
            else if (currentScene.name == "Minigame_Level4")
            {
                currentSceneIndex = SceneIndexes.MINIGAME_LEVEL4;
            }

            AddSceneToLoad(SceneIndexes.GAME_SCENE);
            AddSceneToUnload(currentSceneIndex);
            UpdateScenes();
            CloseSettings();
            GameManager.Instance.OnUIClose();
        }
        
        #endregion

        #region Credits

        public void OpenCredits()
        {
            CreditsViewport.SetActive(true);
            CreditsPanel.transform.DOMoveY(Screen.height * 4.8f, CreditsDuration).SetEase(Ease.Linear).onComplete += 
                () => StartCoroutine(CloseCredits(2));
            _raycaster.enabled = true;
        }

        private IEnumerator CloseCredits(int waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            
            CreditsViewport.SetActive(false);
            CreditsPanel.transform.DOKill();
            CreditsPanel.transform.position = new Vector3(Screen.width * 0.5f, Screen.height * -0.6f);
            if(!_settingsOpen) _raycaster.enabled = false;
        }

        #endregion

    }
}
