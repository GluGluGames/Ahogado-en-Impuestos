using System;
using GGG.Components.HexagonalGrid;
using UnityEngine;

namespace GGG.Components.UI.TileClean
{
    public class TileCleanListener : MonoBehaviour
    {
        private TileCleanUI _ui;
        private TileCleanButton _button;
        private TileCleanResource _resource;
        private static HexTile _tile;

        private void Awake()
        {
            _ui = FindObjectOfType<TileCleanUI>();
            _button = GetComponentInChildren<TileCleanButton>();
            _resource = GetComponentInChildren<TileCleanResource>();

            _ui.OnUiOpen += Initialize;
            _ui.OnUiClose += Deinitialize;
        }

        private void OnDisable()
        {
            _ui.OnUiOpen -= Initialize;
            _ui.OnUiClose -= Deinitialize;
        }

        public static HexTile Tile() => _tile;

        private void Initialize(HexTile tile)
        {
            if (_tile) return;

            _tile = tile;
            _button.Initialize(tile);
            _resource.Initialize(tile);
        }

        private void Deinitialize()
        {
            _tile.DeselectTile();
            _tile = null;
            _button.Deinitialize();
        }
    }
}
