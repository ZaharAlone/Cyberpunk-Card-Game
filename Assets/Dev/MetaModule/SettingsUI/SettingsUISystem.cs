using CyberNet.Core.PauseUI;
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
        }
        private void OpenSettingsUI()
        {
            ref var settingsUI = ref _dataWorld.OneData<MetaUIData>().SettingsUIMono;
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
    }
}