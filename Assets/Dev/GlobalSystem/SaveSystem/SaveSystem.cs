using CyberNet.Global.Settings;
using EcsCore;
using UnityEngine;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.SaveSystem
{
    [EcsSystem(typeof(MetaModule))]
    public class SaveSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        private string filenameSave = "CyberNet_Save.json";
        private string filenameSettings = "Settings.json";
        private string directory = "/";

        public void PreInit()
        {
            InitSettings();
            InitSaveGame();
            
            SaveAction.SaveGameToFile += Save;
            SaveAction.DeleteSaveGame += CreateNewSave;
            SaveAction.SaveSettingsGame += SaveSettings;
        }

        private void InitSaveGame()
        {
            _dataWorld.CreateOneData(new SaveData());
            
            if (SaveUtility.CheckFile(directory, filenameSave))
                LoadSave();
            else
                CreateNewSave();   
        }
        
        private void InitSettings()
        {
            _dataWorld.CreateOneData(new SettingsData());

            if (SaveUtility.CheckFile(directory, filenameSettings))
                LoadSettings();
            else
                CreateNewSettings();
        }

        private void CreateNewSave()
        {
            ref var saveData = ref _dataWorld.OneData<SaveData>();
            saveData.ProgressCompany = ProgressCompany.None;
            Save();
        }

        private void LoadSave()
        {
            ref var saveData = ref _dataWorld.OneData<SaveData>();
            var jsonPlayer = SaveUtility.Load(directory, filenameSave);
            saveData = JsonUtility.FromJson<SaveData>(jsonPlayer);
        }

        private void Save()
        {
            ref var saveData = ref _dataWorld.OneData<SaveData>();
            SaveUtility.Save(saveData, directory, filenameSave);
        }

        private void SaveSettings()
        {
            ref var saveSettings = ref _dataWorld.OneData<SettingsData>();
            SaveUtility.Save(saveSettings, directory, filenameSettings);
        }

        private void LoadSettings()
        {
            ref var settingsData = ref _dataWorld.OneData<SettingsData>();
            var jsonSettings = SaveUtility.Load(directory, filenameSettings);
            settingsData = JsonUtility.FromJson<SettingsData>(jsonSettings);
        }
        
        private void CreateNewSettings()
        {
            ref var saveData = ref _dataWorld.OneData<SettingsData>();
            saveData.AudioSettings = new AudioSettingsConfig();
            saveData.ControlsSettings = new ControlsSettingsConfig();
            saveData.VideoSettings = new VideoSettingsConfig();
            saveData.GameSettings = new GameSettingsConfig();
            
            saveData.GameSettings.IsShowDistrickPopup = true;
            saveData.AudioSettings.MasterVolume = 100;
            saveData.AudioSettings.MusicVolume = 100;
            saveData.AudioSettings.SFXVolume = 100;
            
            SaveSettings();
        }

        public void Destroy()
        {
            SaveAction.SaveGameToFile -= Save;
            SaveAction.DeleteSaveGame -= CreateNewSave;
            SaveAction.SaveSettingsGame -= SaveSettings;
        }
    }
}