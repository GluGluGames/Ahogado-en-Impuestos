using GGG.Classes.Buildings;
using GGG.Components.Core;
using GGG.Components.HexagonalGrid;

using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TMPro;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorUI : MonoBehaviour
    {
        [Header("Generator fields")] 
        [SerializeField] private Building Generator;
        [SerializeField] private int[] BoostTimes;
        [SerializeField] private Sprite[] BoostButtonToggles;
        [Space(5), Header("Containers")] 
        [SerializeField] private GameObject[] LevelContainers;
        [Space(5), Header("Text")] 
        [SerializeField] private TMP_Text[] BostTextsLevel1;
        [SerializeField] private TMP_Text[] BostTextsLevel2;
        [SerializeField] private TMP_Text[] BostTextsLevel3;
        [Space(5), Header("Buttons")]
        [SerializeField] private Button[] BostButtonsLevel1;
        [SerializeField] private Button[] BostButtonsLevel2;
        [SerializeField] private Button[] BostButtonsLevel3;
        [SerializeField] private Button[] LevelButtons;
        [SerializeField] private Button CloseButton;

        private class GeneratorData
        {
            public BuildingComponent Generator;
            public int BoostIndex;
        }
        
        public static Action OnGeneratorOpen;

        private GameManager _gameManager;

        private Dictionary<BuildingComponent, int> _generators = new ();
        private readonly Dictionary<BuildingComponent, int[]> _currentGeneration = new ();
        private readonly Dictionary<BuildingComponent, bool[,]> _buildingBoosting = new();
        private List<HexTile> _tiles;
        private List<BuildingComponent> _buildings;
        private HexTile _generatorTile;
        
        private GameObject _viewport;
        private bool _open;
        private BuildingComponent _currentGenerator;

        private void Awake()
        {
            for (int i = 0; i < LevelButtons.Length; i++)
            {
                int idx = i;
                LevelButtons[i].gameObject.SetActive(i % LevelButtons.Length == 0);
                LevelButtons[i].onClick.AddListener(() => ActiveLevelPanel(idx));
            }

            for (int i = 0; i < BostButtonsLevel1.Length; i++)
            {
                int idx = i;
                BostButtonsLevel1[i].onClick.AddListener(() => OnBuildingBoost(1, idx));
                BostButtonsLevel1[i].image.sprite = null;
                
                BostButtonsLevel2[i].onClick.AddListener(() => OnBuildingBoost(2, idx));
                BostButtonsLevel2[i].image.sprite = null;
                
                BostButtonsLevel3[i].onClick.AddListener(() => OnBuildingBoost(3, idx));
                BostButtonsLevel3[i].image.sprite = null;
            }
            
            CloseButton.onClick.AddListener(OnCloseButton);
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
            
            _viewport = transform.GetChild(0).gameObject;
            _viewport.SetActive(false);
            _viewport.transform.position = new Vector3(Screen.width * -0.5f, Screen.height * 0.5f);

            _tiles = FindObjectsOfType<HexTile>().ToList();
            BuildingManager.OnBuildsLoad += OnBuildLoads;
        }

        private void ActiveLevelPanel(int idx)
        {
            for (int i = 0; i < LevelContainers.Length; i++)
                LevelContainers[i].SetActive(i == idx);
            
        }

        private void FindGeneratorTile()
        {
            _generatorTile = _tiles.Find(t => 
                t.GetCurrentBuilding() && t.GetCurrentBuilding().Equals(_currentGenerator));

            if (!_generatorTile)
                throw new Exception("Generator Tile not found");
        }

        private void HandleLevelButtons(int level)
        {
            LevelButtons[0].gameObject.SetActive(true);
            LevelButtons[1].gameObject.SetActive(level >= 2);
            LevelButtons[2].gameObject.SetActive(level >= 3);
        }

        private void OnBuildingBoost(int level, int idx)
        {
            if (_currentGeneration[_currentGenerator][level - 1] >= level || 
                _buildingBoosting[_currentGenerator][level - 1, idx]) return;
            
            BuildingComponent building = _generatorTile.neighbours[idx].GetCurrentBuilding();
            if (!building) return;

            building.Boost();
            
            ChangeButtonSprite(level, idx, true);
            _currentGeneration[_currentGenerator][level - 1]++;
            _buildingBoosting[_currentGenerator][level - 1, idx] = true;
            
            StartCoroutine(BoostBuilding(building, level, idx));
        }

        private void ChangeButtonSprite(int level, int idx, bool visible)
        {
            Color color = new (1, 1, 1, visible ? 1 : 0);
            
            switch (level)
            {
                case 1:
                    BostButtonsLevel1[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel1[idx].interactable = !visible;
                    BostButtonsLevel1[idx].image.color = color;
                    break;
                case 2:
                    BostButtonsLevel2[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel2[idx].interactable = !visible;
                    BostButtonsLevel2[idx].image.color = color;
                    break;
                default:
                    BostButtonsLevel3[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel3[idx].interactable = !visible;
                    BostButtonsLevel3[idx].image.color = color;
                    break;
            }
        }

        private IEnumerator BoostBuilding(BuildingComponent building, int level, int idx)
        {
            float deltaTime = BoostTimes[level - 1];
            BuildingComponent aux = _currentGenerator;

            while (deltaTime > 0)
            {
                deltaTime -= Time.deltaTime;
                yield return null;
            }
            
            building.EndBoost();
            
            ChangeButtonSprite(level, idx, false);
            _currentGeneration[_currentGenerator][level - 1]--;
            _buildingBoosting[_currentGenerator][level - 1, idx] = false;
        }

        private void ChangeBuildText()
        {
            for (int i = 0; i < BostTextsLevel1.Length; i++)
            {
                BuildingComponent building = _generatorTile.neighbours[i].GetCurrentBuilding();
                
                BostTextsLevel1[i].SetText(building && building.BuildData().CanBeBoost() ? 
                    building.BuildData().GetName() : "");
                
                BostTextsLevel2[i].SetText(building && building.BuildData().CanBeBoost() ? 
                    building.BuildData().GetName() : "");
                
                BostTextsLevel3[i].SetText(building && building.BuildData().CanBeBoost() ? 
                    building.BuildData().GetName() : "");
            }
        }

        private void HandleButtonToggles(int level)
        {
            for (int i = 0; i < level; i++)
            {
                for (int j = 0; j < BostButtonsLevel1.Length; j++)
                {
                    ChangeButtonSprite(i + 1, j, _buildingBoosting[_currentGenerator][i, j]);
                }
            }
        }

        private void FindGenerator(int level)
        {
            _buildings = BuildingManager.Instance.GetBuildings();
            if (_buildings == null) return;
            
            foreach (BuildingComponent building in _buildings)
            {
                if(building.BuildData().GetType() != typeof(Generator)) continue;

                _currentGenerator = building;

                if (_generators.ContainsKey(building))
                {
                    _generators[building] = _generators[building] == level ? _generators[building] : level;
                    continue;
                }

                _generators.Add(building, level);
                _currentGeneration.Add(building, new int[3]);
                _buildingBoosting.Add(building, new bool[3,6]);
            }
        }

        private void OnBuildLoads(BuildingComponent[] buildings)
        {
            if (buildings == null) return;

            _buildings = buildings.ToList();

            foreach (BuildingComponent building in buildings)
            {
                if (building.BuildData().GetType() != typeof(Generator)) continue;

                StartCoroutine(LoadGeneratorState(building));
            }
        }

        private void SaveGeneratorState()
        {
            if (_generators.Count <= 0) return;
            
            GeneratorData[] saveData = new GeneratorData[_generators.Count];
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "tiles_data.json");

            foreach (BuildingComponent generator in _generators.Keys)
            {
                GeneratorData data = new()
                {
                    Generator = generator
                };
            }
        }

        private IEnumerator LoadGeneratorState(BuildingComponent generator)
        {
            yield return null;
        }

        public void Open(int level)
        {
            if(_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            FindGenerator(level);
            
            OnGeneratorOpen?.Invoke();
            
            HandleLevelButtons(level);
            HandleButtonToggles(level);
            FindGeneratorTile();
            ChangeBuildText();

            _viewport.transform.DOMoveX(Screen.width * 0.5f, 0.75f).SetEase(Ease.InCubic);
        }

        private void OnCloseButton()
        {
            if (!_open || _gameManager.TutorialOpen() || _gameManager.OnTutorial()) return;

            Close();
        }

        private void Close()
        {
            _viewport.transform.DOMoveX(Screen.width * -0.5f, 0.75f).SetEase(Ease.OutCubic).onComplete += () =>
            {
                _open = false;
                _viewport.SetActive(false);
                
                LevelButtons[1].gameObject.SetActive(false);
                LevelButtons[2].gameObject.SetActive(false);
                _currentGenerator = null;
                _generatorTile = null;
            };
            
            _gameManager.OnUIClose();
        }
    }
}
