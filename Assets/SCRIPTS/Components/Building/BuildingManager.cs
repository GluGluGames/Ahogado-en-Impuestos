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
using GGG.Components.Scenes;
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
            public string Building;
            public int[] CurrentCost;
            public string[] CurrentResourcesCost;
            public int Level;
            public string FarmResource;
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

        
        private int _currentId = 1;
        
        private const string _EXIT_TIME = "ExitTime";
        private const float _RATE_GROW = 1.2f;
        
        public static Action<BuildingComponent[]> OnBuildsLoad;
        public Action<string, int> OnBuildAdd;

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

        private void Start()
        {
            _player = PlayerManager.Instance;
            _gameManager = GameManager.Instance;
            _achievementsManager = AchievementsManager.Instance;
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        public List<BuildingComponent> GetBuildings() => _buildings;

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
            
            OnBuildAdd?.Invoke(build.BuildData().GetKey(), _buildingsCount[building]);
            SaveBuildings();
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
            
            SaveBuildings();
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
            string filePath = Path.Combine(Application.persistentDataPath, "buildings_data.json");

            foreach (BuildingComponent build in _buildings)
            {
                List<string> aux = new();
                foreach(Resource resource in build.CurrentCost().GetResource())
                    aux.Add(resource.GetKey());
                
                BuildingData data = new()
                {
                    Id = build.Id(),
                    Position = build.Position(),
                    Building = build.BuildData().GetKey(),
                    Level = build.CurrentLevel(),
                    CurrentCost = build.CurrentCost().GetCost(),
                    CurrentResourcesCost = aux.ToArray()
                };
                
                data.IsBoost = build.BuildData().CanBeBoost() && build.IsBoost();

                saveData[i] = data;
                i++;
            }
            
            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        public IEnumerator LoadBuildings()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "buildings_data.json");
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
            List<Building> buildingsSo = Resources.LoadAll<Building>("Buildings").ToList();
            int i = 0;
                
            foreach (BuildingData buildData in buildings)
            {
                Building build = buildingsSo.Find(x => x.GetKey() == buildData.Building);
                 GameObject go = build.Spawn(buildData.Position, transform, buildData.Level, false);
                buildingComponents[i] = go.GetComponent<BuildingComponent>();
                List<Resource> aux = new();
                foreach (string resource in buildData.CurrentResourcesCost)
                    PlayerManager.Instance.GetResource(resource);
                
                buildingComponents[i].SetId(buildData.Id);
                buildingComponents[i].SetLevel(buildData.Level);
                buildingComponents[i].SetCurrentCost(new ResourceCost(buildData.CurrentCost, aux.ToArray()));

                if (buildingComponents[i].BuildData().CanBeBoost() && buildData.IsBoost)
                    buildingComponents[i].Boost();
                
                
                AddBuilding(buildingComponents[i]);
                i++;
            }

            while(_buildings.Find((x) => x.Id() == _currentId)) _currentId++;
            _buildings = buildingComponents.ToList();
            
            OnBuildsLoad?.Invoke(buildingComponents);
        }
    }
}
