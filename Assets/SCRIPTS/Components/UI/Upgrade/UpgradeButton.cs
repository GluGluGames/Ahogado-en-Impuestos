using System;
using GGG.Components.Buildings;
using GGG.Components.Core;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using GGG.Components.Serialization;
using GGG.Shared;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Upgrade
{
    [RequireComponent(typeof(Button))]
    public class UpgradeButton : MonoBehaviour
    {
        [SerializeField] private Sound BuildSound;

        private Button _button;
        private BuildingComponent _building;

        public Action OnBuildUpgrade;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(HexTile tile)
        {
            _building = tile.GetCurrentBuilding();
            bool firstCondition = _building.BuildData().CanUpgraded() &&
                                   _building.CurrentLevel() < _building.BuildData().GetMaxLevel();

            bool secondCondition = true;
            int level = _building.CurrentLevel();

            for (int i = 0; i < _building.BuildData().GetUpgradeCost().Length; i++)
            {
                if (PlayerManager.Instance.GetResourceCount(_building.BuildData().GetUpgradeResource(level, i).GetKey())
                    >= _building.BuildData().GetBuildingCost(i)) continue;
                
                
                secondCondition = false;
            }
            
            _button.image.color = firstCondition ? Color.white : new Color(0.81f, 0.81f, 0.81f);
            _button.interactable = firstCondition && secondCondition;
        }
        
        public void OnUpgradeButton()
        {
            if(GameManager.Instance.TutorialOpen() || GameManager.Instance.OnTutorial()) return;
            
            ResourceCost[] cost = _building.BuildData().GetUpgradeCost();
            int currentLevel = _building.CurrentLevel() - 1;
            
            for (int i = 0; i < cost[currentLevel].GetCostsAmount(); i++)
                PlayerManager.Instance.AddResource(cost[currentLevel].GetResource(i).GetKey(), -cost[currentLevel].GetCost(i));

            _building.AddLevel();
            _building.SetCurrentCost(cost[currentLevel]);
            _building.BuildData().Spawn(_building.transform.position, _building.transform, 
                _building.CurrentLevel(), true);
            SerializationManager.Instance.Save();
            SoundManager.Instance.Play(BuildSound);
            OnBuildUpgrade?.Invoke();
        }
    }
}
