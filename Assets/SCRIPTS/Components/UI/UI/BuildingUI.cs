using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private GameObject _viewport;
        private BuildButton[] _buttons;
        private bool _open;
        private HexTile _selectedTile;
        
        public Action OnMenuOpen;
    
        private void Start() {
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            CloseButton.onClick.AddListener(Close);
            CloseButton.gameObject.SetActive(false);

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
            }

            _buttons = GetComponentsInChildren<BuildButton>(true);

            foreach(BuildButton button in _buttons)
                button.OnStructureBuild += (x) => Close();
            

            _open = false;
            transform.position = new Vector3(0, -400f, 0);
        }
        
        public bool IsOpen() { return _open; }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard) {
                return; 
            }
            
            _selectedTile = tile;
            _open = true;
            _viewport.SetActive(true);
            CloseButton.gameObject.SetActive(true);
            transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            OnMenuOpen?.Invoke();
        }

        public void Close() {
            if (!_open) return;
            
            transform.DOMove(new Vector3(0f, -400, 0f), 0.5f, true).SetEase(Ease.InOutSine).onComplete += () => {
                _viewport.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };
            _selectedTile.DeselectTile();
            _selectedTile = null;
            _open = false;
        }
    }
}
