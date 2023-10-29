using DG.Tweening;
using GGG.Classes.Buildings;
using TMPro;
using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.EventSystems;
using System;
using GGG.Components.Player;
using GGG.Shared;
using UnityEngine.UI;

namespace GGG.Components.UI {
    public class BuildButton : MonoBehaviour, IPointerDownHandler {
        [SerializeField] private Building BuildingInfo;
        [SerializeField] private GameObject Container;
        
        private PlayerManager _player;
        private HexTile _selectedHexTile;
        private Resource _buildResource;
        private BuildingComponent _auxBuild;
        private ResourceCost _cost;

        public Action<BuildingComponent, HexTile> OnStructureBuild;

        public void Initialize()
        {
            _player = PlayerManager.Instance;
            _buildResource = _player.GetMainResource();
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }

            _cost = BuildingInfo.GetBuildingCost();
            
            TextMeshProUGUI[] texts = Container.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = Container.GetComponentsInChildren<Image>();
            
            texts[0].text = _cost.GetCost(0).ToString();
            images[0].sprite = _cost.GetResource(0).GetSprite();

            for (int i = 1; i < images.Length; i++)
            {
                if (i >= _cost.GetCostsAmount())
                {
                    images[i].gameObject.SetActive(false);
                    continue;
                }
                
                if (_cost.GetCost(i) == 0)
                {
                    images[i].gameObject.SetActive(false);
                    continue;
                }
                
                texts[i].SetText(_cost.GetCost(i).ToString());
                images[i].sprite = _cost.GetResource(i).GetSprite();
            }
            
        }

        private void BuildStructure()
        {
            for (int i = 0; i < _cost.GetCostsAmount(); i++)
            {
                if (_player.GetResourceCount(_cost.GetResource(i).GetKey()) < _cost.GetCost(i))
                {
                    // TODO - Can't buy warning
                    return;
                }
            }
            
            SoundManager.Instance.Play("Build");
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition(), GameObject.Find("Buildings").transform, 1, false);
            _auxBuild = auxGo.GetComponent<BuildingComponent>();

            _selectedHexTile.SetBuilding(_auxBuild);
            OnStructureBuild?.Invoke(_auxBuild, _selectedHexTile);

            for (int i = 0; i < _cost.GetCostsAmount(); i++)
            {
                if(!_cost.GetResource(i)) continue;
                
                _player.AddResource(_cost.GetResource(i).GetKey(), -_cost.GetCost(i));
            }


            //FOW
            _selectedHexTile.Reveal(_auxBuild.GetVisionRange(), 0);

            _selectedHexTile = null;
        }

        #region Event Systems Method

        public void OnPointerDown(PointerEventData eventData) {
            if (!_selectedHexTile.TileEmpty()) return;
            
            BuildStructure();
        }

        #endregion

    }
    
}
