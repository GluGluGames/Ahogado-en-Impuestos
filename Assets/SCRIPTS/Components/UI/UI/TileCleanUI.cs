using DG.Tweening;
using GGG.Components.Buildings;
using System;
using GGG.Components.Player;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.UI
{
    public class TileCleanUI : MonoBehaviour
    {
        [SerializeField] private Button CleanButton;
        [SerializeField] private Button CloseButton;
        [SerializeField] private TMP_Text CostAmountText;

        private PlayerManager _player;
        private Resource _cleanResource;
        private Transform _transform;
        private GameObject _viewport;
        private HexTile _selectedTile;

        private bool _open;
        
        public Action OnMenuOpen;

        private void Start()
        {
            _player = PlayerManager.Instance;
            _cleanResource = _player.GetMainResource();
            
            CleanButton.onClick.AddListener(CleanTile);
            CloseButton.onClick.AddListener(Close);
            CloseButton.gameObject.SetActive(false);

            _transform = transform;
            _transform.position = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 360);

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
            }
        }
        
        public bool IsOpen() { return _open; }

        private void CleanTile()
        {
            if (_player.GetResourceCount(_cleanResource.GetName()) < _selectedTile.GetClearCost())
            {
                // TODO - Can't clear tile warning
                return;
            }
            
            _selectedTile.SetTileType(TileType.Standard);
            _player.AddResource(_cleanResource.GetName(), -_selectedTile.GetClearCost());
            Close();
        }

        private void Open(HexTile tile)
        {
            if (_open || tile.GetTileType() == TileType.Standard)
                return;

            _viewport.SetActive(true);
            _selectedTile = tile;
            CostAmountText.SetText(_selectedTile.GetClearCost().ToString());
            _open = true;
            CloseButton.gameObject.SetActive(true);
            OnMenuOpen?.Invoke();

            _transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f), 0.1f).SetEase(Ease.InCubic);
        }

        public void Close()
        {
            if (!_open) return;

            _transform.DOMove(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f - 360), 0.1f).SetEase(Ease.InCubic).onComplete += () => {
                _viewport.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };

            _selectedTile.DeselectTile();
            _selectedTile = null;
            _open = false;
        }
    }
}
