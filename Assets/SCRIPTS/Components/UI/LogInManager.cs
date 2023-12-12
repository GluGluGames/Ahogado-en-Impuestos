using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LogInManager : MonoBehaviour
{
    [Serializable]
    private class User
    {
        public int id;
        public string name;
        public int age;
        public int gender;
        public string password;
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
    private class GETUser
    {
        public string result;
        public List<User> data;
    }

    [Header("GENDER PANEL")]
    [SerializeField] private GameObject GenderPanel;
    [Space(5), Header("AGE PANEL")]
    [SerializeField] private GameObject AgePanel;
    [SerializeField] private TMP_Text AgeText;
    [SerializeField] private Button AgeIncreaseButton;
    [SerializeField] private Button AgeDecreaseButton;
    [SerializeField] private Button AgeSelectButton;
    [Space(5), Header("USERNAME PANEL")] 
    [SerializeField] private GameObject UsernamePanel;
    [SerializeField] private TMP_InputField UserNameInput;
    [SerializeField] private Button UsernameSelectButton;
    [Space(5), Header("PASSWORD PANEL")] 
    [SerializeField] private GameObject PasswordPanel;
    [SerializeField] private TMP_InputField PasswordInput;
    [SerializeField] private TMP_InputField PasswordConfirmInput;
    [SerializeField] private Button ConfirmPasswordButton;
    
    private const string _CONTENT_TYPE = "application/json";

    private string _databaseUser;
    private string _password;
    private string _postUri;
    private string _getUri;

    private User _currentUser;
    private int _currentAge = 10;

    private void Awake()
    {
        LoadCredentials();
    }

    private void Start()
    {
        _currentUser = new User();
        
        GenderPanel.SetActive(true);
        AgePanel.SetActive(false);
        UsernamePanel.SetActive(false);
        PasswordPanel.SetActive(false);
        
        AgeText.SetText(_currentAge.ToString());
        
        AgeIncreaseButton.onClick.AddListener(OnAgeIncrease);
        AgeDecreaseButton.onClick.AddListener(OnAgeDecrease);
        AgeSelectButton.onClick.AddListener(OnAgeSelect);
        
        UsernameSelectButton.onClick.AddListener(OnUsernameSelect);
    }

    public void OnGenderSelect(bool male)
    {
        _currentUser.gender = male ? 1 : 0;
        GenderPanel.SetActive(false);
        AgePanel.SetActive(true);
    }

    private void OnAgeIncrease()
    {
        if (_currentAge + 1 > 99) return;
        
        _currentAge++;
        AgeText.SetText(_currentAge.ToString());
    }

    private void OnAgeDecrease()
    {
        if (_currentAge - 1 < 0) return;
        
        _currentAge--;
        AgeText.SetText(_currentAge.ToString());
    }

    private void OnAgeSelect()
    {
        _currentUser.age = _currentAge;
        AgePanel.SetActive(false);
        UsernamePanel.SetActive(true);
    }

    private void OnUsernameSelect()
    {
        if (string.IsNullOrEmpty(UserNameInput.text)) return;
        
        _currentUser.name = UserNameInput.text;
        StartCoroutine(GetUser(x =>
        {
            PasswordConfirmInput.gameObject.SetActive(!x);
            ConfirmPasswordButton.onClick.AddListener(x ? OnPasswordConfirm : OnPasswordValidate);
        }, FindUsersJSON(), _currentUser.name));
        
        UsernamePanel.SetActive(false);
        PasswordPanel.SetActive(true);
    }

    private void OnPasswordValidate()
    {
        if (string.IsNullOrEmpty(PasswordInput.text) || string.IsNullOrEmpty(PasswordConfirmInput.text) || 
            PasswordInput.text != PasswordConfirmInput.text) return;

        _currentUser.password = PasswordInput.text;
        StartCoroutine(PostUser(CreateUserJSON(_currentUser.name, _currentUser.age, _currentUser.gender, _currentUser.password)));
        ConfirmPasswordButton.onClick.RemoveAllListeners();
    }

    private void OnPasswordConfirm()
    {
        if (string.IsNullOrEmpty(PasswordInput.text)) return;
        
        StartCoroutine(GetPassword(x =>
        {
            print($"Password correct -> {x}");
        }, FindUsersJSON(), PasswordInput.text));
    }

    private string CreateUserJSON(string userName, int userAge, int gender, string password)
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
    
    private string FindUsersJSON()
    {
        string json = $@"{{
            ""username"":""{_databaseUser}"",
            ""password"":""{_password}"",
            ""table"":""Users"",
            ""operation"":""select""
        }}";

        return json;
    }

    private IEnumerator GetUser(Action<bool> found, string data, string username)
    {
        using UnityWebRequest www = UnityWebRequest.Post(_getUri, data, _CONTENT_TYPE);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            throw new Exception("Error at GET request: " + www.error);
        
        print($"Get Request Result -> {www.downloadHandler.text}");
        
        List<User> users = JsonUtility.FromJson<GETUser>(www.downloadHandler.text).data;

        if (users.Find(x => x.name == username) != null)
        {
            print($"User found with username -> {username}");
            found?.Invoke(true);
        }
        else
        {
            print($"User not found with username -> {username}");
            found?.Invoke(false);
        }
        
    }
    
    private IEnumerator GetPassword(Action<bool> correct, string data, string password)
    {
        using UnityWebRequest www = UnityWebRequest.Post(_getUri, data, _CONTENT_TYPE);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            throw new Exception("Error at GET request: " + www.error);
        
        print($"Get Request Result -> {www.downloadHandler.text}");
        
        List<User> users = JsonUtility.FromJson<GETUser>(www.downloadHandler.text).data;

        bool passwordCorrect = users.Find(x => x.password == password) != null;
        correct?.Invoke(passwordCorrect);
        print(passwordCorrect ? $"Password correct -> {password}" : $"Password incorrect -> {password}");
    }

    private IEnumerator PostUser(string data)
    {
        using UnityWebRequest www = UnityWebRequest.Post(_postUri, data, _CONTENT_TYPE);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
            throw new Exception(www.error);

        print($"POST Successful -> {www.downloadHandler.text}");

    }

    private void LoadCredentials()
    {
        string configPath = "Assets/config.json";

        if (File.Exists(configPath))
        {
            string configJson = File.ReadAllText(configPath);
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