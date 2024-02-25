using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Core;
using GGG.Input;

using UnityEngine;
using DG.Tweening;
using System;

namespace GGG.Components.UI.Buildings
{
    public class BuildingUI : MonoBehaviour
    {
        private BuildingCloseButton _closeButton;
        
        private InputManager _input;
        private GameManager _gameManager;
        
        private GameObject _viewport;
        private BuildButton[] _buttons;
        
        private bool _open;
        private int _currentPanel;
        
        public static Action<HexTile> OnUiOpen;
        public static Action OnUiClose;

        private void Awake()
        {
            _closeButton = GetComponentInChildren<BuildingCloseButton>();
            _closeButton.OnCloseButton += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * 0.5f, Screen.width * -0.5f, 0);
        }

        private void Start() {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
        }
        
        private void OnEnable()
        {
            HexTile[] tiles = FindObjectsOfType<HexTile>();
            foreach (HexTile tile in tiles) tile.OnHexSelect += Open;
            
            _buttons = GetComponentsInChildren<BuildButton>(true);
            foreach (BuildButton button in _buttons) button.OnStructureBuild += Close;
        }

        private void Update() {
            if (!_open) return;
            if (!_viewport.activeInHierarchy) _viewport.SetActive(true);
            if (_input.Escape() && !_gameManager.OnTutorial()) Close();
        }

        private void OnDisable()
        {
            _closeButton.OnCloseButton -= OnCloseButton;
            
            HexTile[] tiles = FindObjectsOfType<HexTile>();
            foreach (HexTile tile in tiles) tile.OnHexSelect -= Open;
            
            _buttons = GetComponentsInChildren<BuildButton>(true);
            foreach (BuildButton button in _buttons) button.OnStructureBuild -= Close;
        }

        private void Open(HexTile tile) {
            if (_open || tile.GetTileType() != TileType.Standard || !tile.TileEmpty()) return; 
            _open = true;
            
            _viewport.SetActive(true);
            _viewport.transform.DOMoveY(0, 0.75f).SetEase(Ease.InCubic);
            OnUiOpen?.Invoke(tile);
            _gameManager.OnUIOpen();
        }

        private void Close(BuildingComponent x, HexTile y) => Close();
        
        private void OnCloseButton() {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;
            
            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveY(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => {
                _viewport.SetActive(false);
                _open = false;
                
                _gameManager.OnUIClose();
            };
            
            OnUiClose?.Invoke();
        }
    }
}
