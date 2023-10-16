using DG.Tweening;
using GGG.Classes.Buildings;
using TMPro;
using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace GGG.Components.UI {
    public class BuildButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler {
        [SerializeField] private Building BuildingInfo;
        [SerializeField] private TextMeshProUGUI BuildNameText;

        private HexTile _selectedHexTile;
        private BuildingComponent _auxBuild;

        public Action<BuildingComponent, HexTile> OnStructureBuild;

        private void Start() {
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += (x) => _selectedHexTile = tile;
            }

            BuildNameText.SetText(BuildingInfo.GetName());
        }

        private void BuildStructure() {
            GameObject auxGo = BuildingInfo.Spawn(_selectedHexTile.SpawnPosition());
            _auxBuild = auxGo.GetComponent<BuildingComponent>();

            _selectedHexTile.SetBuilding(_auxBuild);
            OnStructureBuild?.Invoke(_auxBuild, _selectedHexTile);
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
