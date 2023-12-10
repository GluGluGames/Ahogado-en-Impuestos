using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Components.Player;
using GGG.Shared;


using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GGG.Components.Achievements;
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
            public int Id;
            public Vector3 Position;
            public Building Building;
            public ResourceCost CurrentCost;
            public int Level;
            public Resource FarmResource;
            public bool IsBoost;
        }

        [SerializeField] private Building SeaFarm;
        [SerializeField] private Resource Seaweed;

        private PlayerManager _player;
        private GameManager _gameManager;
        private AchievementsManager _achievementsManager;
        
        private readonly Dictionary<Building, int> _buildingsCount = new();
        private readonly Dictionary<Building, ResourceCost> _buildingsCosts = new();
        private List<BuildingComponent> _buildings = new();
        private List<Building> _builds;
        private List<BuildingComponent> _achievementsBuildings = new(3);

        
        private readonly List<Farm> _farms = new();
        private int _currentId = 1;
        
        private const string _EXIT_TIME = "ExitTime";
        private const float _RATE_GROW = 1.05f;
        
        public static Action<BuildingComponent[]> OnBuildsLoad;

        private void Awake()
        {
            if (!Instance) Instance = this;
            
            _builds = Resources.LoadAll<Building>("Buildings").ToList();
            foreach (Building build in _builds)
            {
                _buildingsCount.Add(build, 0);
                _buildingsCosts.Add(build, new ResourceCost(build.GetBuildingCost()));
            }
        }

        private void Update() {
            if (_farms.Count == 0 || _gameManager.GetCurrentTutorial() == Tutorials.BuildTutorial) return;

            foreach (Farm farm in _farms) 
                farm.Produce();
        }

        private void Start()
        {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;
            _achievementsManager = AchievementsManager.Instance;
        }

        private void OnDisable()
        {
            SaveBuildings();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        public List<BuildingComponent> GetBuildings() => _buildings;

        private void ResourceSummary(Farm farm, TimeSpan time)
        {
            int generatedTime = time.Minutes >= 180 ? 180 : time.Minutes;
            int resourcesGenerated = Mathf.RoundToInt(
                generatedTime * 60 / (farm.GetGeneration() + (farm.IsBoost() ? farm.GetGeneration() * 0.25f : 0)));
            
            _player.AddResource(farm.GetResource().GetKey(), resourcesGenerated);
        }

        public void AddBuilding(BuildingComponent build)
        {
            Building building = build.BuildData();
            
            build.Initialize();
            build.SetId(_currentId);
            _buildingsCount[building]++;
            _buildings.Add(build);
            
            ArchitectAchievement(build);
            ConstructorAchievement();

            do _currentId++; 
            while (_buildings.Find((x) => x.Id() == _currentId));
            
            int formula = Mathf.RoundToInt(building.GetBuildingCost().GetCost(0) * Mathf.Pow(_RATE_GROW, _buildingsCount[building]));
            _buildingsCosts[building].SetCost(0, formula);

            if (build.GetType() == typeof(Farm)) _farms.Add((Farm) build);
        }

        private void ArchitectAchievement(BuildingComponent build)
        {
            if (_achievementsManager.Achievement("02").IsUnlocked()) return;
            
            if(!_achievementsBuildings.Find(x => x.BuildData() == build.BuildData()))
                _achievementsBuildings.Add(build);

            if (_achievementsBuildings.Count >= 3)
                StartCoroutine(_achievementsManager.UnlockAchievement("02"));
        }

        private void ConstructorAchievement()
        {
            if (_achievementsManager.Achievement("06").IsUnlocked()) return;

            int aux = 0;

            foreach (int building in _buildingsCount.Values)
            {
                aux += building;
                if (aux < 20) continue;
                
                StartCoroutine(_achievementsManager.UnlockAchievement("06"));
                break;
            }
        }

        public void RemoveBuilding(BuildingComponent build)
        {
            Building building = build.BuildData();
            
            _buildingsCount[building]--;
            _buildings.Remove(build);
            _currentId = build.Id();
            
            int formula = Mathf.RoundToInt(building.GetBuildingCost().GetCost(0) * Mathf.Pow(_RATE_GROW, _buildingsCount[building]));
            _buildingsCosts[building].SetCost(0, formula <= 0 ? 0 : formula);
            
            if (build.GetType() == typeof(Farm)) _farms.Remove((Farm) build);
        }

        public int GetBuildCount(Building build) => _buildingsCount[build];
        public ResourceCost GetBuildingCost(Building build)
        {
            if (build == SeaFarm && _buildingsCount[build] <= 0)
                return new ResourceCost(new[] { 0 }, new[] { Seaweed });

            return _buildingsCosts[build];
        }

        public void SaveBuildings()
        {
            if (!SceneManagement.InGameScene() || 
                _gameManager.GetCurrentTutorial() is Tutorials.BuildTutorial or Tutorials.InitialTutorial) return;
            
            BuildingData[] saveData = new BuildingData[_buildings.Count];
            int i = 0;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "buildings_data.json");

            foreach (BuildingComponent build in _buildings)
            {
                BuildingData data = new()
                {
                    Id = build.Id(),
                    Position = build.Position(),
                    Building = build.BuildData(),
                    Level = build.CurrentLevel(),
                    CurrentCost = build.CurrentCost()
                };

                if (build.GetType() == typeof(Farm))
                {
                    Farm farm = (Farm)build;
                    if (farm.GetResource()) data.FarmResource = farm.GetResource();
                }
                
                data.IsBoost = build.BuildData().CanBeBoost() && build.IsBoost();

                saveData[i] = data;
                i++;
            }

            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadBuildings()
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "buildings_data.json");
            string data;

            if (!File.Exists(filePath))
            {
                OnBuildsLoad?.Invoke(null);
                yield break;
            }
            
            if (filePath.Contains("://") || filePath.Contains(":///")) {
                UnityWebRequest www = UnityWebRequest.Get(filePath);
                yield return www.SendWebRequest();
                data = www.downloadHandler.text;
            }
            else data = File.ReadAllText(filePath);
            
            BuildingData[] buildings = JsonHelper.FromJson<BuildingData>(data);
            BuildingComponent[] buildingComponents = new BuildingComponent[buildings.Length];
            int i = 0;
                
            foreach (BuildingData build in buildings) {
                 GameObject go = build.Building.Spawn(build.Position, transform, build.Level, false);
                buildingComponents[i] = go.GetComponent<BuildingComponent>();
                
                buildingComponents[i].SetId(build.Id);
                buildingComponents[i].SetLevel(build.Level);
                buildingComponents[i].SetCurrentCost(build.CurrentCost);
                
                if (buildingComponents[i].GetType() == typeof(Farm))
                {
                    Farm farm = (Farm)buildingComponents[i];
                    if (build.FarmResource)
                    {
                        farm.Resource(build.FarmResource);
                        GameObject resource = Instantiate(build.FarmResource.GetModel(),
                            farm.transform.position + new Vector3(0, 2.5f), Quaternion.identity, farm.transform);
                        resource.transform.localScale = build.FarmResource.GetModelScale();
                        farm.SetResourceModel(resource);
                    }
                }

                if (buildingComponents[i].BuildData().CanBeBoost() && build.IsBoost)
                    buildingComponents[i].Boost();
                
                
                AddBuilding(buildingComponents[i]);
                i++;
            }

            while(_buildings.Find((x) => x.Id() == _currentId)) _currentId++;
            _buildings = buildingComponents.ToList();

            if (_farms.Count == 0)
            {
                TimeSpan time = DateTime.Now.Subtract(DateTime.Parse(PlayerPrefs.GetString("ExitTime")));

                if (time.Minutes < 1f)
                    foreach (Farm farm in _farms) if (farm.GetResource()) ResourceSummary(farm, time);
            }
            
            OnBuildsLoad?.Invoke(buildingComponents);
        }
    }
}
