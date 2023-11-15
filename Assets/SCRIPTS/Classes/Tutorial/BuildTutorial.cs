using System;
using System.Collections;
using GGG.Components.Buildings;
using GGG.Components.UI;
using UnityEngine;

namespace GGG.Classes.Tutorial
{
    [CreateAssetMenu(fileName = "BuildTutorial", menuName = "Game/Tutorials/BuildTutorial")]
    public class BuildTutorial : TutorialBase
    {
        private BuildingUI _buildingUi;
        private BuildButton[] _buildButtons;

        private UpgradeUI _upgradeUI;
        private TileCleanUI _tileCleanUI;

        private HexTile[] _tiles;
        
        private bool _structureBuild;
        private bool _upgradeMenuOpen;
        private bool _structureSold;
        private bool _tileCleanOpen;

        private enum Steps
        {
            SellStep,
            TileCleanStep,
            Finish
        }
        
        public override IEnumerator StartTutorial(Action OnTutorialStart, Action<bool, bool> OnTutorialEnd, 
            Action<string, Sprite, string> OnUiChange)
        {
            InitializeTutorial();

            #region Structure Build Tutorial
            
            for (int i = 0; i < 2; i++)
            {
                yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            }

            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, false);
            yield return BuildStep();
            
            #endregion

            #region Upgrade Menu Open

            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, true);
            yield return UpgradeMenuStep();

            #endregion

            #region Structure Sold Tutorial

            for (int i = 0; i < 2; i++)
            {
                yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            }
            
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, false);
            yield return StructureSoldStep();

            #endregion

            #region Tile Clean Tutorial

            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, true, true);
            yield return TileCleanStep();
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, false, false, false);
            _tileCleanUI.Close();
            
            #endregion
            
            yield return TutorialOpen(OnTutorialStart, OnTutorialEnd, OnUiChange, true, true, true);
            
            FinishTutorial();
        }

        protected override void InitializeTutorial()
        {
            _structureBuild = false;
            _upgradeMenuOpen = false;
            _structureSold = false;
            _tileCleanOpen = false;
            
            _buildButtons = FindObjectsOfType<BuildButton>();
            _tiles = FindObjectsOfType<HexTile>();
            if (!_buildingUi) _buildingUi = FindObjectOfType<BuildingUI>();
            if (!_upgradeUI) _upgradeUI = FindObjectOfType<UpgradeUI>();
            if (!_tileCleanUI) _tileCleanUI = FindObjectOfType<TileCleanUI>();
            
            foreach (BuildButton button in _buildButtons)
                button.StructureBuild += CheckStructureBuild;

            _upgradeUI.OnUiOpen += CheckUpgradeMenuOpen;
            _upgradeUI.OnSellButtonPress += CheckStructureSold;
            _tileCleanUI.OnUiOpen += CheckTileCleanUi;
        }

        protected override void FinishTutorial()
        {
            TutorialCompleted = true;
            
            ChangeTilesState(Steps.Finish);
            foreach (BuildButton button in _buildButtons)
                button.StructureBuild -= CheckStructureBuild;
            
            _upgradeUI.OnUiOpen -= CheckUpgradeMenuOpen;
            _upgradeUI.OnSellButtonPress -= CheckStructureSold;
            _tileCleanUI.OnUiOpen -= CheckTileCleanUi;
        }

        private IEnumerator BuildStep() {
            while (!_structureBuild)
            {
                yield return null;
            }
        }

        private IEnumerator UpgradeMenuStep()
        {
            ChangeTilesState(Steps.SellStep);
            
            while (!_upgradeMenuOpen)
            {
                yield return null;
            }
        }
        
        private IEnumerator StructureSoldStep()
        {
            while (!_structureSold)
            {
                yield return null;
            }
        }
        
        private IEnumerator TileCleanStep()
        {
            ChangeTilesState(Steps.TileCleanStep);
            
            while (!_tileCleanOpen)
            {
                yield return null;
            }
        }

        private void ChangeTilesState(Steps step)
        {
            foreach (HexTile tile in _tiles)
            {
                switch (step)
                {
                    case Steps.Finish:
                        tile.selectable = true;
                        continue;
                    case Steps.SellStep:
                    {
                        if (tile.GetCurrentBuilding()) tile.selectable = true;
                        else tile.selectable = false;
                        continue;
                    }
                    case Steps.TileCleanStep:
                    {
                        if (tile.tileType != TileType.Standard) tile.selectable = true;
                        else tile.selectable = false;
                        continue;
                    }
                }
            }
        }
        
        private void CheckStructureBuild() => _structureBuild = true;

        private void CheckUpgradeMenuOpen() => _upgradeMenuOpen = true;

        private void CheckStructureSold() => _structureSold = true;

        private void CheckTileCleanUi() => _tileCleanOpen = true;
    }
}
