using TMPro;
using UnityEngine;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class GraphicsDropdown : MonoBehaviour
    {
        private TMP_Dropdown _dropdown;

        private const string _PREFS_NAME = "Graphics";

        private void Start()
        {
            if (!_dropdown) _dropdown = GetComponent<TMP_Dropdown>();

            _dropdown.value = PlayerPrefs.HasKey(_PREFS_NAME) ? PlayerPrefs.GetInt(_PREFS_NAME) : 3;
        }

        public void OnDropdownChange(int option)
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
            
            PlayerPrefs.SetInt(_PREFS_NAME, option);
        }
    }
}
