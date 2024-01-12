using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.AI;
using CyberNet.Global;
using CyberNet.Meta.SelectPlayersForGame;
using CyberNet.Meta.SettingsUI;
using CyberNet.Meta.StartGame;
using CyberNet.Platform;
using CyberNet.Tools;
using CyberNet.Tutorial;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CyberNet.Meta
{
    /// <summary>
    /// Система управляющая переключением окон в главном меню
    /// </summary>
    [EcsSystem(typeof(MetaModule))]
    public class MainMenuUISystem : IPreInitSystem, IInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            MainMenuAction.OpenMainMenu += OpenMainMenu;
            MainMenuAction.CloseMainMenu += CloseMainMenu;
            MainMenuAction.OpenCampaign += OpenCampaign;
            MainMenuAction.StartTutorial += StartTutorial;
            MainMenuAction.OpenLocalGame += OpenLocalGame;
            MainMenuAction.OpenServerGame += OpenServerGame;
            MainMenuAction.OpenSettingsGame += OpenSettingsGame;
            MainMenuAction.OpenExitGame += OpenExitGame;
            MainMenuAction.ExitGame += Exit;
        }

        public void Init()
        {
            #if STEAM && DEMO
            ShowPreviewDemoScreen();
            #else
            OpenMainMenu();
            #endif
        }

        private void ShowPreviewDemoScreen()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.PreviewStartDemoGameMono.OpenWindow();
        }
        
        private void StartTutorial()
        {
            CreatePlayerDataLocalGame();
            SelectPlayerAction.CreatePlayer?.Invoke(1);
            StartGameAction.StartTutorial?.Invoke();
            CloseMainMenu();
        }

        private void OpenCampaign()
        {
            CampaignUIAction.OpenCampaignUI?.Invoke();
            CloseMainMenu();
        }
        
        private void OpenExitGame()
        {
            #if STEAM && DEMO
            OpenPreviewDemoExitGame();
            return;
            #endif
            
            ref var popupViewConfig = ref _dataWorld.OneData<PopupData>().PopupViewConfig;
            var popupView = new PopupConfimStruct();
            popupView.HeaderLoc = popupViewConfig.HeaderPopupConfim;
            popupView.DescrLoc = popupViewConfig.DescrPopupConfim;
            popupView.ButtonConfimLoc = popupViewConfig.ConfimButtonPopupConfim;
            popupView.ButtonCancelLoc = popupViewConfig.CancelButtonPopupConfim;
            popupView.ButtonConfimAction = MainMenuAction.ExitGame;
            PopupAction.ConfirmPopup?.Invoke(popupView);
        }

        private void OpenPreviewDemoExitGame()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.PreviewEndDemoGameMono.OpenWindow();
        }
        
        private void OpenSettingsGame()
        {
            SettingsUIAction.OpenSettingsUI?.Invoke();
            CloseMainMenu();
        }
        
        private void OpenServerGame()
        {
            OnlineGameUIAction.OpenOnlineGameUI?.Invoke();
            CloseMainMenu();
        }

        private void OpenLocalGame()
        {
            var playerConfig = CreatePlayerDataLocalGame();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(playerConfig, true);
            CloseMainMenu();
        }

        private SelectLeaderData CreatePlayerDataLocalGame()
        {
            var selectPlayerData = new SelectPlayerData();
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            selectPlayerData.SelectLeaders = new();
            var playerName = "";
            playerName = PlatformAction.GetPlayerName?.Invoke();
            
            var playerLeaderData = new SelectLeaderData {
                PlayerID = 0,
                PlayerOrAI = PlayerOrAI.Player,
                SelectLeader = "cyberpsycho",
                NamePlayer = playerName,
                KeyVisualCity = cityVisualSO.PlayerVisualKeyList[0]
            };
            selectPlayerData.SelectLeaders.Add(playerLeaderData);

            _dataWorld.CreateOneData(selectPlayerData);
            return playerLeaderData;
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
        
        public void Exit()
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