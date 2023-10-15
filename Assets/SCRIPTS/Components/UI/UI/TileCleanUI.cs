using DG.Tweening;
using GGG.Components.Buildings;
using System;
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

        private Transform _transform;
        private GameObject _viewport;
        private HexTile _selectedTile;

        private bool _open;

        private Action OnMenuClose;

        private void Start()
        {
            CleanButton.onClick.AddListener(CleanTile);
            CloseButton.onClick.AddListener(Close);
            CloseButton.gameObject.SetActive(false);

            _transform = transform;
            _transform.position = new Vector3(960, -360);

            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in tiles) {
                tile.OnHexSelect += Open;
                OnMenuClose += tile.DeselectTile;
            }
        }

        private void CleanTile()
        {
            _selectedTile.SetTileType(TileType.Standard);
            Close();
        }

        private void Open(HexTile tile)
        {
            if (_open || tile.GetTileType() == TileType.Standard)
                return;

            _viewport.SetActive(true);
            _selectedTile = tile;
            _open = true;
            CloseButton.gameObject.SetActive(true);

            _transform.DOMove(new Vector3(960, 540), 0.1f).SetEase(Ease.InCubic);
        }

        private void Close()
        {
            if (!_open) return;

            _transform.DOMove(new Vector3(960, -360), 0.1f).SetEase(Ease.InCubic).onComplete += () => {
                _viewport.SetActive(false);
                CloseButton.gameObject.SetActive(false);
            };

            _selectedTile = null;
            _open = false;
            OnMenuClose?.Invoke();
        }
    }
}
