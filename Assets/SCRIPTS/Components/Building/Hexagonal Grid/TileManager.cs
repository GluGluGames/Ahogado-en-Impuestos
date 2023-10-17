
using Codice.Client.Common;
using GGG.Components.Ticks;
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
        [SerializeField] private GameObject _FOWPrefab;
        [SerializeField] private bool FOWActive = false;

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

            // Register every hex playerSpawnTile
            foreach (HexTile hexTile in hexTiles)
            {
                RegisterTile(hexTile);
                if(FOWActive) { AddFogOfWarTile(hexTile); }

            }

            foreach (HexTile hexTile in hexTiles)
            {
                List<HexTile> neighbours = GetNeighbours(hexTile);
                hexTile.neighbours = neighbours;
            }

            // Put the player somewhere

<<<<<<< HEAD
            HexTile playerSpawnTile = GetRandomHex();
            while (playerSpawnTile.tileType != HexTileGenerationSettings.TileType.Cliff)
=======
            HexTile tile = GetRandomHex();
            while (tile.tileType != TileType.Cliff)
>>>>>>> origin/main
            {
                playerSpawnTile = GetRandomHex();
            }

            if (Player) {
                _playerPosCube = playerSpawnTile.cubeCoordinate;
                Player.transform.position = playerSpawnTile.transform.position + new Vector3(0.0f, 1f, 0.0f);
                PlayerPosition.CurrentTile = playerSpawnTile;

                foreach (HexTile tileAux in hexTiles)
<<<<<<< HEAD
                tileAux.OnHexSelect += () =>
                {
                    _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                    _path.Reverse();
                    PlayerPosition.CurrentPath = _path;
                };

                //FOW
                RevealTile(playerSpawnTile, 2);
=======
                    tileAux.OnHexSelect += (x) =>
                    {
                        _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                        _path.Reverse();
                        PlayerPosition.CurrentPath = _path;
                    };
>>>>>>> origin/main
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
        
        /// <summary>
        ///  Add prefab which acts as FOW. Also we make the basic hex invisible by changing its layer. We modify this when we eliminate the FOW.
        /// </summary>
        /// <param name="tile"></param>
        private void AddFogOfWarTile(HexTile tile)
        {
            GameObject fow = Instantiate(_FOWPrefab, transform);
            fow.name = $"FOW C{tile.offsetCoordinate.x}, R{tile.offsetCoordinate.y}";
            fow.transform.position = tile.transform.position;
            tile.fow = fow;
            tile.gameObject.layer = 7; // Layer is "Hidden"

            tile.transform.GetChild(0).gameObject.layer = 7;

        }

        /// <summary>
        /// Reveals certain number of tiles on a circle. Range is defined by <paramref name="depth"/>
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="depth"></param>
        public void RevealTile(HexTile tile, int depth)
        {
            tile.Reveal(depth, 0);

        }
    }
}