using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.SaveSystem;
using FMOD.Studio;

namespace CyberNet.Global.Settings
{
    [EcsSystem(typeof(GlobalModule))]
    public class SettingParameterSystem : IPreInitSystem, IInitSystem, IDestroySystem
    {
        private Bus _busMaster;
        private Bus _busMusic;
        private Bus _busSFX;
        
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SettingsAction.SetShowDistrictPopup += SetShowDistrictPopup;
            SettingsAction.SetMasterVolume += SetMasterVolume;
            SettingsAction.SetMusicVolume += SetMusicVolume;
            SettingsAction.SetSFXVolume += SetSFXVolume;
        }

        public void Init()
        {
            InitSound();
        }

        private void InitSound()
        {
            _busMaster = FMODUnity.RuntimeManager.GetBus("bus:/");
            _busMusic = FMODUnity.RuntimeManager.GetBus("bus:/Music");
            _busSFX = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
            
            ref var audioSettings = ref _dataWorld.OneData<SettingsData>().AudioSettings;
            
            _busMaster.setVolume((float)audioSettings.MasterVolume / 100);
            _busMusic.setVolume((float)audioSettings.MusicVolume / 100);
            _busSFX.setVolume((float)audioSettings.SFXVolume / 100);
        }
        
        private void SetMasterVolume(int value)
        {
            ref var audioSettings = ref _dataWorld.OneData<SettingsData>().AudioSettings;
            audioSettings.MasterVolume = value;
            _busMaster.setVolume((float)value / 100);
            SaveSettings();
        }

        private void SetMusicVolume(int value)
        {
            ref var audioSettings = ref _dataWorld.OneData<SettingsData>().AudioSettings;
            audioSettings.MusicVolume = value;
            _busMusic.setVolume((float)value / 100);
            SaveSettings();
        }

        private void SetSFXVolume(int value)
        {
            ref var audioSettings = ref _dataWorld.OneData<SettingsData>().AudioSettings;
            audioSettings.SFXVolume = value;
            _busSFX.setVolume((float)value / 100);
            SaveSettings();
        }

        private void SetShowDistrictPopup(bool value)
        {
            ref var settingsData = ref _dataWorld.OneData<SettingsData>();
            settingsData.GameSettings.IsShowDistrickPopup = value;
            SaveSettings();
        }

        private void SaveSettings()
        {
            SaveAction.SaveSettingsGame?.Invoke();
        }
        
        public void Destroy() { }
    }
}