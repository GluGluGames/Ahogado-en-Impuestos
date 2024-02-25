using System;
using GGG.Components.Buildings;
using GGG.Components.Core;
using GGG.Components.HexagonalGrid;
using UnityEngine;

namespace GGG.Components.UI.Upgrade
{
    public class UpgradeInteractButton : MonoBehaviour
    {
        private BuildingComponent _building;

        public Action OnBuildInteract;

        public void Initialize(HexTile tile)
        {
            _building = tile.GetCurrentBuilding();
        }

        public void OnInteract()
        {
            if (GameManager.Instance.OnTutorial() || GameManager.Instance.TutorialOpen()) return;
            
            _building.Interact();
            OnBuildInteract?.Invoke();
        }
    }
}
