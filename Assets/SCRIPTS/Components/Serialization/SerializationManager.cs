using GGG.Components.Buildings;
using GGG.Components.Buildings.Generator;
using GGG.Components.Buildings.Laboratory;
using GGG.Components.Core;
using GGG.Components.HexagonalGrid;
using GGG.Components.Player;
using GGG.Shared;

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Components.UI;

namespace GGG.Components.Serialization
{
    public class SerializationManager : MonoBehaviour
    {
        private const string _EXIT_TIME = "ExitTime";

        private GameManager _gameManager;
        private SceneManagement _sceneManagement;

        private PlayerManager _playerManager;
        private BuildingManager _buildingManager;
        private HUDManager _hudManager;
        private TileManager _tileManager;
        private GeneratorUI _generatorUI;
        private LaboratoryUI _laboratoryUI;

        private const int _SAVE_TIME = 300;
        
        private float _delta;

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

            _delta = 0f;
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
        }

        private void Update()
        {
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
            _playerManager = PlayerManager.Instance;
            _buildingManager = BuildingManager.Instance;
            _hudManager = HUDManager.Instance;
            _tileManager = TileManager.Instance;
            _generatorUI = FindObjectOfType<GeneratorUI>();
            _laboratoryUI = FindObjectOfType<LaboratoryUI>();
        }

        private IEnumerator Load()
        {
            yield return null;
            
            List<IEnumerator> order = new()
            {
                _playerManager.LoadResourcesCount(),
                _hudManager.LoadShownResource(),
                _buildingManager.LoadBuildings()
            };

            foreach (IEnumerator enumerator in order)
            {
                yield return enumerator;
            }
        }

        private void Save()
        {
            _playerManager.SaveResourcesCount();
            _hudManager.SaveShownResources();
            _tileManager.SaveTilesState();
            _buildingManager.SaveBuildings();
            _laboratoryUI.SaveResearchProgress();
            _generatorUI.SaveGeneratorState();
        }
    }
}
