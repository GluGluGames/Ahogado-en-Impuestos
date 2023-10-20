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
        [SerializeField] private BasicResource ShownResource;
        [SerializeField] private bool OnMinigame;
        
        private PlayerManager _player;
        private BuildingUI _buildingUI;
        private TileCleanUI _tileCleanUI;
        private UpgradeUI _upgradeUI;

        private void Start() {
            _player = PlayerManager.Instance;

            _buildingUI = GetComponentInChildren<BuildingUI>();
            _tileCleanUI = GetComponentInChildren<TileCleanUI>();
            _upgradeUI = GetComponentInChildren<UpgradeUI>();
            
            MenusHandle();
        }

        private void Update()
        {
            SeaweedsText.SetText(_player.GetResourceCount(ShownResource.GetName().GetLocalizedString()).ToString());
        }

        private void MenusHandle()
        {
            if (OnMinigame) return;
            
            _buildingUI.OnMenuOpen += () => {
                _tileCleanUI.Close();
                _upgradeUI.Close();
            };

            _tileCleanUI.OnMenuOpen += () => {
                _buildingUI.Close();
                _upgradeUI.Close();
            };

            _upgradeUI.OnMenuOpen += () => {
                _buildingUI.Close();
                _tileCleanUI.Close();
            };
        }
    }
}
