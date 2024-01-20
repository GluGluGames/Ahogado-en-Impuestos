using System;
using GGG.Components.Core;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization;

namespace GGG.Components.Serialization.Login
{
    public class LogInManager : MonoBehaviour
    {
        [Space(5), Header("USERNAME PANEL")] 
        [SerializeField] private GameObject LoginPanel;
        [SerializeField] private TMP_InputField UserNameInput;
        [SerializeField] private TMP_InputField PasswordInput;
        [SerializeField] private TMP_Text LoginErrorText;
        [SerializeField] private Button LoginButton;
        [SerializeField] private Button RegisterButton;
        [Space(5), Header("REGISTER PANEL")]
        [SerializeField] private GameObject RegisterPanel;
        [SerializeField] private LoginBackButton RegisterBackButton;
        [SerializeField] private TMP_InputField RegisterUsernameInput;
        [SerializeField] private TMP_InputField RegisterPasswordInput;
        [SerializeField] private TMP_Text RegisterErrorText;
        [SerializeField] private Button ConfirmRegisterButton;
        [Header("AGE")]
        [SerializeField] private TMP_InputField AgeInput;
        [Header("GENRE")]
        [SerializeField] private Button MaleButton;
        [SerializeField] private Button FemaleButton;
        [SerializeField] private GameObject MaleSelected;
        [SerializeField] private GameObject FemaleSelected;
        [Header("ERRORS")]
        [SerializeField] private LocalizedString IncorrectPassword;
        [SerializeField] private LocalizedString MissingData;
        [SerializeField] private LocalizedString UsernameTaken;
        [SerializeField] private LocalizedString AgeInvalid;

        // private SerializationManager.User _currentUser;

        /*
        private void Start()
        {
            // _currentUser = new SerializationManager.User();
            //_currentUser.Gender = -1;
            if (!FindObjectOfType<SerializationManager>())
                throw new Exception("Not Serialization Manager Found");

            LoginPanel.SetActive(true);
            RegisterPanel.SetActive(false);

            LoginErrorText.SetText("");
            RegisterErrorText.SetText("");

            MaleSelected.SetActive(false);
            FemaleSelected.SetActive(false);

            RegisterBackButton.OnButtonClick += Back;

            AgeInput.onValueChanged.AddListener(OnAgeChange);

            LoginButton.onClick.AddListener(OnPasswordConfirm);
            RegisterButton.onClick.AddListener(OnRegister);
            ConfirmRegisterButton.onClick.AddListener(OnRegisterValidate);
            MaleButton.onClick.AddListener(() => OnGenderSelect(true));
            FemaleButton.onClick.AddListener(() => OnGenderSelect(false));
        }

        private void OnRegister()
        {
            LoginPanel.SetActive(false);
            UserNameInput.text = "";
            PasswordInput.text = "";
            LoginErrorText.SetText("");
            RegisterUsernameInput.text = "";
            RegisterPasswordInput.text = "";
            AgeInput.text = "";
            RegisterErrorText.SetText("");
            RegisterPanel.SetActive(true);
        }

        public void OnGenderSelect(bool male)
        {
            MaleSelected.SetActive(male);
            FemaleSelected.SetActive(!male);
            _currentUser.Gender = male ? 1 : 0;
        }

        private void Back()
        {
            MaleSelected.SetActive(false);
            FemaleSelected.SetActive(false);
            _currentUser.Gender = -1;
            _currentUser.Name = "";
            _currentUser.Password = "";
            _currentUser.Age = -1;
        }

        private void OnAgeChange(string age) {
            if (string.IsNullOrEmpty(age)) return;

            _currentUser.Age = int.Parse(age); 
        }

        private void LoadMainMenu()
        {
            SceneManagement.Instance.AddSceneToLoad(SceneIndexes.MAIN_MENU);
            SceneManagement.Instance.AddSceneToUnload(SceneIndexes.MAIN_MENU);
            SceneManagement.Instance.UpdateScenes();
        }

        private void OnRegisterValidate()
        {
            if (string.IsNullOrEmpty(RegisterPasswordInput.text) || string.IsNullOrEmpty(RegisterUsernameInput.text)
                || string.IsNullOrEmpty(AgeInput.text) || _currentUser.Gender == -1)
            {
                RegisterErrorText.SetText(MissingData.GetLocalizedString());
                return;
            }

            if(int.Parse(AgeInput.text) > 99 || int.Parse(AgeInput.text) <= 0)
            {
                RegisterErrorText.SetText(AgeInvalid.GetLocalizedString());
                return;
            }

            _currentUser.Name = RegisterUsernameInput.text;
            _currentUser.Password = RegisterPasswordInput.text;

            StartCoroutine(SerializationManager.GetRequest<SerializationManager.User>((found, user) =>
            {
                if (found)
                {
                    RegisterErrorText.SetText(UsernameTaken.GetLocalizedString());
                    return;
                }

                _currentUser.Name = RegisterUsernameInput.text;
                _currentUser.Password = RegisterPasswordInput.text;
                StartCoroutine(SerializationManager.PostData(SerializationManager.CreateUserJson(_currentUser.Name,
                    _currentUser.Age, _currentUser.Gender, _currentUser.Password)));
                SerializationManager.SetCurrentUser(_currentUser);

                SerializationManager.CityStats newCityStats = new SerializationManager.CityStats { Name = _currentUser.Name };
                SerializationManager.ExpeditionStats newExpeditionStats = new SerializationManager.ExpeditionStats { Name = _currentUser.Name };
                StartCoroutine(SerializationManager.PostData(SerializationManager.CreateCityStatsJson(newCityStats)));
                StartCoroutine(SerializationManager.PostData(SerializationManager.CreateExpeditionJson(newExpeditionStats)));

                SerializationManager.SetCurrentCityStats(newCityStats);
                SerializationManager.SetCurrentExpeditionStats(newExpeditionStats);

                LoadMainMenu();

            }, SerializationManager.FindUsersJson(_currentUser.Name)));

            
        }

        private void OnPasswordConfirm()
        {
            if (string.IsNullOrEmpty(PasswordInput.text) || string.IsNullOrEmpty(UserNameInput.text))
            {
                LoginErrorText.SetText(MissingData.GetLocalizedString());
                return;
            }

            _currentUser.Name = UserNameInput.text;
            _currentUser.Password = PasswordInput.text;

            StartCoroutine(SerializationManager.GetRequest<SerializationManager.User>((found, user) =>
                {
                    if (!found) 
                    {
                        LoginErrorText.SetText(IncorrectPassword.GetLocalizedString());
                        return; 
                    }

                    SerializationManager.SetCurrentUser(user);

                    StartCoroutine(SerializationManager.GetRequest<SerializationManager.CityStats>((found, stats) =>
                    {
                        SerializationManager.CityStats newCityStats = null;
                        if (!found)
                        {
                            newCityStats = new SerializationManager.CityStats { Name = _currentUser.Name };
                            StartCoroutine(SerializationManager.PostData(SerializationManager.CreateCityStatsJson(newCityStats)));
                        }

                        SerializationManager.SetCurrentCityStats(found ? stats : newCityStats);
                        
                    }, SerializationManager.FindCityStatsJson(_currentUser.Name)));

                    StartCoroutine(SerializationManager.GetRequest<SerializationManager.ExpeditionStats>((found, stats) =>
                    {
                        SerializationManager.ExpeditionStats newExpeditionStats = null;
                        if (!found)
                        {
                            newExpeditionStats = new SerializationManager.ExpeditionStats { Name = _currentUser.Name };
                            StartCoroutine(SerializationManager.PostData(SerializationManager.CreateExpeditionJson(newExpeditionStats)));
                        }

                        SerializationManager.SetCurrentExpeditionStats(found ? stats : newExpeditionStats);
                    }, SerializationManager.FindExpeditionStatsJson(_currentUser.Name)));

                    LoadMainMenu();
                },
                SerializationManager.FindPasswordsJson(_currentUser.Name, _currentUser.Password)));
            
        }
        */
    }
}