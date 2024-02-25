using System;
using GGG.Components.HexagonalGrid;
using UnityEngine;

namespace GGG.Components.UI.Upgrade
{
    public class UpgradeListener : MonoBehaviour
    {
        private UpgradeUI _ui;
        private UpgradeButton _upgradeButton;
        private UpgradeInteractButton _interactButton;
        private UpgradeSellButton _sellButton;
        private UpgradePrice _upgradePrice;
        private UpgradeSellPrice _sellPrice;

        private HexTile _currentTile;

        private void Awake()
        {
            _ui = GetComponent<UpgradeUI>();
            _upgradeButton = GetComponentInChildren<UpgradeButton>();
            _interactButton = GetComponentInChildren<UpgradeInteractButton>();
            _sellButton = GetComponentInChildren<UpgradeSellButton>();
            _upgradePrice = GetComponentInChildren<UpgradePrice>();
            _sellPrice = GetComponentInChildren<UpgradeSellPrice>();
        }

        private void OnEnable()
        {
            _ui.OnUiOpen += Initialize;
            _ui.OnUIClose += Deinitialize;
        }

        private void OnDisable()
        {
            _ui.OnUiOpen -= Initialize;
            _ui.OnUIClose -= Deinitialize;
        }

        private void Initialize(HexTile tile)
        {
            _currentTile = tile;
            
            _sellButton.Initialize(tile);
            _interactButton.Initialize(tile);
            _upgradeButton.Initialize(tile);
            _upgradePrice.Initialize(tile);
            _sellPrice.Initialize(tile);
        }

        private void Deinitialize()
        {
            _currentTile.DeselectTile();
            _currentTile = null;
        }
    }
}
