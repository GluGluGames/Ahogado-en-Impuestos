using UnityEngine;
using DG.Tweening;
using GGG.Components.Buildings;
using UnityEngine.UI;
using System;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private bool _open;

        public Action OnMenuClose;
    
        private void Start() {
            CloseButton.onClick.AddListener(Close);

            //Cell[] cells = FindObjectsOfType<Cell>();
            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
                OnMenuClose += tile.DeselectTile;
            }

            _open = false;
            transform.position = new Vector3(0, -400f, 0);
        }

        private void Open(BuildingComponent building) {
            if (_open || building) return;
            
            transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = true;
        }

        private void Close() {
            if (!_open) return;
            
            transform.DOMove(new Vector3(0f, -400, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            _open = false;
            OnMenuClose?.Invoke();
        }
    }
}
