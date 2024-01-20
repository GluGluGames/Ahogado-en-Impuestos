using System.Collections;
using GGG.Components.Core;
using GGG.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LanguageDropdown : MonoBehaviour
    {
        private TMP_Dropdown _language;
        
        private void Start()
        {
            if(!_language) _language = GetComponent<TMP_Dropdown>();
            
            _language.value = GameManager.Instance.GetCurrentLanguage() == Language.Spanish ? 0 : 1;
        }

        public void OnDropdownChange(int id)
        {
            StartCoroutine(SetLocale(id));
        }
        
        private static IEnumerator SetLocale(int localeId)
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
            PlayerPrefs.SetInt("LocalKey", localeId);

            GameManager.Instance.SetLanguage((Language) localeId);
        }
    }
}
