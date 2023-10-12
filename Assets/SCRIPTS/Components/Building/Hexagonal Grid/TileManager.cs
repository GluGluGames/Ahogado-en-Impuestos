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

        private Vector3Int _playerPosCube;
        private PlayerMovement _playerMovement;
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

            _playerMovement = PlayerMovement.Instance;
            _playerPosCube = tile.cubeCoordinate;
            _playerMovement.transform.position = tile.transform.position + new Vector3(0.0f, 1f, 0.0f);
            _playerMovement.currentTile = tile;

            HexTile[] tiles = FindObjectsOfType<HexTile>();

            foreach (HexTile tileAux in tiles)
                tileAux.OnHexSelect += () =>
                {
                    _path = Pathfinder.FindPath(_playerMovement.currentTile, tileAux);
                };
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

        public void OnDrawGizmos()
        {
            if (_path != null)
            {
                foreach (HexTile tile in _path)
                {
                    Gizmos.DrawCube(tile.transform.position + new Vector3(0f, 1f, 0f), new Vector3(0.5f, 0.5f, 0.5f));
                }
            }
        }
    }
}