using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace GGG.Components.Menus
{
    [RequireComponent(typeof(Slider))]
    public class SoundSlider : MonoBehaviour
    {
        [SerializeField] private AudioMixer Mixer;
        [SerializeField] private TMP_Text VolumeText;
        [SerializeField] private string PrefsName;
        [SerializeField] private string FloatName;

        private Slider _slider;

        public void Initialize()
        {
            if (!_slider) _slider = GetComponent<Slider>();
            
            _slider.value = PlayerPrefs.GetFloat(PrefsName, 1);
            SetVolume(_slider.value);
            UpdateText();
        }

        private void OnApplicationQuit()
        {
            PlayerPrefs.SetFloat(PrefsName, _slider.value);
        }

        private float GetRange(float max, float min, float value)
        {
            return Mathf.Abs(value - min) / (max - min) * 100;
        }

        public void SetVolume(float volume)
        {
            Mixer.SetFloat(FloatName, Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat(PrefsName, volume);
            UpdateText();
        }

        private void UpdateText()
        {
            VolumeText.SetText(Mathf.FloorToInt(GetRange(_slider.maxValue, _slider.minValue, _slider.value)).ToString());
        }
    }
}
