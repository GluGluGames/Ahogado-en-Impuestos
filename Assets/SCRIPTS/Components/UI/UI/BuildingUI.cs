using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private BuildButton[] _buttons;
        private bool _open;

        public Action OnMenuClose;
    
        private void Start() {
            CloseButton.onClick.AddListener(() => { Close(null); });

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
                OnMenuClose += tile.DeselectTile;
                tile.OnHexDeselect += () => Close();
            }

            _buttons = GetComponentsInChildren<BuildButton>();

            foreach(BuildButton button in _buttons)
                button.OnStructureBuild += Close;
            

            _open = false;
            transform.position = new Vector3(0, -400f, 0);
        }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard) {
                return; 
            }
            
            transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = true;
        }

        private void Close(BuildingComponent aux = null) {
            if (!_open) return;
            
            transform.DOMove(new Vector3(0f, -400, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = false;
            OnMenuClose?.Invoke();
        }
    }
}
