using System;
using System.Collections;
using System.Collections.Generic;
using GGG.Components.Scenes;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace GGG.Components.Serialization
{
    public class DatabaseManager : MonoBehaviour
    {
        private SceneManagement _sceneManagement;


        [Serializable]
        private class Credentials
        {
            public string username;
            public string password;
            public string uri;
            public string uriGet;
            public string uriDelete;
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
        public class Stats
        {
            public string Name;
        }

        [Serializable]
        public class CityStats : Stats
        {
            public int PayedTaxes = 0;
            public int UnpayedTaxes = 0;
            public int SeaFarms = 0;
            public int FishFarms = 0;
            public int Laboratories = 0;
            public int Generators = 0;
            public int UnlockedBuildings = 0;
            public int UnlockedResources = 0;
            public int ShopExchanges = 0;
            public int PlayedTime = 0;
        }

        [Serializable]
        public class ExpeditionStats : Stats
        {
            public int Deads = 0;
            public int ResourcesTaken = 0;
        }

        [Serializable]
        private class JsonData<T>
        {
            public string result;
            public List<T> data;
        }



        private static User _currentUser;
        private static CityStats _currentUserCityStats;
        private static ExpeditionStats _currentUserExpeditionStats;

        private const string _CONTENT_TYPE = "application/json";

        private static string _databaseUser;
        private static string _password;
        private static string _postUri;
        private static string _getUri;
        private static string _deleteUri;

        private float _deltaTime;
        private bool _auxCorrutine;

        private const int _SAVE_TIME = 300;

        private void Start()
        {
            _sceneManagement = SceneManagement.Instance;
            _sceneManagement.OnMinigameSceneLoaded += HandleExpeditionStats;
            _sceneManagement.OnMinigameSceneUnloaded += OnExpeditionUnload;

            _sceneManagement.OnGameSceneLoaded += Initialize;
        }

        private void Update()
        {
            if (_deltaTime >= _SAVE_TIME)
            {
                AddPlayedTime();
                _deltaTime = 0;
            }

            _deltaTime += Time.deltaTime;
        }

        private void Initialize()
        {
            /*
            _buildingManager.OnBuildAdd += OnBuildingAdd;
            _laboratoryUI.OnResourceResearch += AddResourceUnlock;
            _laboratoryUI.OnBuildResearch += AddBuildingUnlock;
            TaxUI.OnTaxesPay += AddPayTaxes;
            TaxUI.OnTaxesNotPay += AddNotPayTaxes;
            ShopUI.OnExchange += AddExchange;
            */
        }

        
        private void HandleExpeditionStats()
        {
            //TriggerSensor.OnDead += AddDead;
            //ResourceComponent.OnResourcePicked += AddResourcesTaken;
        }

        private void OnExpeditionUnload()
        {
            //TriggerSensor.OnDead -= AddDead;
            //ResourceComponent.OnResourcePicked -= AddResourcesTaken;
        }
        

        private void OnBuildingAdd(string key, int amount) => StartCoroutine(AddBuildingStat(key, amount));


        private IEnumerator AddBuildingStat(string key, int amount)
        {

            switch (key)
            {
                case "Fishfarm":
                    _currentUserCityStats.FishFarms = amount;
                    break;
                case "Seafarm":
                    _currentUserCityStats.SeaFarms = amount;
                    break;
                case "Laboratory":
                    _currentUserCityStats.Laboratories = amount;
                    break;
                case "Generator":
                    _currentUserCityStats.Generators = amount;
                    break;
                default:
                    yield break;
            }

            if(_auxCorrutine) yield return new WaitWhile(() => _auxCorrutine);
            _auxCorrutine = true;
            yield return DeleteData(FindCityStatsJson(_currentUserCityStats.Name));
            yield return PostData(CreateCityStatsJson(_currentUserCityStats));
            _auxCorrutine = false;
        }

        private void AddResourceUnlock()
        {
            _currentUserCityStats.UnlockedResources++;
            StartCoroutine(UpdateCityStats());
        }

        private void AddBuildingUnlock()
        {
            _currentUserCityStats.UnlockedBuildings++;
            StartCoroutine(UpdateCityStats());
        }

        private void AddPayTaxes()
        {
            _currentUserCityStats.UnlockedBuildings++;
            StartCoroutine(UpdateCityStats());

        }

        private void AddNotPayTaxes()
        {
            _currentUserCityStats.UnlockedBuildings++;
            StartCoroutine(UpdateCityStats());
        }

        private void AddExchange()
        {
            _currentUserCityStats.ShopExchanges++;
            StartCoroutine(UpdateCityStats());
        }

        private void AddDead()
        {
            _currentUserExpeditionStats.Deads++;
            StartCoroutine(UpdateExpeditionStats());
        }

        private void AddResourcesTaken()
        {
            _currentUserExpeditionStats.ResourcesTaken++;
            StartCoroutine(UpdateExpeditionStats());
        }

        private void AddPlayedTime()
        {
            _currentUserCityStats.PlayedTime += _SAVE_TIME;
            StartCoroutine(UpdateCityStats());
        }

        private IEnumerator UpdateCityStats()
        {
            yield return DeleteData(FindCityStatsJson(_currentUserCityStats.Name));
            yield return PostData(CreateCityStatsJson(_currentUserCityStats));
        }

        private IEnumerator UpdateExpeditionStats()
        {
            yield return DeleteData(FindExpeditionStatsJson(_currentUserCityStats.Name));
            yield return PostData(CreateExpeditionJson(_currentUserExpeditionStats));
        }

        public IEnumerator ResetStats()
        {
            _currentUserExpeditionStats = new ExpeditionStats { Name = _currentUserExpeditionStats.Name };
            _currentUserCityStats = new CityStats { Name = _currentUserCityStats.Name };
            yield return UpdateExpeditionStats();
            yield return UpdateCityStats();
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

        public static string CreateCityStatsJson(CityStats stats)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""UsersCityStats"",
                ""data"": {{
                    ""name"": ""{stats.Name}"",
                    ""payedTaxes"": ""{stats.PayedTaxes}"",
                    ""unpayedTaxes"": ""{stats.UnpayedTaxes}"",
                    ""seaFarms"": ""{stats.SeaFarms}"",
                    ""fishFarms"": ""{stats.FishFarms}"",
                    ""laboratories"": ""{stats.Laboratories}"",
                    ""generators"": ""{stats.Generators}"",
                    ""unlockedBuildings"": ""{stats.UnlockedBuildings}"",
                    ""unlockedResources"": ""{stats.UnlockedResources}"",
                    ""shopExchanges"": ""{stats.ShopExchanges}"",
                    ""time"": ""{stats.PlayedTime}""
                }}
            }}";

            return json;
        }

        public static string CreateExpeditionJson(ExpeditionStats stats)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""UsersExpeditionStats"",
                ""data"": {{
                    ""name"": ""{stats.Name}"",
                    ""deads"": ""{stats.Deads}"",
                    ""resourcesTaken"": ""{stats.ResourcesTaken}""
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

        public static string FindCityStatsJson(string userName)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""UsersCityStats"",
                ""filter"":{{""name"":""{userName}""}}
            }}";

            return json;
        }

        public static string FindExpeditionStatsJson(string userName)
        {
            string json = $@"{{
                ""username"":""{_databaseUser}"",
                ""password"":""{_password}"",
                ""table"":""UsersExpeditionStats"",
                ""filter"":{{""name"":""{userName}""}}
            }}";

            return json;
        }

        public static User CurrentUser() => _currentUser;
        public static CityStats GetCityStats() => _currentUserCityStats;
        public static ExpeditionStats GetExpeditionStats() => _currentUserExpeditionStats;
        public static void SetCurrentUser(User user) => _currentUser = user;
        public static void SetCurrentExpeditionStats(ExpeditionStats stats) => _currentUserExpeditionStats = stats;
        public static void SetCurrentCityStats(CityStats stats) => _currentUserCityStats = stats;

        public static IEnumerator GetRequest<T>(Action<bool, T> response, string data)
        {
            using UnityWebRequest www = UnityWebRequest.Post(_getUri, data, _CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception("Error at GET request: " + www.error);

            print($"GET Request Successful");

            T result = default;
            JsonData<T> json = JsonConvert.DeserializeObject<JsonData<T>>(www.downloadHandler.text);

            bool statsFound = json.data.Count > 0;
            if (statsFound) result = json.data[0];

            response?.Invoke(statsFound, result);
        }

        public static IEnumerator PostData(string data)
        {
            using UnityWebRequest www = UnityWebRequest.Post(_postUri, data, _CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception(www.error);

            print($"POST Request Successful");
        }

        public static IEnumerator DeleteData(string data)
        {
            using UnityWebRequest www = UnityWebRequest.Post(_deleteUri, data, _CONTENT_TYPE);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
                throw new Exception(www.error);

            print("DELETE Request Successful");
        }

        private static void LoadCredentials()
        {

            TextAsset configData = Resources.Load<TextAsset>(Application.isEditor
                ? "Config/config_editor"
                : "Config/config");

            if (configData)
            {
                string configJson = configData.text;
                Credentials config = JsonUtility.FromJson<Credentials>(configJson);

                _databaseUser = config.username;
                _password = config.password;
                _postUri = config.uri;
                _getUri = config.uriGet;
                _deleteUri = config.uriDelete;
            }
            else
            {
                throw new Exception("Config file not found!");
            }
        }
    }
}
