using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Shared;

using TMPro;
using UnityEngine;

namespace GGG.Components.UI
{
    public class HUDManager : MonoBehaviour {
        #region Singleton

        public static HUDManager Instance;
        
        private void Awake() {
            if (Instance != null) return;

            Instance = this;
        }

        #endregion

        [SerializeField] private TextMeshProUGUI SeaweedsText;
        [SerializeField] private Resource ShownResource;

        private PlayerManager _player;

        private bool _playerInitialized;

        private void Start() {
            _player = PlayerManager.Instance;

            SeaweedsText.gameObject.SetActive(false);
            _player.OnPlayerInitialized += () => {
                _playerInitialized = true;
                SeaweedsText.gameObject.SetActive(true);
            };
        }

        private void Update() {
            if (!_playerInitialized) return;
            
            SeaweedsText.SetText(_player.GetResourceCount(ShownResource.GetKey()).ToString());
        }

        public void ChangeScene(SceneIndexes sceneToLoad, SceneIndexes sceneToUnload)
        { 
            SceneManagement.Instance.AddSceneToUnload(sceneToUnload);
            SceneManagement.Instance.AddSceneToLoad(sceneToLoad);
            SceneManagement.Instance.UpdateScenes();
        }
    }
}
