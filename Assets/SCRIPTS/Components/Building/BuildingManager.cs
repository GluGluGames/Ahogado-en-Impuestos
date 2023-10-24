using System;
using System.Collections;
using System.IO;
using GGG.Classes.Buildings;
using UnityEngine;
using UnityEngine.Networking;

namespace GGG.Components.Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance;

        [Serializable]
        private class BuildingData
        {
            public Vector3 Position;
            public Building Building;
        }

        private void Awake()
        {
            if (Instance != null) return;

            Instance = this;
        }

        private void OnEnable() {
            StartCoroutine(LoadBuildings());
        }

        private void OnDisable() {
            SaveBuildings();
        }

        public void SaveBuildings() {
            BuildingComponent[] buildings = GetComponentsInChildren<BuildingComponent>();
            BuildingData[] saveData = new BuildingData[buildings.Length];
            int i = 0;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "buildings_data.json");

            foreach (BuildingComponent build in buildings) {
                BuildingData data = new BuildingData();
                
                data.Position = build.GetPosition();
                data.Building = build.GetBuild();

                saveData[i] = data;
                i++;
            }

            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadBuildings()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "buildings_data.json");
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
                BuildingData[] buildings = JsonHelper.FromJson<BuildingData>(data);
                foreach (BuildingData build in buildings) {
                    GameObject go = build.Building.Spawn(build.Position, transform);
                }
            }
        }
    }
}
