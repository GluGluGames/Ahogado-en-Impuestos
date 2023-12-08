using System;
using GGG.Components.HexagonalGrid;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Input;
using GGG.Shared;

using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class TileCleanUI : MonoBehaviour
    {
        [SerializeField] private Button CleanButton;
        [SerializeField] private Button CloseButton;
        [SerializeField] private GameObject Container;

        private PlayerManager _player;
        private InputManager _input;
        private GameManager _gameManager;
        private Resource _cleanResource;
        private GameObject _viewport;
        private HexTile[] _tiles;
        private HexTile _selectedTile;

        private bool _open;
        private int _tilesClean;

        private TextMeshProUGUI _costAmountText;

        public Action OnUiOpen;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _input = InputManager.Instance;
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);
            
            Initialize();
        }

        private void Update() {
            if (!_open || !_input.Escape() || _gameManager.OnTutorial()) return;

            Close();
        }
        
        private void OnDisable()
        {
            _player.OnPlayerInitialized -= OnPlayerInitialized;
            foreach (HexTile tile in _tiles)
                tile.OnHexSelect -= Open;
            
            CleanButton.onClick.RemoveAllListeners();
            CloseButton.onClick.RemoveAllListeners();
        }

        private void Initialize()
        {
            _player.OnPlayerInitialized += OnPlayerInitialized;
            
            CleanButton.onClick.AddListener(CleanTile);
            CloseButton.onClick.AddListener(OnCloseButton);
            
            _costAmountText = Container.GetComponentInChildren<TextMeshProUGUI>();

            _tiles = FindObjectsOfType<HexTile>();
            _tilesClean = PlayerPrefs.HasKey("TilesClean") ? PlayerPrefs.GetInt("TilesClean") : 0;

            foreach (HexTile tile in _tiles) {
                tile.OnHexSelect += Open;
                if (_tilesClean > 0)
                    tile.SetClearCost(Mathf.RoundToInt(tile.GetClearCost() + _tilesClean * 25));
            }
        }

        private void OnPlayerInitialized()
        {
            _cleanResource = _player.GetResource("Seaweed");
            Container.GetComponentInChildren<Image>().sprite = _cleanResource.GetSprite();
        }

        private void CleanTile()
        {
            _selectedTile.SetTileType(TileType.Standard);
            _player.AddResource(_cleanResource.GetKey(), -_selectedTile.GetClearCost());
            foreach (HexTile tile in _tiles)
                tile.SetClearCost(Mathf.RoundToInt(tile.GetClearCost() + 25));
            PlayerPrefs.SetInt("TilesClean", _tilesClean++);
            PlayerPrefs.Save();
            
            Close();
        }

        private void CheckCleanState()
        {
            bool condition = _player.GetResourceCount(_cleanResource.GetKey()) < _selectedTile.GetClearCost();

            CleanButton.interactable = !condition;
            CleanButton.image.color = condition ? new Color(0.81f, 0.84f, 0.81f, 0.9f) : Color.white;
        }

        private void Open(HexTile tile)
        {
            if (_open || tile.GetTileType() is TileType.Standard or TileType.Build)
                return;

            _open = true;
            _selectedTile = tile;
            
            _costAmountText.SetText(_selectedTile.GetClearCost().ToString());
            CheckCleanState();

            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            OnUiOpen?.Invoke();
            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        public void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () => {
                _viewport.SetActive(false);
                _gameManager.OnUIClose();
                _open = false;
            };

            _selectedTile.DeselectTile();
            _selectedTile = null;
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.GetCurrentTutorial() == Tutorials.BuildTutorial) return;

            Close();
        }
    }
}
