using CyberNet.Core.PauseUI;
using CyberNet.Meta;
using CyberNet.SaveSystem;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;

namespace CyberNet.Global.Settings
{
    [EcsSystem(typeof(MetaModule))]
    public class SettingsUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SettingsAction.OpenSettingsUI += OpenSettingsUI;
            SettingsAction.CloseSettingsUI += CloseSettingsUI;
            
            SettingsAction.OpenGameTab += OpenGameTab;
            SettingsAction.OpenVideoTab += OpenVideoTab;
            SettingsAction.OpenAudioTab += OpenAudioTab;
            SettingsAction.OpenControlsTab += OpenControlsTab;
            SettingsAction.OpenCreditsTab += OpenCreditsTab;
        }
        
        private void OpenGameTab()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            var gameConfig = _dataWorld.OneData<SettingsData>().GameSettings;
            settingsUI.OpenGameTab(gameConfig);
        }

        private void OpenVideoTab()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            var videoConfig = _dataWorld.OneData<SettingsData>().VideoSettings;
            settingsUI.OpenVideoTab(videoConfig);
        }

        private void OpenAudioTab()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            var audioConfig = _dataWorld.OneData<SettingsData>().AudioSettings;
            settingsUI.OpenAudioTab(audioConfig);
        }

        private void OpenControlsTab()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            var controlConfig = _dataWorld.OneData<SettingsData>().ControlsSettings;
            settingsUI.OpenControlsTab(controlConfig);
        }

        private void OpenCreditsTab()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            settingsUI.OpenCreditsTab();
        }

        private void OpenSettingsUI()
        {
            var settingsUI = _dataWorld.OneData<MetaUIData>().SettingsUIMono;
            OpenGameTab();
            settingsUI.OpenWindow();
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

        public void Destroy()
        {
            SettingsAction.OpenSettingsUI -= OpenSettingsUI;
            SettingsAction.CloseSettingsUI -= CloseSettingsUI;
        }
    }
}