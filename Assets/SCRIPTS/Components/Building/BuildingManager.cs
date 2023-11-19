using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Shared;
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
            public int Level;
        }

        private PlayerManager _player;
        private GameManager _gameManager;
        private List<BuildingComponent> _buildings = new();
        private readonly Dictionary<Building, int> _buildingsCount = new();
        private const string _EXIT_TIME = "ExitTime";
        
        public static Action<BuildingComponent[]> OnBuildsLoad;

        private void Awake()
        {
            if (Instance != null) return;

            Instance = this;
        }

        private void Update() {
            if (_buildings.Count == 0 || _gameManager.GetCurrentTutorial() == Tutorials.BuildTutorial) return;

            foreach (BuildingComponent build in _buildings) {
                if(build.NeedInteraction()) continue;
                
                build.GetBuild().Interact(build.GetCurrentLevel());
            }
        }

        private IEnumerator Start()
        {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;
            
            yield return LoadBuildings();

            if (_buildings.Count == 0) yield break;
            
            TimeSpan time = DateTime.Now - DateTime.Parse(PlayerPrefs.GetString(_EXIT_TIME));

            if (time.Minutes < 3f) yield break;
            
            foreach (BuildingComponent build in _buildings)
            {
                if (build.NeedInteraction()) continue;
                
                Farm farm = (Farm) build.GetBuild();
                int generatedTime = time.Minutes >= 180 ? 180 : time.Minutes;
                int resourcesGenerated =
                    Mathf.RoundToInt((generatedTime * 60) / farm.GetGeneration(build.GetCurrentLevel()));
                    
                _player.AddResource(farm.GetResource().GetKey(), resourcesGenerated);
            }
        }

        private void OnDisable()
        {
            if (_gameManager.GetCurrentTutorial() == Tutorials.BuildTutorial ||
                SceneManagement.Instance.CurrentScene() != SceneIndexes.GAME_SCENE) return;
            
            SaveBuildings();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        public List<BuildingComponent> GetBuildings() => _buildings;

        public void AddBuilding(BuildingComponent build)
        {
            _buildingsCount[build.GetBuild()]++;
            _buildings.Add(build);
        }

        public void RemoveBuilding(BuildingComponent build)
        {
            _buildingsCount[build.GetBuild()]--;
            _buildings.Remove(build);
        }

        public int GetBuildCount(Building build) => _buildingsCount[build];

        public void SaveBuildings() {
            BuildingComponent[] buildings = GetComponentsInChildren<BuildingComponent>();
            BuildingData[] saveData = new BuildingData[buildings.Length];
            int i = 0;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "buildings_data.json");

            foreach (BuildingComponent build in buildings) {
                BuildingData data = new BuildingData();
                
                data.Position = build.GetPosition();
                data.Building = build.GetBuild();
                data.Level = build.GetCurrentLevel();

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
            else data = File.ReadAllText(filePath);
            
            
            Building[] builds = Resources.LoadAll<Building>("Buildings");

            foreach (Building build in builds)
                _buildingsCount.Add(build, 0);
            
            if (!string.IsNullOrEmpty(data)) {
                BuildingData[] buildings = JsonHelper.FromJson<BuildingData>(data);
                BuildingComponent[] buildingComponents = new BuildingComponent[buildings.Length];
                int i = 0;
                
                foreach (BuildingData build in buildings) {
                    GameObject go = build.Building.Spawn(build.Position, transform, build.Level, false);
                    buildingComponents[i] = go.GetComponent<BuildingComponent>();
                    buildingComponents[i].SetLevel(build.Level);
                    _buildingsCount[buildingComponents[i].GetBuild()]++;
                    i++;
                }

                OnBuildsLoad?.Invoke(buildingComponents);
                _buildings = buildingComponents.ToList();
            }
            else
            {
                OnBuildsLoad?.Invoke(null);
                _buildings = new List<BuildingComponent>();
            }
        }
    }
}
