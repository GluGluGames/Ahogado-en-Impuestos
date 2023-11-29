using System;
using System.Collections;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Shared;
using System.Linq.Expressions;
using GGG.Classes.Buildings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
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

        private HexTile[] _tiles;

        [Serializable]
        private class TileData {
            public TileType Type;
            public bool IsEmpty;
        }

        public static Action<BuildingComponent[]> OnTilesStateLoaded;

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
                if (FOWActive) { AddFogOfWarTile(hexTile); }

            }

            foreach (HexTile hexTile in hexTiles)
            {
                List<HexTile> neighbours = GetNeighbours(hexTile);
                hexTile.neighbours = neighbours;
            }

            // Put the player somewhere


            HexTile playerSpawnTile = GetRandomHex();
            while (playerSpawnTile.tileType != TileType.Cliff)
            {
                playerSpawnTile = GetRandomHex();
            }


            // JUGADOR
            GameObject aux = GameObject.Find("PlayerModel");
            if(aux != null)
            {
                Player = aux.transform;
            }
            if (Player) {
                PlayerPosition.currentPath = new List<HexTile>();
                _playerPosCube = playerSpawnTile.cubeCoordinate;
                Player.position = playerSpawnTile.transform.position + new Vector3(0.0f, 1f, 0.0f);
                PlayerPosition.CurrentTile = playerSpawnTile;

                PlayerPosition.PlayerPos = playerSpawnTile.cubeCoordinate;

                foreach (HexTile tileAux in hexTiles)
                    tileAux.OnHexSelect += (tileAux) =>
                    {
                        if (SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME).isLoaded)
                        {
                            _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                            _path.Reverse();
                            PlayerPosition.currentPath = _path;
                        }
                        _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                        _path.Reverse();
                        PlayerPosition.currentPath = _path;
                    };

                // FOW
                RevealTile(playerSpawnTile, 2);
            }
            else // ONLY do on main scene. Where there is no player...
            {
                RevealTile(hexTiles[hexTiles.Length / 2], 3);
            }
        }

        private void OnEnable()
        {
            BuildingManager.OnBuildsLoad += OnBuildsLoad;
        }

        private void OnDisable() {
            SaveTilesState();
        }

        private void OnDestroy()
        {
            HexTile[] hexTiles = gameObject.GetComponentsInChildren<HexTile>();
            foreach (HexTile tileAux in hexTiles)
                tileAux.OnHexSelect -= (tileAux) =>
                {
                    Debug.Log(PlayerPosition.CurrentTile);
                    _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tileAux);
                    _path.Reverse();
                    PlayerPosition.currentPath = _path;
                };

            BuildingManager.OnBuildsLoad -= OnBuildsLoad;
        }

        private void OnBuildsLoad(BuildingComponent[] builds)
        {
            if (builds == null)
            {
                OnTilesStateLoaded?.Invoke(null);
                return;
            }
            
            StartCoroutine(LoadTilesState(builds));
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

        public HexTile GetRandomHex()
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

            tile.gameObject.transform.GetChild(0).gameObject.layer = 7;
            for (int i = 0; i < tile.gameObject.transform.GetChild(0).childCount; i++)
                tile.gameObject.transform.GetChild(0).gameObject.transform.GetChild(i).gameObject.layer = 7;
        }

        /// <summary>
        /// Reveals certain number of tiles on a circle. Range is defined by <paramref name="depth"/>
        /// </summary>
        /// <param name="tile"></param>
        /// <param name="depth"></param>
        public void RevealTile(HexTile tile, int depth)
        {
            if(FOWActive)
            {
                tile.Reveal(depth, 0);
            }

        }
        
        // Data persistence

        private void SaveTilesState() {
            _tiles = GetComponentsInChildren<HexTile>();
            TileData[] saveData = new TileData[_tiles.Length];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "tiles_data.json");
            int i = 0;

            foreach (HexTile tile in _tiles) {
                TileData data = new TileData();

                data.Type = tile.GetTileType();
                data.IsEmpty = tile.TileEmpty();
                saveData[i] = data;
                i++;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadTilesState(BuildingComponent[] builds) {
            _tiles = GetComponentsInChildren<HexTile>();
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "tiles_data.json");
#if UNITY_EDITOR
            filePath = "file://" + filePath;
#endif
            string data;
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }

            if (!string.IsNullOrEmpty(data)) {
                TileData[] tiles = JsonHelper.FromJson<TileData>(data);
                int i = 0, j = 0;
                
                foreach (HexTile tile in _tiles) {
                    tile.SetTileType(tiles[i].Type);
                    if (!tiles[i].IsEmpty)
                    {
                        tile.SetBuilding(builds[j]);
                        tile.Reveal(builds[j].GetVisionRange(), 0);
                        j++;
                    }
                    
                    i++;
                }
                
                OnTilesStateLoaded?.Invoke(builds);
            }
            else {
                SaveTilesState();
            }
        }
    }
}