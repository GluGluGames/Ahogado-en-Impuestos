using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using UnityEngine;

namespace GGG.Components.UI.Buildings
{
    public class BuildingListener : MonoBehaviour
    {
        private int _currentPage;
        private List<BuildingPanel> _panels;
        private List<BuildingArrow> _arrows;
        private BuildingPageText _pageText;

        private static HexTile _selectedTile;

        private void Awake()
        {
            _arrows = GetComponentsInChildren<BuildingArrow>().ToList();
            _arrows.ForEach(x=> x.OnArrowPress += OnPageChange);
            _panels = GetComponentsInChildren<BuildingPanel>().ToList();
            _pageText = GetComponentInChildren<BuildingPageText>();
        }

        private void OnEnable()
        {
            BuildingUI.OnUiOpen += Initialize;
            BuildingUI.OnUiClose += Deinitialize;
        }

        private void OnDisable()
        {
            BuildingUI.OnUiOpen -= Initialize;
            BuildingUI.OnUiClose -= Deinitialize;
        }

        private void Initialize(HexTile tile)
        {
            _selectedTile = tile;
            _currentPage = 0;
            
            _panels.ForEach(x => x.Initialize());
            _pageText.UpdateText(_currentPage, _panels.Count);
        }

        private void Deinitialize()
        {
            _selectedTile = null;
        }

        public static bool CanBuild(Building build)
        {
            if (!BuildingManager.Instance) return false;
            
            return (BuildingManager.Instance.GetBuildCount(build) < build.GetMaxBuildingNumber() || build.GetMaxBuildingNumber() == -1) 
                   && build.IsUnlocked();
        }

        public static HexTile SelectedTile() => _selectedTile;

        private void OnPageChange(int direction)
        {
            int page = _currentPage + direction;
            if (page < 0 || page > _panels.Count) return;

            _currentPage = page;
            _panels.ForEach(x => x.OnArrow(_currentPage));
            _pageText.UpdateText(_currentPage + 1, _panels.Count);
        }
    }
}
