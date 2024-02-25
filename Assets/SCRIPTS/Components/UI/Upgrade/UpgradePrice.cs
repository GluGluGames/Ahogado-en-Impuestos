using System.Collections.Generic;
using GGG.Classes.Buildings;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.Upgrade
{
    public class UpgradePrice : MonoBehaviour
    {
        private Building _building;
        private List<GameObject> _children = new();
        private List<TMP_Text> _prices = new();
        private List<Image> _resources = new();
        
        public void Initialize(HexTile tile)
        {
            _building = tile.GetCurrentBuilding().BuildData();
            int level = tile.GetCurrentBuilding().CurrentLevel();
            
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                
                if (!_building.GetUpgradeResource(level, i))
                {
                    child.SetActive(false);
                    continue;
                }
                
                child.SetActive(true);
                if (_children.Contains(child))
                {
                    _prices[i].SetText(_building.GetUpgradeCost(level, i).ToString());
                    _resources[i].sprite = _building.GetUpgradeResource(level, i).GetSprite();
                    continue;
                }
                
                TMP_Text text = child.GetComponentInChildren<TMP_Text>();
                text.SetText(_building.GetUpgradeCost(level, i).ToString());
                _prices.Add(text);
                
                Image image = child.GetComponentInChildren<Image>();
                image.sprite = _building.GetUpgradeResource(level, i).GetSprite();
                _resources.Add(image);
                
                _children.Add(child);
            }
        }
    }
}
