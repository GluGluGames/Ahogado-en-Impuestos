using GGG.Components.Buildings;

using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Collections.Generic;
using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Input;

namespace GGG.Components.UI {
    public class BuildingUI : MonoBehaviour {
        [SerializeField] private Button CloseButton;

        private InputManager _input;
        private BuildingManager _buildingManager;
        private GameManager _gameManager;
        private GameObject _viewport;
        private BuildButton[] _buttons;
        private bool _open;
        private HexTile _selectedTile;

        public static Action OnUiOpen;

        private void Start() {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            _buildingManager = BuildingManager.Instance;
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            
            CloseButton.onClick.AddListener(OnCloseButton);

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
            }

            _buttons = GetComponentsInChildren<BuildButton>(true);

            foreach (BuildButton button in _buttons) {
                button.Initialize(_buildingManager);
                button.OnStructureBuild += (x, y) => Close();
            }

            _open = false;
            _viewport.transform.position = new Vector3(Screen.width * 0.5f, 0, 0);
        }

        private void Update() {
            if (!_open ) return;

            if (!_viewport.activeInHierarchy)
            {
                _viewport.SetActive(true);
            }
            
            if (!_input.Escape()) return;
            Close();
        }

        private void CheckBuildings()
        {
            foreach(BuildButton button in _buttons)
                button.CheckUnlockState();
        }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard || !tile.TileEmpty()) {
                return; 
            }
            
            _viewport.transform.DOMoveY(Screen.height * 0.5f, 0.75f).SetEase(Ease.InCubic).onComplete += () =>
            {
                _open = true;
            };
            
            _selectedTile = tile;
            _viewport.SetActive(true);
            CheckBuildings();
            OnUiOpen?.Invoke();
            _gameManager.OnUIOpen();
        }
        
        private void OnCloseButton() {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }
        
        public void Close()
        {
            _viewport.transform.DOMoveY(0, 0.75f).SetEase(Ease.OutCubic).onComplete += () => {
                _viewport.SetActive(false);
                _open = false;
                _gameManager.OnUIClose();
            };
            
            _selectedTile.DeselectTile();
            _selectedTile = null;
            
            
        }
    }
}
