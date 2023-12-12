using System;
using GGG.Components.Core;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GGG.Components.Serialization.Login
{
    public class LogInManager : MonoBehaviour
    {
        [Space(5), Header("GENDER PANEL")]
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
        [SerializeField] private LoginBackButton PasswordBackButton;
        [SerializeField] private TMP_InputField PasswordInput;
        [SerializeField] private TMP_InputField PasswordConfirmInput;
        [SerializeField] private Button ConfirmPasswordButton;

        private SerializationManager.User _currentUser;
        private int _currentAge = 10;

        private void Start()
        {
            _currentUser = new SerializationManager.User();
            if (!FindObjectOfType<SerializationManager>())
                throw new Exception("Not Serialization Manager Found");

            UsernamePanel.SetActive(true);
            GenderPanel.SetActive(false);
            AgePanel.SetActive(false);
            PasswordPanel.SetActive(false);

            AgeText.SetText(_currentAge.ToString());

            AgeIncreaseButton.onClick.AddListener(OnAgeIncrease);
            AgeDecreaseButton.onClick.AddListener(OnAgeDecrease);
            AgeSelectButton.onClick.AddListener(OnAgeSelect);

            UsernameSelectButton.onClick.AddListener(OnUsernameSelect);
        }

        public void OnGenderSelect(bool male)
        {
            _currentUser.Gender = male ? 1 : 0;
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
            _currentUser.Age = _currentAge;
            AgePanel.SetActive(false);

            PasswordPanel.SetActive(true);
            PasswordConfirmInput.gameObject.SetActive(true);
        }

        private void LoadMainMenu()
        {
            SceneManagement.Instance.AddSceneToLoad(SceneIndexes.MAIN_MENU);
            SceneManagement.Instance.AddSceneToUnload(SceneIndexes.LOGIN_SCENE);
            SceneManagement.Instance.UpdateScenes();
        }

        private void OnUsernameSelect()
        {
            if (string.IsNullOrEmpty(UserNameInput.text)) return;

            _currentUser.Name = UserNameInput.text;
            StartCoroutine(SerializationManager.GetUserData((x, y) =>
            {
                GenderPanel.SetActive(!x);
                PasswordPanel.SetActive(x);
                ConfirmPasswordButton.onClick.AddListener(x ? OnPasswordConfirm : OnPasswordValidate);
                
                PasswordBackButton.SetLastPanel(!x ? AgePanel : UsernamePanel);
                UserNameInput.text = "";
                UsernamePanel.SetActive(false);
            }, SerializationManager.FindUsersJson(_currentUser.Name)));

            
        }

        private void OnPasswordValidate()
        {
            if (string.IsNullOrEmpty(PasswordInput.text) || string.IsNullOrEmpty(PasswordConfirmInput.text) ||
                PasswordInput.text != PasswordConfirmInput.text)
            {
                // TODO - Error message
                return;
            }

            _currentUser.Password = PasswordInput.text;
            StartCoroutine(SerializationManager.PostUser(SerializationManager.CreateUserJson(_currentUser.Name,
                _currentUser.Age, _currentUser.Gender, _currentUser.Password)));
            ConfirmPasswordButton.onClick.RemoveAllListeners();
            
            LoadMainMenu();
        }

        private void OnPasswordConfirm()
        {
            if (string.IsNullOrEmpty(PasswordInput.text)) return;
            _currentUser.Password = PasswordInput.text;

            StartCoroutine(SerializationManager.GetUserData((x, y) =>
                {
                    print(x ? "Password correct" : "Password incorrect"); // TODO - Error message
                    if (!x) return;
                    
                    SerializationManager.SetCurrentUser(y);
                    ConfirmPasswordButton.onClick.RemoveAllListeners();
                    LoadMainMenu();
                },
                SerializationManager.FindPasswordsJson(_currentUser.Name, _currentUser.Password)));
            
            
        }
    }
}