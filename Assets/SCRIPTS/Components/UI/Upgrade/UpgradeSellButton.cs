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
    public class UpgradeSellButton : MonoBehaviour
    {
        [SerializeField] private Sound SellSound;

        private Button _button;
        private BuildingComponent _building;
        private HexTile _tile;

        public Action OnBuildSell;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        public void Initialize(HexTile tile)
        {
            _building = tile.GetCurrentBuilding();
            _tile = tile;
            
            bool sell = _building.BuildData().GetKey() != "CityHall";
            _button.image.color = sell ? Color.white : new Color(0.81f, 0.81f, 0.81f);
            _button.interactable = sell;
        }
        
        public void OnSellButton()
        {
            if(GameManager.Instance.TutorialOpen() || GameManager.Instance.OnTutorial()) return;
            
            PlayerManager player = PlayerManager.Instance;
            int cost = BuildingManager.Instance.GetBuildingCost(_building.BuildData()).GetCost(0);
            
            player.AddResource("Seaweed", Mathf.RoundToInt(cost * 0.5f));
            _building.OnBuildDestroy();
            BuildingManager.Instance.RemoveBuilding(_building);
            SoundManager.Instance.Play(SellSound);
            _tile.DestroyBuilding();
            SerializationManager.Instance.Save();
            
            _building = null;
            OnBuildSell?.Invoke();
        }
    }
}
