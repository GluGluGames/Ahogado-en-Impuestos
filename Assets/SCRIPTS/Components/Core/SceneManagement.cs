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
        [SerializeField] private Button ExitButton;
        [SerializeField] private float CreditsDuration = 60f;

        private readonly List<AsyncOperation> _sceneAsyncOperation = new();
        private GraphicRaycaster _raycaster;
        private float _totalSceneProgress;
        private bool _settingsOpen;

        public Action OnGameSceneLoaded;
        public Action OnGameSceneUnloaded;

        public Action OnMinigameSceneLoaded;

        private void Start()
        {
            SceneManager.sceneUnloaded += (scene) =>
            {
                if(scene.buildIndex == (int) SceneIndexes.GAME_SCENE)
                    OnGameSceneUnloaded?.Invoke();;
            };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.GAME_SCENE)
                    OnGameSceneLoaded?.Invoke();
            };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.MINIGAME)
                    OnMinigameSceneLoaded?.Invoke();
            };
        }

        #region Scene Management

        public SceneIndexes CurrentScene() => (SceneIndexes)SceneManager.GetActiveScene().buildIndex;

        public void AddSceneToLoad(SceneIndexes scene)
        { 
           _sceneAsyncOperation.Add(SceneManager.LoadSceneAsync((int) scene, LoadSceneMode.Additive));
        }

        public void AddSceneToUnload(SceneIndexes scene)
        {
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
            
            LoadingScreenViewport.SetActive(false);
            _sceneAsyncOperation.Clear();
        }
        
        #endregion

        #region Settings

        public void OpenSettings()
        {
            SettingsViewport.SetActive(true);
            _raycaster.enabled = true;
            _settingsOpen = true;
        }

        public void CloseSettings()
        {
            SettingsViewport.SetActive(false);
            _raycaster.enabled = false;
            _settingsOpen = false;
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
