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
    public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
        [SerializeField] private Building BuildingInfo;
        [SerializeField] private TextMeshProUGUI BuildNameText;
        [SerializeField] private TMP_Text PriceText;

        private HexTile _selectedHexTile;
        private BuildingComponent _auxBuild;
        private PlayerManager _player;
        private int _cost;

        public Action<BuildingComponent, HexTile> OnStructureBuild;

        public void Initialize()
        {
            _player = PlayerManager.Instance;
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }

            _cost = BuildingInfo.GetPrice();
            
            BuildNameText.SetText(BuildingInfo.GetName());
            PriceText.SetText(_cost.ToString());
        }

        private void BuildStructure() {
            if (_player.GetResourceCount(BasicResources.SEAWEED) < _cost)
            {
                // TODO - Can't buy warning
                return;
            }
            
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition());
            _auxBuild = auxGo.GetComponent<BuildingComponent>();

            _selectedHexTile.SetBuilding(_auxBuild);
            OnStructureBuild?.Invoke(_auxBuild, _selectedHexTile);
           

            //FOW
            _selectedHexTile.Reveal(_auxBuild.visionRange, 0);

            _selectedHexTile = null;
        }

        #region Event Systems Method
        
        public void OnPointerEnter(PointerEventData eventData) {
            transform.DOScale(new Vector3(1.1f, 1.1f, 1.1f), 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.OutSine);
        }

        public void OnPointerExit(PointerEventData eventData) {
            transform.DOKill();
            transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        }

        public void OnPointerDown(PointerEventData eventData) {
            if (!_selectedHexTile.TileEmpty()) return;
            
            BuildStructure();
        }

        #endregion

    }
    
}
