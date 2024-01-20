using GGG.Shared;

using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GGG.Components.Scenes
{
    public class SceneManagement : MonoBehaviour
    {
        #region Singleton
        
        public static SceneManagement Instance;

        private void Awake()
        {
            if (Instance != null) return;
            
            Instance = this;
        }
        
        #endregion
        
        private readonly List<AsyncOperation> _sceneAsyncOperation = new();
        private IEnumerator _enumeratorOperations;
        private SoundManager _soundManager;
        private float _totalSceneProgress;

        public Action OnStartLoading;
        public Action OnEndLoading;
        public Action<float> OnLoadProgress;
        
        public Action OnGameSceneLoaded;
        public Action OnGameSceneUnloaded;
        public Action OnMinigameSceneLoaded;
        public Action OnMinigameSceneUnloaded;

        private void Start()
        {
            _soundManager = SoundManager.Instance;
            
            SceneManager.sceneUnloaded += (scene) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.GAME_SCENE)
                {
                    _soundManager.Stop("MainMenu");
                }

                if (scene.buildIndex is (int)SceneIndexes.MINIGAME_LEVEL1 or (int)SceneIndexes.MINIGAME_LEVEL2 or
                    (int)SceneIndexes.MINIGAME_LEVEL3 or (int)SceneIndexes.MINIGAME_LEVEL4)
                {
                    OnMinigameSceneUnloaded?.Invoke();
                }

            };

            SceneManager.sceneLoaded += (scene, mode) =>
            {
                if (scene.buildIndex == (int)SceneIndexes.GAME_SCENE)
                {
                    OnGameSceneLoaded?.Invoke();
                    
                    _soundManager.Stop("MainMenu");
                    if(!_soundManager.IsPlaying("MainTheme")) _soundManager.Play("MainTheme");
                    if(!_soundManager.IsPlaying("AmbientSound")) _soundManager.Play("AmbientSound");
                }

                if (scene.buildIndex is (int)SceneIndexes.MINIGAME_LEVEL1 or (int)SceneIndexes.MINIGAME_LEVEL2 or
                    (int)SceneIndexes.MINIGAME_LEVEL3 or (int)SceneIndexes.MINIGAME_LEVEL4)
                {
                    OnMinigameSceneLoaded?.Invoke();
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

        public void LoadScene(SceneIndexes sceneToLoad, SceneIndexes currentScene)
        {
            AddSceneToUnload(currentScene);
            AddSceneToLoad(sceneToLoad);
            UpdateScenes();
        }

        public void LoadScene(SceneIndexes sceneToLoad)
        {
            UnloadActiveScenes();
            AddSceneToLoad(sceneToLoad);
            UpdateScenes();
        }
        
        private void AddSceneToLoad(SceneIndexes scene)
        { 
           _sceneAsyncOperation.Add(SceneManager.LoadSceneAsync((int) scene, LoadSceneMode.Additive));
        }

        private void AddSceneToUnload(SceneIndexes scene)
        {
            if(scene == SceneIndexes.GAME_SCENE) OnGameSceneUnloaded?.Invoke();
            _sceneAsyncOperation.Add(SceneManager.UnloadSceneAsync((int) scene));
        }

        private void UnloadActiveScenes()
        {
            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                Scene scene = SceneManager.GetSceneByBuildIndex(i);
                if(scene != SceneManager.GetSceneByBuildIndex((int) SceneIndexes.SHARED) && scene.isLoaded)
                    AddSceneToUnload((SceneIndexes) scene.buildIndex);
            }
        }

        private void UpdateScenes()
        {
            if (_sceneAsyncOperation.Count <= 0)
                throw new Exception("No scenes to update");
            
            StartCoroutine(GetSceneLoadProgress());
        }

        public void AddEnumerators(IEnumerator enumerators) => _enumeratorOperations = enumerators;
        
        private IEnumerator GetSceneLoadProgress()
        {
            OnStartLoading?.Invoke();
            
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
                    OnLoadProgress?.Invoke(_totalSceneProgress);
                    
                    yield return null;
                }
            }

            if (_enumeratorOperations != null) 
                yield return _enumeratorOperations;
            
            OnEndLoading?.Invoke();
            _sceneAsyncOperation.Clear();
        }
        
        #endregion

    }
}
