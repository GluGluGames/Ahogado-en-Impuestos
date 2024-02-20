using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using GGG.Components.Buildings;
using GGG.Components.HexagonalGrid;
using GGG.Components.Scenes;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class TilesSerialization : MonoBehaviour
    {
        [Serializable]
        private class TileData {
            public TileType Type;
            public bool IsEmpty;
            public int BuildId;
        }
        
        public static Action<BuildingComponent, HexTile> OnBuildingTileLoaded;
        
        public void SaveTilesState() 
        {
            if (SceneManagement.InMiniGameScene()) return;

            List<HexTile> tiles = TileManager.Instance.GetHexTiles();
            TileData[] saveData = new TileData[tiles.Count];
            string filePath = Path.Combine(Application.persistentDataPath, "tiles_data.json");
            int i = 0;

            foreach (HexTile tile in tiles) {
                TileData data = new()
                {
                    Type = tile.GetTileType(),
                    IsEmpty = tile.TileEmpty()
                };

                if (tile.GetTileType() == TileType.Build) data.BuildId = tile.GetCurrentBuilding().Id();
                
                saveData[i] = data;
                i++;
            }

            string jsonData = SerializationManager.EncryptDecrypt(JsonHelper.ToJson(saveData, true));
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadTilesState()
        {
            List<BuildingComponent> builds = BuildingManager.Instance.GetBuildings();
            string filePath = Path.Combine(Application.persistentDataPath, "tiles_data.json");
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
            else data = SerializationManager.EncryptDecrypt(File.ReadAllText(filePath));

            TileData[] tilesData = JsonHelper.FromJson<TileData>(data);
            List<HexTile> tiles = TileManager.Instance.GetHexTiles();
            int i = 0, j = 0;

            foreach (HexTile tile in tiles)
            {
                tile.SetTileType(tilesData[i].Type);
                if (!tilesData[i].IsEmpty)
                {
                    BuildingComponent build = builds.Find((x) => x.Id() == tilesData[i].BuildId);

                    tile.SetBuilding(build);
                    tile.Reveal(build.VisionRange(), 0);
                    OnBuildingTileLoaded?.Invoke(build, tile);
                    j++;
                }

                i++;
            }
        }
    }
}