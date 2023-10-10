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
        
        private PlayerManager _player;

        private void Start() {
            _player = PlayerManager.Instance;
        }

        private void Update() {
            SeaweedsText.text = $"Seaweeds: {_player.GetResourceCount(BasicResources.SEAWEED)}";
        }
    }
}
