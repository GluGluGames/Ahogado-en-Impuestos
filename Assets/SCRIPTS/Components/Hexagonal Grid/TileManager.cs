using GGG.Shared;
using GGG.Components.Buildings;
using GGG.Components.Core;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Classes.Buildings;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace GGG.Components.HexagonalGrid
{
    public class TileManager : MonoBehaviour
    {
        #region Singleton

            public static TileManager Instance;
            
            private void Awake()
            {
                if(!Instance)
                    Instance = this;
            }

        #endregion
        
        [SerializeField] private GameObject HighlightPrefab;
        [SerializeField] private GameObject FowPrefab; 
        [SerializeField] private bool FowActive;

        private Dictionary<Vector3Int, HexTile> _tilesDic;
        private HexTile _selectedTile;
        private List<HexTile> _tiles;

        [Serializable]
        private class TileData {
            public TileType Type;
            public bool IsEmpty;
            public int BuildId;
        }

        public static Action<BuildingComponent, HexTile> OnBuildingTileLoaded;

        #region Player Info
        
        [SerializeField] private Transform Player;
        private List<HexTile> _path;

        #endregion Player Info

        #region Unity Events

        private void Start()
        {
            _tilesDic = new Dictionary<Vector3Int, HexTile>();
            _tiles = GetComponentsInChildren<HexTile>().ToList();

            // Register every hex playerSpawnTile
            foreach (HexTile hexTile in _tiles)
            {
                RegisterTile(hexTile);
                if (FowActive) { AddFogOfWarTile(hexTile); }
            }

            foreach (HexTile tile in _tiles)
            {
                List<HexTile> neighbours = GetNeighbours(tile);
                tile.neighbours = neighbours;
            }
            
            if (Player) StartCoroutine(InitializePlayer());
            else RevealTile(_tiles[_tiles.Count / 2], 3);
        }

        private void OnDisable() {
            foreach (HexTile tileAux in _tiles) tileAux.OnHexSelect -= InitializePath;
            SaveTilesState();
        }

        #endregion

        #region Getters&Setters

        public GameObject GetHighlightPrefab() => HighlightPrefab;
        private void RegisterTile(HexTile tile) => _tilesDic.Add(tile.cubeCoordinate, tile);

        public List<HexTile> GetHexTiles() => _tiles;
        public void SelectTile(HexTile tile) => _selectedTile = tile;

        public HexTile GetSelectedTile() => _selectedTile; 

        public HexTile GetRandomHex()
        {
            int rand = Random.Range(0, _tiles.Count);
            return _tiles.ElementAt(rand);
        }
        
        private List<HexTile> GetNeighbours(HexTile tile)
        {
            List<HexTile> neighbours = new();

            Vector3Int[] neighbourCoords = 
            {
                new (0, -1, 1),
                new (1, -1 , 0),
                new (1, 0, -1),
                new (0, 1, -1),
                new (-1, 1, 0),
                new (-1, 0, 1)
            };

            foreach (Vector3Int neighbourCoord in neighbourCoords)
            {
                Vector3Int tileCoord = tile.cubeCoordinate;
                
                if (_tilesDic.TryGetValue(tileCoord + neighbourCoord, out HexTile neighbour))
                    neighbours.Add(neighbour);
            }
            
            return neighbours;
        }

        #endregion

        #region Methods

        private IEnumerator InitializePlayer()
        {
            HexTile playerSpawnTile = GetRandomHex();
            while (playerSpawnTile.tileType == TileType.Mountain && playerSpawnTile.selectable == false)
            {
                playerSpawnTile = GetRandomHex();
            }
                
            PlayerPosition.currentPath = new List<HexTile>();
            Player.position = playerSpawnTile.transform.position + new Vector3(0.0f, PlayerPosition.heightOffset, 0.0f);
            PlayerPosition.CurrentTile = playerSpawnTile;

            PlayerPosition.PlayerPos = playerSpawnTile.cubeCoordinate;

            // yield return new WaitWhile(() => !SceneManager.GetSceneByBuildIndex((int)SceneIndexes.MINIGAME_LEVEL1).isLoaded);
            yield return null;

            foreach (HexTile tileAux in _tiles)
                tileAux.OnHexSelect += InitializePath;

            // FOW
            RevealTile(playerSpawnTile, 2);
        }

        private void InitializePath(HexTile tile)
        {
            _path = Pathfinder.FindPath(PlayerPosition.CurrentTile, tile);
            _path.Reverse();
            PlayerPosition.currentPath = _path;
        }

        public void DestroyBuilding(BuildingComponent build)
        {
            HexTile buildTile = _tiles.Find((x) => x.GetCurrentBuilding() == build);
            buildTile.DestroyBuilding();
        }

        /// <summary>
        ///  Add prefab which acts as FOW. Also we make the basic hex invisible by changing its layer. We modify this when we eliminate the FOW.
        /// </summary>
        /// <param name="tile"></param>
        private void AddFogOfWarTile(HexTile tile)
        {
            GameObject fow = Instantiate(FowPrefab, transform);
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
            if(FowActive) tile.Reveal(depth, 0);
        }

        #endregion
        
        #region Data persistence

        public void SaveTilesState() {
            if (GameManager.Instance.GetCurrentTutorial() is Tutorials.BuildTutorial or Tutorials.InitialTutorial)
                return;
            
            TileData[] saveData = new TileData[_tiles.Count];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "tiles_data.json");
            int i = 0;

            foreach (HexTile tile in _tiles) {
                TileData data = new()
                {
                    Type = tile.GetTileType(),
                    IsEmpty = tile.TileEmpty()
                };

                if (tile.GetTileType() == TileType.Build)
                {
                    print(tile.GetCurrentBuilding());
                    data.BuildId = tile.GetCurrentBuilding().Id();
                }

                saveData[i] = data;
                i++;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadTilesState()
        {
            List<BuildingComponent> builds = BuildingManager.Instance.GetBuildings();
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "tiles_data.json");
            string data;

            if (!File.Exists(filePath))
            {
                yield break;
            }
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else {
                data = File.ReadAllText(filePath);
            }
            
            TileData[] tiles = JsonHelper.FromJson<TileData>(data);
            int i = 0, j = 0;

            foreach (HexTile tile in _tiles)
            {
                tile.SetTileType(tiles[i].Type);
                if (!tiles[i].IsEmpty)
                {
                    BuildingComponent build = builds.Find((x) => x.Id() == tiles[i].BuildId);
                    
                    tile.SetBuilding(build);
                    tile.Reveal(build.VisionRange(), 0);
                    OnBuildingTileLoaded?.Invoke(build, tile);
                    j++;
                }

                i++;
            }
        }
        
        #endregion
    }
}