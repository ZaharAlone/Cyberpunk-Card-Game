using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using ModulesFrameworkUnity;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using CyberNet.Global;
using CyberNet.Server;

namespace CyberNet.Meta
{
    /// <summary>
    /// Система управляющая переключением окон в главном меню
    /// </summary>
    [EcsSystem(typeof(MetaModule))]
    public class MainMenuUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            MainMenuAction.OpenMainMenu += OpenMainMenu;
            MainMenuAction.CloseMainMenu += CloseMainMenu;
            MainMenuAction.OpenCampaign += OpenCampaign;
            MainMenuAction.OpenLocalGameVSAI += OpenLocalGameVSAI;
            MainMenuAction.OpenLocalGameVSPlayer += OpenLocalGameVSPlayer;
            MainMenuAction.OpenServerGame += OpenServerGame;
            MainMenuAction.OpenSettingsGame += OpenSettingsGame;
            MainMenuAction.OpenExitGame += OpenExitGame;
        }
        private void OpenCampaign()
        {
            CampaignUIAction.OpenCampaignUI?.Invoke();
            CloseMainMenu();
        }
        
        private void OpenExitGame()
        {

        }
        
        private void OpenSettingsGame()
        {

        }
        
        private void OpenServerGame()
        {
            OnlineGameUIAction.OpenOnlineGameUI?.Invoke();
            CloseMainMenu();
        }
        
        private void OpenLocalGameVSPlayer()
        {
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(GameModeEnum.LocalVSPlayer);
            CloseMainMenu();
        }
        
        private void OpenLocalGameVSAI()
        {
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(GameModeEnum.LocalVSAI);
            CloseMainMenu();
        }
        
        private void CloseMainMenu()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.MainMenuUIMono.CloseMainMenu();
        }

        private void OpenMainMenu()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.MainMenuUIMono.OpenMainMenu();
        }
        
        public void Exti()
        {
#if UNITY_EDITOR
            EditorApplication.isPlaying = false;
#else
		        Application.Quit();
#endif
        }

        public void GoToWebPage(string url)
        {
            Application.OpenURL(url);
        }
    }
}