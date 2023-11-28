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
using UnityEngine.Networking;

namespace GGG.Components.Buildings.Generator
{
    public class GeneratorUI : MonoBehaviour
    {
        [Header("Generator fields")] 
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
        
        public static Action OnGeneratorOpen;

        private GameManager _gameManager;

        private readonly Dictionary<int, Generator> _generators = new ();
        private List<HexTile> _tiles;
        private HexTile _generatorTile;
        
        private GameObject _viewport;
        private bool _open;
        private Generator _currentGenerator;

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

        private void OnDisable()
        {
            if (!SceneManagement.InGameScene() || _gameManager.OnTutorial() || _generators.Count <= 0) return;
            
            SaveGeneratorState();
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
            if (_currentGenerator.CurrentGeneration(level) >= level || _currentGenerator.BoostIndex(level, idx) >= 1) 
                return;
            
            BuildingComponent building = _generatorTile.neighbours[idx].GetCurrentBuilding();
            if (!building) return;
            
            _currentGenerator.AddGeneration(level, 1);
            _currentGenerator.AddBoostIndex(level, idx, 1);
            _currentGenerator.AddBoostingBuilding(level, idx, building.Id());
            _currentGenerator.SetBoostingTime(level, idx, BoostTimes[level - 1]);
            
            ChangeButtonSprite(level, idx);
            
            StartCoroutine(BoostBuilding(_currentGenerator.Id(), building, level, idx));
        }

        private void ChangeButtonSprite(int level, int idx)
        {
            Color color = new (1, 1, 1, _currentGenerator.BoostIndex(level, idx) == 1 ? 1 : 0);
            
            switch (level)
            {
                case 1:
                    BostButtonsLevel1[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel1[idx].interactable = _currentGenerator.BoostIndex(level, idx) != 1;
                    BostButtonsLevel1[idx].image.color = color;
                    break;
                case 2:
                    BostButtonsLevel2[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel2[idx].interactable = _currentGenerator.BoostIndex(level, idx) != 1;
                    BostButtonsLevel2[idx].image.color = color;
                    break;
                default:
                    BostButtonsLevel3[idx].image.sprite = BoostButtonToggles[level - 1];
                    BostButtonsLevel3[idx].interactable = _currentGenerator.BoostIndex(level, idx) != 1;
                    BostButtonsLevel3[idx].image.color = color;
                    break;
            }
        }

        private IEnumerator BoostBuilding(int generatorId, BuildingComponent building, int level, int idx)
        {
            building.Boost();
            
            while (_generators[generatorId].BoostingTime(level, idx) > 0)
            {
                _generators[generatorId].AddBoostingTime(level, idx, -Time.deltaTime);
                yield return null;
            }
            
            building.EndBoost();
            
            _currentGenerator.AddGeneration(level, -1);
            _currentGenerator.AddBoostIndex(level, idx, -1);
            _currentGenerator.AddBoostingBuilding(level, idx, -1);
            _currentGenerator.SetBoostingTime(level, idx, 0f);
            ChangeButtonSprite(level, idx);
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
                    ChangeButtonSprite(i + 1, j);
                }
            }
        }

        private void InitializeGenerator(Generator generator)
        {
            _currentGenerator = generator;
            if(!_generators.ContainsKey(generator.Id())) 
                _generators.Add(generator.Id(), generator);
        }

        public void OnBuildDestroy(int id) => _generators.Remove(id);
        

        private void OnBuildLoads(BuildingComponent[] buildings)
        {
            if (buildings == null) return;

            List<Generator> build = new();

            foreach (BuildingComponent building in buildings)
            {
                if (building.GetType() != typeof(Generator)) continue;

                build.Add((Generator) building);
            }
            
            StartCoroutine(LoadGeneratorState(build.ToArray()));
        }
        
        [Serializable]
        private class GeneratorData
        {
            public int GeneratorId;
            public List<BoostIndexes> Indexes;
            public List<BoostBuildings> Buildings;
            public List<BoostingTime> RemainingTimes;
        }

        [Serializable]
        private class BoostIndexes {
            public int Level;
            public int Index;

            public BoostIndexes(int level, int index)
            {
                Level = level;
                Index = index;
            }
        }

        [Serializable]
        private class BoostBuildings
        {
            public int Level;
            public int Index;
            public int BuildingId;

            public BoostBuildings(int level, int idx, int buildingId)
            {
                Level = level;
                Index = idx;
                BuildingId = buildingId;
            }
        }

        [Serializable]
        private class BoostingTime
        {
            public int Level;
            public int Index;
            public float Time;

            public BoostingTime(int level, int idx, float time)
            {
                Level = level;
                Index = idx;
                Time = time;
            }
        }

        private void SaveGeneratorState()
        {
            if (_generators.Count <= 0) return;
            
            GeneratorData[] saveData = new GeneratorData[_generators.Count];
            int i = 0;
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "generators_boost.json");

            foreach (int id in _generators.Keys)
            {
                int level = _generators[id].CurrentLevel();
                
                GeneratorData data = new()
                {
                    GeneratorId = id,
                    Indexes = new List<BoostIndexes>(),
                    Buildings = new List<BoostBuildings>(),
                    RemainingTimes = new List<BoostingTime>()
                };

                for (int y = 1; y <= level; y++) {
                    for (int j = 0; j < _generators[id].BoostIndexes().GetLength(1); j++) {
                        if (_generators[id].BoostIndex(y, j) == 1)
                            data.Indexes.Add(new BoostIndexes(y, j));
                        
                        if (_generators[id].BoostBuilding(y, j) != -1)
                            data.Buildings.Add(new BoostBuildings(y, j, _generators[id].BoostBuilding(y, j)));
                        
                        if (_generators[id].BoostingTime(y, j) > 0)
                            data.RemainingTimes.Add(new BoostingTime(y, j, _generators[id].BoostingTime(y, j)));
                    }

                }

                saveData[i] = data;
                i++;
            }
            
            string jsonData = JsonHelper.ToJson(saveData, true);
            File.WriteAllText(filePath, jsonData);
        }

        private IEnumerator LoadGeneratorState(Generator[] generators)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath + "/", "generators_boost.json");
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

            if (string.IsNullOrEmpty(data)) yield break;
            
            GeneratorData[] generatorData = JsonHelper.FromJson<GeneratorData>(data);

            foreach (GeneratorData generator in generatorData)
            {
                foreach (Generator gen in generators)
                {
                    if (gen.Id() != generator.GeneratorId) continue;
                        
                    _generators.Add(generator.GeneratorId, gen);

                    for (int i = 0; i < generator.Buildings.Count; i++)
                    {
                        gen.AddBoostIndex(generator.Indexes[i].Level, generator.Indexes[i].Index, 1);
                        gen.AddBoostingBuilding(generator.Buildings[i].Level, generator.Buildings[i].Index, generator.Buildings[i].BuildingId);
                        gen.AddBoostingTime(generator.RemainingTimes[i].Level, generator.RemainingTimes[i].Index, generator.RemainingTimes[i].Time);
                        gen.AddGeneration(generator.Indexes[i].Level, 1);

                        BuildingComponent build = BuildingManager.Instance.GetBuildings()
                            .Find(x => x.Id() == generator.Buildings[i].BuildingId);

                        StartCoroutine(BoostBuilding(gen.Id(), build, generator.Buildings[i].Level,
                            generator.Buildings[i].Index));
                    }
                }
            }
        }

        public void Open(Generator generator)
        {
            if(_open) return;
            _open = true;
            
            _viewport.SetActive(true);
            _gameManager.OnUIOpen();
            ActiveLevelPanel(0);
            InitializeGenerator(generator);
            
            OnGeneratorOpen?.Invoke();
            
            FindGeneratorTile();
            HandleLevelButtons(_currentGenerator.CurrentLevel());
            HandleButtonToggles(_currentGenerator.CurrentLevel());
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
