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
            SetView(_tempAudioSettings);
            
            SettingsAction.SetMasterVolume?.Invoke(_tempAudioSettings.MasterVolume);
            SettingsAction.SetMusicVolume?.Invoke(_tempAudioSettings.MusicVolume);
            SettingsAction.SetSFXVolume?.Invoke(_tempAudioSettings.SFXVolume);
        }
        
        public void SetView(AudioSettingsConfig config)
        {
            _tempAudioSettings = config;

            var masterValueFloat = ConverterValue.ConvertToFloat(config.MasterVolume);
            _masterSlider.value = masterValueFloat;
            _masterValueText.text = config.MasterVolume.ToString();
            
            var musicValueFloat = ConverterValue.ConvertToFloat(config.MusicVolume);
            _musicSlider.value = musicValueFloat;
            _musicValueText.text = config.MusicVolume.ToString();
            
            var sfxValueFloat = ConverterValue.ConvertToFloat(config.SFXVolume);
            _sfxSlider.value = sfxValueFloat;
            _sfxValueText.text = config.SFXVolume.ToString();
        }
    }
}