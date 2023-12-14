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
using System.IO;
using GGG.Components.UI;
using UnityEngine.Networking;
using UnityEngine.Serialization;

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

        private static User _currentUser;
        
        private const string _CONTENT_TYPE = "application/json";

        private static string _databaseUser;
        private static string _password;
        private static string _postUri;
        private static string _getUri;
        
        private float _delta;
        
        private void Awake()
        {
            LoadCredentials();
        }

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

            _sceneManagement.OnGameSceneUnloaded += Save;

            _delta = 0f;
        }

        private void OnDisable()
        {
            PlayerPrefs.SetString(_EXIT_TIME, DateTime.Now.ToString());
            PlayerPrefs.Save();
            Save();
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
                _buildingManager.LoadBuildings(),
                _tileManager.LoadTilesState(),
                _hudManager.LoadShownResource(),
            };

            foreach (IEnumerator enumerator in order)
            {
                yield return enumerator;
            }
        }

        private void Save()
        {
            if (!SceneManagement.InGameScene() || 
                _gameManager.GetCurrentTutorial() is Tutorials.BuildTutorial or Tutorials.InitialTutorial) return;
            
            _playerManager.SaveResourcesCount();
            _hudManager.SaveShownResources();
            _laboratoryUI.SaveResearchProgress();
            _generatorUI.SaveGeneratorState();
        }
    
        [Serializable]
        private class Credentials
        {
            public string username;
            public string password;
            public string uri;
            public string uriGet;
        }
        
        [Serializable]
        public class User
        {
            public int Id;
            public string Name;
            public int Age;
            public int Gender;
            public string Password;
        }

        [Serializable]
        private class UserData
        {
            public string result;
            public List<User> data;
        }
        
        public static string CreateUserJson(string userName, int userAge, int gender, string password)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""Users"",
                ""data"": {{
                    ""name"": ""{userName}"",
                    ""age"": ""{userAge}"",
                    ""gender"": ""{gender}"",
                    ""password"": ""{password}""
                }}
            }}";

            return json;
        }
        
        public static string FindUsersJson(string userName)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""Users"",
                ""filter"":{{""name"":""{userName}""}}
            }}";

            return json;
        }
        
        public static string FindPasswordsJson(string userName, string password)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""Users"",
                ""filter"":{{""name"":""{userName}"", ""password"":""{password}""}}
            }}";

            return json;
        }

        public static User CurrentUser() => _currentUser;
        public static void SetCurrentUser(User user) => _currentUser = user;

        public static IEnumerator GetUserData(Action<bool, User> response, string data)
        {
            using UnityWebRequest www = UnityWebRequest.Post(_getUri, data, _CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception("Error at GET request: " + www.error);
            
            print($"Get Request Result -> {www.downloadHandler.text}");

            User user = null;
            bool userFound = JsonUtility.FromJson<UserData>(www.downloadHandler.text).data.Count > 0;
            if (userFound) user = JsonUtility.FromJson<UserData>(www.downloadHandler.text).data[0];
            response?.Invoke(userFound, user);
            
        }

        public static IEnumerator PostData(string data)
        {
            using UnityWebRequest www = UnityWebRequest.Post(_postUri, data, _CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception(www.error);

            print($"POST Successful -> {www.downloadHandler.text}");

        }

        private static void LoadCredentials()
        {
            TextAsset configData = Resources.Load<TextAsset>("Config/config");

            if (configData)
            {
                string configJson = configData.text;
                Credentials config = JsonUtility.FromJson<Credentials>(configJson);

                _databaseUser = config.username;
                _password = config.password;
                _postUri = config.uri;
                _getUri = config.uriGet;
            }
            else
            {
                throw new Exception("Config file not found!");
            }
        }
    }
}
