using System;
using System.Collections.Generic;
using System.Linq;
using GGG.Components.HexagonalGrid;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Input;
using GGG.Shared;

using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI.TileClean
{
    public class TileCleanUI : MonoBehaviour
    {
        private TileCleanButton _cleanButton;
        private TileCleanCloseButton _closeButton;
        private InputManager _input;
        private GameManager _gameManager;
        private GameObject _viewport;
        private List<HexTile> _tiles;

        private bool _open;
        private int _tilesClean;

        public Action<HexTile> OnUiOpen;
        public Action OnUiClose;

        private void Awake()
        {
            _cleanButton = GetComponentInChildren<TileCleanButton>(true);
            _cleanButton.OnClean += Close;
            
            _closeButton = GetComponentInChildren<TileCleanCloseButton>(true);
            _closeButton.OnCloseButton += OnCloseButton;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
        }

        private void Start()
        {
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            
            Initialize();
        }

        private void Update() {
            if (!_open || !_input.Escape() || _gameManager.OnTutorial()) return;

            Close();
        }
        
        private void OnDisable()
        {
            foreach (HexTile tile in _tiles) tile.OnHexSelect -= Open;
        }

        private void Initialize()
        {
            _tiles = FindObjectsOfType<HexTile>().ToList();
            _tilesClean = PlayerPrefs.GetInt("TilesClean", 0);

            foreach (HexTile tile in _tiles) {
                tile.OnHexSelect += Open;
                if (_tilesClean > 0) tile.SetClearCost(Mathf.RoundToInt(tile.GetClearCost() + _tilesClean * 25));
            }
        }

        private void Open(HexTile tile)
        {
            if (_open || tile.GetTileType() is TileType.Standard or TileType.Build)
                return;
            _open = true;

            _viewport.SetActive(true);
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
            
            _gameManager.OnUIOpen();
            OnUiOpen?.Invoke(tile);
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => {
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                _open = false;
            };

            OnUiClose?.Invoke();
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen()) return;

            Close();
        }
    }
}
