using CyberNet.Tools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CyberNet.Global.Settings
{
    public class AudioSettingsUIMono : MonoBehaviour
    {
        [SerializeField]
        private Slider _masterSlider;
        [SerializeField]
        private TextMeshProUGUI _masterValueText;
        [SerializeField]
        private Slider _musicSlider;
        [SerializeField]
        private TextMeshProUGUI _musicValueText;
        [SerializeField]
        private Slider _sfxSlider;
        [SerializeField]
        private TextMeshProUGUI _sfxValueText;

        private AudioSettingsConfig _tempAudioSettings;
        
        public void SetValueMaster(float value)
        {
            var intValue = ConverterValue.ConvertToInt(value);
            _masterValueText.text = intValue.ToString();
            SettingsAction.SetMasterVolume?.Invoke(intValue);
        }

        public void SetValueMusic(float value)
        {
            var intValue = ConverterValue.ConvertToInt(value);
            _musicValueText.text = intValue.ToString();
            SettingsAction.SetMusicVolume?.Invoke(intValue);
        }
        
        public void SetValueSFX(float value)
        {
            var intValue = ConverterValue.ConvertToInt(value);
            _sfxValueText.text = intValue.ToString();
            SettingsAction.SetSFXVolume?.Invoke(intValue);
        }

        public void OnClickCancelChanges()
        {
            
        }
        public void SetView(AudioSettingsConfig config)
        {
            
        }
    }
}