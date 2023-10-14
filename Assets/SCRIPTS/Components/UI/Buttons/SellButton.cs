using GGG.Components.Buildings;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GGG.Components.UI {
    public class SellButton : MonoBehaviour {

        private HexTile _selectedHexTile;
        public Action onStructureSold;


        void Start()
        {   
            HexTile[] hexTiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tile in hexTiles)
            {
                tile.OnHexSelect += () => {
                    _selectedHexTile = tile;

                    print(_selectedHexTile.name);
                    print(_selectedHexTile.TileEmpty());
                    if (!_selectedHexTile.TileEmpty())
                    {
                        print(tile.name);
                    }
                };
            }
        }
    }
}
