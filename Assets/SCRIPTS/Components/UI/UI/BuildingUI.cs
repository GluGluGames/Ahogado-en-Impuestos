using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using GGG.Components.Core;
using GGG.Input;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private InputManager _input;
        private GameObject _viewport;
        private BuildButton[] _buttons;
        private bool _open;
        private HexTile _selectedTile;

        public static Action OnUiOpen;

        private void Start() {
            _input = InputManager.Instance;
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            CloseButton.onClick.AddListener(Close);
            CloseButton.gameObject.SetActive(false);

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
            }

            _buttons = GetComponentsInChildren<BuildButton>(true);

            foreach (BuildButton button in _buttons) {
                button.Initialize();
                button.OnStructureBuild += (x, y) => Close();
            }


            _open = false;
            transform.position = new Vector3(0, -400f, 0);
        }

        private void Update() {
            if (!_open || !_input.Escape()) return;

            Close();
        }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard || !tile.TileEmpty()) {
                return; 
            }
            
            _selectedTile = tile;
            _open = true;
            _viewport.SetActive(true);
            CloseButton.gameObject.SetActive(true);
            GameManager.Instance.OnUIOpen();
            
            transform.DOMove(new Vector3(0f, 0f, 0f), 0.5f, true).SetEase(Ease.InOutSine);
            OnUiOpen?.Invoke();
        }

        private void Close() {
            if (!_open) return;
            
            transform.DOMove(new Vector3(0f, -400, 0f), 0.5f, true).SetEase(Ease.InOutSine).onComplete += () => {
                _viewport.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };
            _selectedTile.DeselectTile();
            _selectedTile = null;
            GameManager.Instance.OnUIClose();
            _open = false;
        }
    }
}
