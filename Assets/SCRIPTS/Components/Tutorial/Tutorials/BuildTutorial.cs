using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.Achievements;
using GGG.Components.Serialization;
using GGG.Components.HexagonalGrid;
using GGG.Components.UI;
using GGG.Components.UI.Buttons;
using UnityEngine;

namespace GGG.Components.Tutorial.Tutorials
{
    public class BuildTutorial : Tutorial
    {
        private AchievementsManager _achievements;
        private List<HexTile> _tiles;
        private List<BuildButton> _buildButtons;
        private LateralUI _lateralUI;
        private UpgradeUI _upgradeUI;
        private TileCleanUI _tileCleanUI;

        private enum TileStates
        {
            SellState,
            TileCleanState,
            Finish
        }
        
        private bool _structureBuild;
        private bool _upgradeOpen;
        private bool _upgradeClose;
        private bool _tileCleanOpen;
        private bool _tileCleanClose;
        
        protected override void TutorialCondition()
        {
            BuildingUI.OnUiOpen += StartTutorialNoEnum;
        }

        protected override void InitializeTutorial()
        {
            base.InitializeTutorial();

            _lateralUI = FindObjectOfType<LateralUI>();
            _achievements = AchievementsManager.Instance;
            
            _tiles = TileManager.Instance.GetHexTiles();
            _buildButtons = FindObjectsOfType<BuildButton>().ToList();
            _upgradeUI = FindObjectOfType<UpgradeUI>();
            _tileCleanUI = FindObjectOfType<TileCleanUI>();
            
            _buildButtons.ForEach(x => x.StructureBuild += OnStructureBuild);
            _upgradeUI.OnUiOpen += OnUpgradeOpen;
            _upgradeUI.OnCloseButtonPress += OnUpgradeClose;
            _tileCleanUI.OnUiOpen += OnTileCleanOpen;
            _tileCleanUI.OnUiClose += OnTileCleanClose;

            _steps = new List<IEnumerator>()
            {
                BuildStep(),
                UpgradeOpenStep(),
                UpgradeCloseStep(),
                TileCleanStep(),
                TileCleanCloseStep()
            };
        }

        IEnumerator BuildStep() => new WaitWhile(() => !_structureBuild);

        IEnumerator UpgradeOpenStep()
        {
            ChangeTileState(TileStates.SellState);
            yield return new WaitWhile(() => !_upgradeOpen);
        }

        IEnumerator UpgradeCloseStep() => new WaitWhile(() => !_upgradeClose);

        IEnumerator TileCleanStep()
        {
            ChangeTileState(TileStates.TileCleanState);
            yield return new WaitWhile(() => !_tileCleanOpen);
        }

        IEnumerator TileCleanCloseStep() => new WaitWhile(() => !_tileCleanClose);

        private void ChangeTileState(TileStates state)
        {
            foreach (HexTile tile in _tiles)
            {
                switch (state)
                {
                    case TileStates.Finish:
                        tile.selectable = true;
                        continue;
                    case TileStates.SellState:
                        tile.selectable = !tile.TileEmpty();
                        continue;
                    case TileStates.TileCleanState:
                        tile.selectable = tile.tileType != TileType.Standard && tile.tileType != TileType.Build;
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(state), state, null);
                }
            }
        }

        private void OnStructureBuild() => _structureBuild = true;
        private void OnUpgradeOpen() => _upgradeOpen = true;
        private void OnUpgradeClose() => _upgradeClose = true;
        private void OnTileCleanOpen() => _tileCleanOpen = true;
        private void OnTileCleanClose() => _tileCleanClose = true;

        protected override void FinishTutorial()
        {
            BuildingUI.OnUiOpen -= StartTutorialNoEnum;
            _lateralUI.ToggleOpenButton();
            ChangeTileState(TileStates.Finish);
            
            _buildButtons.ForEach(x => x.StructureBuild -= OnStructureBuild);
            _upgradeUI.OnUiOpen -= OnUpgradeOpen;
            _upgradeUI.OnCloseButtonPress -= OnUpgradeClose;
            _tileCleanUI.OnUiOpen -= OnTileCleanOpen;
            _tileCleanUI.OnUiClose -= OnTileCleanClose;

            StartCoroutine(_achievements.UnlockAchievement("01"));

            SerializationManager.Instance.Save();
            base.FinishTutorial();
        }
    }
}
