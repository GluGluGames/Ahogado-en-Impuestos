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

        [SerializeField] private Building SeaFarm;
        [SerializeField] private Resource Seaweed;
        
        private AchievementsManager _achievementsManager;
        
        private readonly Dictionary<Building, int> _buildingsCount = new();
        private readonly Dictionary<Building, ResourceCost> _buildingsCosts = new();
        private List<BuildingComponent> _buildings = new();
        private List<Building> _builds;
        private List<BuildingComponent> _achievementsBuildings = new(3);

        
        private int _currentId = 1;
        
        private const string _EXIT_TIME = "ExitTime";
        private const float _RATE_GROW = 1.2f;
        
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
            _achievementsManager = AchievementsManager.Instance;
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        public List<BuildingComponent> GetBuildings() => _buildings;

        public void HandleCurrentId()
        {
            while (_buildings.Find((x) => x.Id() == _currentId)) _currentId++;
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
            
            OnBuildAdd?.Invoke(build.BuildData().GetKey(), _buildingsCount[building]);
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
        }

        public int GetBuildCount(Building build) => _buildingsCount[build];
        public ResourceCost GetBuildingCost(Building build)
        {
            if (build == SeaFarm && _buildingsCount[build] <= 0)
                return new ResourceCost(new[] { 0 }, new[] { Seaweed });

            return _buildingsCosts[build];
        }
    }
}
