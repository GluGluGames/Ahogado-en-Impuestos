using GGG.Components.Buildings.Generator;
using GGG.Components.Buildings.Laboratory;
using GGG.Components.HexagonalGrid;
using GGG.Components.Buildings;
using GGG.Components.Scenes;
using GGG.Components.Player;
using GGG.Components.Core;
using GGG.Components.UI;
using GGG.Shared;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GGG.Components.Serialization
{
    public class SerializationManager : MonoBehaviour
    {
        public static SerializationManager Instance;

        private void Awake()
        {
            if (Instance) return;

            Instance = this;
        }

        private const string _EXIT_TIME = "ExitTime";

        private GameManager _gameManager;
        private SceneManagement _sceneManagement;

        private BuildingSerialization _building;
        private GeneratorSerialization _generator;
        private HUDSerialization _hud;
        private LaboratorySerialization _laboratory;
        private ResourcesSerialization _resources;
        private TilesSerialization _tiles;

        private const int _SAVE_TIME = 300;
        private const int _TIME_SAVE_TIME = 5;
        private const string _KEY = "7924629";
        
        private float _delta;
        private float _deltaTime;
        private bool _initialized;

        private void Start()
        {
            _gameManager = GameManager.Instance;
            _sceneManagement = SceneManagement.Instance;

            if (SceneManagement.InGameScene())
            {
                Initialize();
                StartCoroutine(Load());
            }
            
            _sceneManagement.OnGameSceneLoaded += () =>
            {
                Initialize();
                _sceneManagement.AddEnumerators(Load());
            };

            _sceneManagement.OnGameSceneUnloaded += SaveNoEnum;

            _delta = 0f;
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString("hh:mm:ss"));
            PlayerPrefs.Save();
        }

        private void Update()
        {
            if (!_initialized || !_gameManager.Playing()) return;

            if (!SceneManagement.InGameScene() || 
                _gameManager.GetCurrentTutorial() is Tutorials.BuildTutorial or Tutorials.InitialTutorial) return;

            if (_delta >= _SAVE_TIME)
            {
                Save();
                _delta = 0;
            }

            _delta += Time.deltaTime;
        }

        private void Initialize()
        {
            _building = GetComponent<BuildingSerialization>();
            _generator = GetComponent<GeneratorSerialization>();
            _hud = GetComponent<HUDSerialization>();
            _laboratory = GetComponent<LaboratorySerialization>();
            _resources = GetComponent<ResourcesSerialization>();
            _tiles = GetComponent<TilesSerialization>();

            _initialized = true;
        }

        public static string EncryptDecrypt(string data)
        {
            string result = "";

            for (int i = 0; i < data.Length; i++)
                result += (char)(data[i] ^ _KEY[i % _KEY.Length]);

            return result;
        }

        private IEnumerator Load()
        {
            yield return null;
            
            List<IEnumerator> order = new()
            {
                _building.LoadBuildings(),
                _tiles.LoadTilesState(),
                _resources.LoadResourcesCount(),
                _hud.LoadShownResource(),
                _laboratory.LoadResearchProgress(),
                _generator.LoadGeneratorState()
            };

            foreach (IEnumerator enumerator in order)
            {
                yield return enumerator;
            }
        }

        private void SaveNoEnum() => StartCoroutine(Save());

        public IEnumerator Save()
        {
            if (!SceneManagement.InGameScene()) yield break;
            
            yield return _building.SaveBuildings();
            yield return _tiles.SaveTilesState();
            yield return _resources.SaveResourcesCount();
            yield return _hud.SaveShownResources();
            yield return _laboratory.SaveResearchProgress();
            yield return _generator.SaveGeneratorState();
        }
    }
}
