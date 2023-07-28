using EcsCore;
using UnityEngine;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.SaveSystem
{
    [EcsSystem(typeof(MetaModule))]
    public class SaveSystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private string filename = "CyberNet_Save.json";
        private string directory = "/";

        public void PreInit()
        {
            _dataWorld.CreateOneData(new SaveData());
            
            if (SaveUtility.CheckFile(directory, filename))
                LoadSave();
            else
                CreateNewSave();

            SaveEvent.SaveGameToFile += Save;
            SaveEvent.DeleteSaveGame += CreateNewSave;
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
            var jsonPlayer = SaveUtility.Load(directory, filename);
            saveData.ProgressCompany = JsonUtility.FromJson<ProgressCompany>(jsonPlayer);
        }
        
        private void Save()
        {
            ref var saveData = ref _dataWorld.OneData<SaveData>();
            SaveUtility.Save(saveData.ProgressCompany, directory, filename);
        }
    }
}