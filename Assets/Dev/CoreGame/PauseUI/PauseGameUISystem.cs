using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.UI;
using CyberNet.Meta;
using CyberNet.Meta.EndGame;
using CyberNet.Global.Settings;

namespace CyberNet.Core.PauseUI
{
    [EcsSystem(typeof(CoreModule))]
    public class PauseGameUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            PauseGameAction.OpenUIPauseGame += OpenUIPauseGame;
            PauseGameAction.CloseUIPauseGame += CloseUIPauseGame;

            PauseGameAction.ResumeGame += ResumeGame;
            PauseGameAction.SettingsGame += SettingsGame;
            PauseGameAction.ReturnMenu += ReturnGame;
            PauseGameAction.ConfimReturnMenu += ConfimReturnMenu;
            PauseGameAction.QuitGame += QuitGame;
            
            PauseGameAction.ShowPanelUIPauseGame += ShowPanel;
            PauseGameAction.HidePanelUIPauseGame += HidePanel;
        }
        
        private void HidePanel()
        {
            ref var pauseUI = ref _dataWorld.OneData<CoreGameUIData>().PauseGameUIMono;
            pauseUI.HidePanelMenu();
        }

        private void ShowPanel()
        {
            ref var pauseUI = ref _dataWorld.OneData<CoreGameUIData>().PauseGameUIMono;
            pauseUI.ShowPanelMenu();
        }
        
        private void ConfimReturnMenu()
        {
            PopupAction.CloseConfirmPopup?.Invoke();
            EndGameAction.EndGame?.Invoke();
        }
        private void ReturnGame()
        {
            ref var popupViewConfig = ref _dataWorld.OneData<PopupData>().PopupViewConfig;
            var popupView = new PopupConfimStruct();
            popupView.HeaderLoc = popupViewConfig.HeaderPopupReturnMenu;
            popupView.DescrLoc = popupViewConfig.DescrReturnMenu;
            popupView.ButtonConfimLoc = popupViewConfig.ConfimButtonPopupReturnMenu;
            popupView.ButtonCancelLoc = popupViewConfig.CancelButtonPopupReturnMenu;
            popupView.ButtonConfimAction = PauseGameAction.ConfimReturnMenu;
            popupView.ButtonCancelAction = PauseGameAction.ShowPanelUIPauseGame;
            PopupAction.ConfirmPopup?.Invoke(popupView);
            PauseGameAction.OpenPopupExitGame?.Invoke();
            HidePanel();
        }
        private void SettingsGame()
        {
            HidePanel();
            SettingsAction.OpenSettingsUI?.Invoke();
        }
        
        private void ResumeGame()
        {
            PauseGameAction.OffPauseGame?.Invoke();
        }
        
        private void OpenUIPauseGame()
        {
            ref var pauseUI = ref _dataWorld.OneData<CoreGameUIData>().PauseGameUIMono;
            pauseUI.OpenWindow();
        }
        
        private void CloseUIPauseGame()
        {
            ref var pauseUI = ref _dataWorld.OneData<CoreGameUIData>().PauseGameUIMono;
            pauseUI.CloseWindow();
        }
        
        private void QuitGame()
        {
            ref var popupViewConfig = ref _dataWorld.OneData<PopupData>().PopupViewConfig;
            var popupView = new PopupConfimStruct();
            popupView.HeaderLoc = popupViewConfig.HeaderPopupConfim;
            popupView.DescrLoc = popupViewConfig.DescrPopupConfim;
            popupView.ButtonConfimLoc = popupViewConfig.ConfimButtonPopupConfim;
            popupView.ButtonCancelLoc = popupViewConfig.CancelButtonPopupConfim;
            popupView.ButtonConfimAction = MainMenuAction.ExitGame;
            popupView.ButtonCancelAction = PauseGameAction.ShowPanelUIPauseGame;
            
            PopupAction.ConfirmPopup?.Invoke(popupView);
            PauseGameAction.OpenPopupExitGame?.Invoke();
            HidePanel();
        }

        public void Destroy()
        {
            PauseGameAction.OpenUIPauseGame -= OpenUIPauseGame;
            PauseGameAction.CloseUIPauseGame -= CloseUIPauseGame;

            PauseGameAction.ResumeGame -= ResumeGame;
            PauseGameAction.SettingsGame -= SettingsGame;
            PauseGameAction.ReturnMenu -= ReturnGame;
            PauseGameAction.ConfimReturnMenu -= ConfimReturnMenu;
            PauseGameAction.QuitGame -= QuitGame;
            
            PauseGameAction.ShowPanelUIPauseGame -= ShowPanel;
            PauseGameAction.HidePanelUIPauseGame -= HidePanel;
        }
    }
}