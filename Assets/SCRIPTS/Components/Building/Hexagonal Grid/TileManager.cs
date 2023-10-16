using Codice.Client.Common;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GGG.Components.Buildings
{
    public class TileManager : MonoBehaviour
    {
        public static TileManager instance;
        public GameObject highlightPrefab;
        private Dictionary<Vector3Int, HexTile> _tilesDic;
        private HexTile _selectedTile;

        #region playerInfo

        [SerializeField] private Transform Player;
        private Vector3Int _playerPosCube;
        private List<HexTile> _path;

        #endregion playerInfo

        private void Awake()
        {
            instance = this;
            _tilesDic = new Dictionary<Vector3Int, HexTile>();

            HexTile[] hexTiles = gameObject.GetComponentsInChildren<HexTile>();

            // Register every hex tile
            foreach (HexTile hexTile in hexTiles)
            {
                RegisterTile(hexTile);
            }

            foreach (HexTile hexTile in hexTiles)
            {
                List<HexTile> neighbours = GetNeighbours(hexTile);
                hexTile.neighbours = neighbours;
            }

            // Put the player somewhere

            HexTile tile = GetRandomHex();
            while (tile.tileType != HexTileGenerationSettings.TileType.Cliff)
            {
                tile = GetRandomHex();
            }

            if (Player) {
                _playerPosCube = tile.cubeCoordinate;
                Player.transform.position = tile.transform.position + new Vector3(0.0f, 1f, 0.0f);
                PlayerPosition.CurrentTile = tile;

                foreach (HexTile tileAux in hexTiles)
                tileAux.OnHexSelect += () =>
                {
                    _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                    _path.Reverse();
                    PlayerPosition.CurrentPath = _path;
                };
            }
        }

        public void RegisterTile(HexTile tile)
        {
            _tilesDic.Add(tile.cubeCoordinate, tile);
        }

        private List<HexTile> GetNeighbours(HexTile tile)
        {
            List<HexTile> neighbours = new List<HexTile>();

            Vector3Int[] neighbourCoords = new Vector3Int[]
            {
            new Vector3Int(0, -1, 1),
            new Vector3Int(1, -1 , 0),
            new Vector3Int(1, 0, -1),
            new Vector3Int(0, 1, -1),
            new Vector3Int(-1, 1, 0),
            new Vector3Int(-1, 0, 1),
            };

            foreach (Vector3Int neighbourCoord in neighbourCoords)
            {
                Vector3Int tileCoord = tile.cubeCoordinate;

                if (_tilesDic.TryGetValue(tileCoord + neighbourCoord, out HexTile neighbour))
                {
                    neighbours.Add(neighbour);
                }
            }
            return neighbours;
        }

        public void SelectTile(HexTile tile)
        { _selectedTile = tile; }

        public HexTile GetSelectedTile()
        { return _selectedTile; }

        private HexTile GetRandomHex()
        {
            int rand = Random.Range(0, _tilesDic.Count);
            return _tilesDic.ElementAt(rand).Value;
        }
        
    }
}