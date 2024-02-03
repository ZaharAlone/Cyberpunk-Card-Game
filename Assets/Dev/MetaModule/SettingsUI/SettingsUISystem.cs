using CyberNet.Core.PauseUI;
using CyberNet.SaveSystem;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Meta.SettingsUI
{
    [EcsSystem(typeof(MetaModule))]
    public class SettingsUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SettingsUIAction.OpenSettingsUI += OpenSettingsUI;
            SettingsUIAction.CloseSettingsUI += CloseSettingsUI;
            
            SettingsUIAction.SetShowDistrictPopup += SetShowDistrictPopup;
        }
        private void OpenSettingsUI()
        {
            ref var settingsUI = ref _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            UpdateViewSettings();
            settingsUI.OpenWindow();
        }

        private void UpdateViewSettings()
        {
            var settingsData = _dataWorld.OneData<SettingsData>();
            ref var settingsUI = ref _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            
            settingsUI.SetViewDistrict(settingsData.IsShowDistrickPopup);
        }

        private void SetShowDistrictPopup(bool value)
        {
            ref var settingsData = ref _dataWorld.OneData<SettingsData>();
            settingsData.IsShowDistrickPopup = value;
            SaveAction.SaveSettingsGame?.Invoke();
        }
        
        private void CloseSettingsUI()
        {
            ref var settingsUI = ref _dataWorld.OneData<MetaUIData>().SettingsUIMono;

            if (_dataWorld.IsModuleActive<CoreModule>())
            {
                PauseGameAction.ShowPanelUIPauseGame?.Invoke();
            }
            else
            {
                MainMenuAction.OpenMainMenu?.Invoke();
            }
            
            settingsUI.CloseWindow();
        }
    }
}