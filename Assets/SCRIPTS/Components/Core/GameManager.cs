using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace GGG.Components.Core
{
    public class GameManager : MonoBehaviour {

        #region Singleton

        public static GameManager Instance;

        private void Awake() {
            if (Instance == null) Instance = this;
            
            if(!DebugMode) StartCoroutine(InitializeGame());
        }

        #endregion

        [SerializeField] private bool DebugMode;

        private IEnumerator InitializeGame() {
            yield return null;
            const int SCENE_IDX = 1;
            
            // TODO - Load Main Menu
            // TEMP:
            AsyncOperation async = SceneManager.LoadSceneAsync(SCENE_IDX, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
        }
    }
}
