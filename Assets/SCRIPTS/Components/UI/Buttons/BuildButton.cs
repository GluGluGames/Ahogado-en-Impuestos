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
        private int _cost;

        public Action<BuildingComponent, HexTile> OnStructureBuild;

        public void Initialize()
        {
            _player = PlayerManager.Instance;
            _buildResource = _player.GetMainResource();
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }

            _cost = BuildingInfo.GetPrimaryPrice();
            
            TextMeshProUGUI[] texts = Container.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = Container.GetComponentsInChildren<Image>();

            if (texts.Length == 2 && images.Length == 2)
            {
                texts[0].text = BuildingInfo.GetPrimaryPrice().ToString();
                images[0].sprite = BuildingInfo.GetPrimaryResource().GetSprite();

                if (BuildingInfo.GetSecondaryPrice() == 0)
                {
                    images[1].gameObject.SetActive(false);
                }
                else
                {
                    texts[1].text = BuildingInfo.GetSecondaryPrice().ToString();
                    images[1].sprite = BuildingInfo.GetSecondaryResource().GetSprite();
                }
            }
        }

        private void BuildStructure() {
            bool aux = BuildingInfo.GetSecondaryPrice() != 0;

            if (_player.GetResourceCount(BuildingInfo.GetPrimaryResource().GetKey()) < BuildingInfo.GetPrimaryPrice() || aux && _player.GetResourceCount(BuildingInfo.GetSecondaryResource()?.GetKey()) < BuildingInfo.GetSecondaryPrice())
            {
                // TODO - Can't buy warning
                return;
            }
            
            SoundManager.Instance.Play("Build");
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition(), GameObject.Find("Buildings").transform);
            _auxBuild = auxGo.GetComponent<BuildingComponent>();
            BuildingManager.Instance.AddBuilding(_auxBuild);

            _selectedHexTile.SetBuilding(_auxBuild);
            OnStructureBuild?.Invoke(_auxBuild, _selectedHexTile);

            _player.AddResource(BuildingInfo.GetPrimaryResource().GetKey(), -BuildingInfo.GetPrimaryPrice());
            if(aux) _player.AddResource(BuildingInfo.GetSecondaryResource()?.GetKey(), -BuildingInfo.GetSecondaryPrice());


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
