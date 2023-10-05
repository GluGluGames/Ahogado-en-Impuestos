using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GGG.Shared;

public class MainMenu : MonoBehaviour
{
    private List<AsyncOperation> _sceneLoading = new();

    public void OnStartButton()
    {
        _sceneLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_MENU));
        _sceneLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME_SCENE, LoadSceneMode.Additive));
        StartCoroutine(LoadSceneAsync());
    }

    private IEnumerator LoadSceneAsync()
    {
        foreach(AsyncOperation a in _sceneLoading) {
            while (!a.isDone) {
                //TODO - Load screen

                yield return null;
            }
        }
    }
}
