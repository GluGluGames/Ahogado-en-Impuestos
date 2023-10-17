using GGG.Shared;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameInitializer
{
    [RuntimeInitializeOnLoadMethod]
    public static void RunOnStart()
    {
        if (!SceneManager.GetSceneByBuildIndex((int)SceneIndexes.SHARED).isLoaded)
            SceneManager.LoadSceneAsync((int)SceneIndexes.SHARED, LoadSceneMode.Additive);
    }
}
