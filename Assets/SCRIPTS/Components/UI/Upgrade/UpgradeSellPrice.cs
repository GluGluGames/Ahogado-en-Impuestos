using System.Collections.Generic;
using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Upgrade
{
    public class UpgradeSellPrice : MonoBehaviour
    {
        private Building _building;
        private GameObject _children;
        private TMP_Text _price;
        private Image _resource;

        public void Initialize(HexTile tile)
        {
            _building = tile.GetCurrentBuilding().BuildData();
            bool sell = _building.GetKey() != "CityHall";
            
            _children = transform.GetChild(0).gameObject;
            _children.SetActive(sell);
            if (!sell) return;
            
            if (!_resource) _resource = _children.GetComponentInChildren<Image>();
            if (!_price) _price = _children.GetComponentInChildren<TMP_Text>();
                
            _price.SetText(Mathf.RoundToInt(BuildingManager.Instance.GetBuildingCost(_building).GetCost(0) * 0.5f).ToString());
            _resource.sprite = BuildingManager.Instance.GetBuildingCost(_building).GetResource(0).GetSprite();
        }
    }
}
