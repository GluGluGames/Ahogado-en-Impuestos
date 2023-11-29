using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using GGG.Components.Core;
using GGG.Shared;
using System.Collections;
using UnityEngine.Localization.Settings;

namespace GGG.Components.Menus
{
    public class Settings : MonoBehaviour
    {
        [Tooltip("AudioMixer.")]
        [SerializeField] private AudioMixer AudioMixer;

        [Space(5)]
        [Header("Toggles")]
        [Tooltip("Toggle which represents if the sound is active or not")]
        [SerializeField] private Toggle SoundToggle;

        [Space(5)]
        [Header("Sliders")]
        [Tooltip("Slider to change the general volume.")]
        [SerializeField] private Slider GeneralSlider;
        [Tooltip("Slider to change the music volume.")]
        [SerializeField] private Slider MusicSlider;
        [Tooltip("Slider to change the sound effects volume.")]
        [SerializeField] private Slider EffectsSlider;

        [Space(5)]
        [Header("Text Fields")]
        [Tooltip("Text that shows the general volume value.")]
        [SerializeField] private TextMeshProUGUI GeneralVolumeText;
        [Tooltip("Text that shows the music volume value.")]
        [SerializeField] private TextMeshProUGUI MusicText;
        [Tooltip("Text that shows the sound effects volume value.")]
        [SerializeField] private TextMeshProUGUI SoundEffectsText;

        [Space(5)]
        [Header("Dropdowns")]
        [Tooltip("Dropdown of the languages")]
        [SerializeField] private TMP_Dropdown LanguageDropdown;
        [Tooltip("Dropdown of the graphics")]
        [SerializeField] private TMP_Dropdown GraphicsDropdown;

        [Space(5)] 
        [Header("Buttons")] 
        [Tooltip("Close Button")] 
        [SerializeField] private Button CloseButton;
        [Tooltip("Credits Button")] 
        [SerializeField] private Button CreditsButton;
        [Tooltip("ExitButton")] 
        [SerializeField] private Button ExitGameButton;

        private bool _languageActive = false;

        #region Unity Methods
        private void Start()
        {
            StartSounds();
            LanguageDropdown.value = GameManager.Instance.GetCurrentLanguage() == Language.Spanish ? 0 : 1;
            
            CloseButton.onClick.AddListener(OnCloseButton);
            CreditsButton.onClick.AddListener(OnCreditsButton);
            ExitGameButton.onClick.AddListener(OnExitGame);
        }

        #endregion

        #region Methods

        /// <summary>
        /// It initializes all the volumes, the sliders and the texts.
        /// To a preset value if the haven't been changed previously.
        /// Or to the saved values if they have been changed.
        /// </summary>
        private void StartSounds()
        {
            AudioMixer.SetFloat("Volume", PlayerPrefs.GetFloat("GeneralVolume"));

            if (SoundManager.Instance.GetMusicActive())
                AudioMixer.SetFloat("Music", PlayerPrefs.GetFloat("MusicVolume"));
            else
                AudioMixer.SetFloat("Music", -50);

            AudioMixer.SetFloat("SoundEffects", PlayerPrefs.GetFloat("SoundEffectsVolume"));

            GeneralSlider.value = PlayerPrefs.GetFloat("GeneralVolume");
            MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
            EffectsSlider.value = PlayerPrefs.GetFloat("SoundEffectsVolume");

            UpdateText();
        }

        /// <summary>
        /// Updates the texts so the reflect the value of the volumes.
        /// </summary>
        private void UpdateText()
        {
            GeneralVolumeText.text = Mathf.FloorToInt(GetRange(GeneralSlider.maxValue, GeneralSlider.minValue, GeneralSlider.value)).ToString();
            MusicText.text = Mathf.FloorToInt(GetRange(MusicSlider.maxValue, MusicSlider.minValue, MusicSlider.value)).ToString();
            SoundEffectsText.text = Mathf.FloorToInt(GetRange(EffectsSlider.maxValue, EffectsSlider.minValue, EffectsSlider.value)).ToString();
        }

        public void DropdownLanguage(int option)
        {
            StartCoroutine(SetLocale(option));
        }

        public void DropdownGraphics(int option)
        {
            switch (option)
            {
                case 0:
                    QualitySettings.SetQualityLevel(1);
                    break;
                case 1:
                    QualitySettings.SetQualityLevel(2);
                    break;
                case 2:
                    QualitySettings.SetQualityLevel(3);
                    break;
                case 3:
                    QualitySettings.SetQualityLevel(4);
                    break;
            }

        }

        private void OnCloseButton()
        {
            SceneManagement.Instance.CloseSettings();
            if(GameManager.Instance.IsOnUI()) GameManager.Instance.OnUIClose();
        }

        private void OnCreditsButton()
        {
            SceneManagement.Instance.OpenCredits();
        }

        private void OnExitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Getters & Setters
        private float GetRange(float max, float min, float value)
        {
            return Mathf.Abs(value - min) / (max - min) * 100;
        }

        /// <param name="volume">New value of the general volume.</param>
        public void SetVolume(float volume)
        {
            AudioMixer.SetFloat("Volume", volume);
            PlayerPrefs.SetFloat("GeneralVolume", volume);
            UpdateText();
        }

        /// <param name="volume">New value of the music volume.</param>
        public void SetMusicVolume(float volume)
        {
            if (!SoundManager.Instance.GetMusicActive()) return;

            AudioMixer.SetFloat("Music", volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            UpdateText();
        }

        /// <param name="volume">New value of the sound effects volume.</param>
        public void SetSoundEffectsVolume(float volume)
        {
            AudioMixer.SetFloat("SoundEffects", volume);
            PlayerPrefs.SetFloat("SoundEffectsVolume", volume);
            UpdateText();
        }

        private IEnumerator SetLocale(int localeId)
        {
            _languageActive = true;
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
            PlayerPrefs.SetInt("LocalKey", localeId);

            GameManager.Instance.SetLanguage((Language) localeId);

            _languageActive = false;

        }

        #endregion
    }
}