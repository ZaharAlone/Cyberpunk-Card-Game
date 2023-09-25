using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.AI;
using CyberNet.Global;
using CyberNet.Platform;
using CyberNet.Tools;
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
            #endif
        }

        private void ShowPreviewDemoScreen()
        {
            ref var metaUI = ref _dataWorld.OneData<MetaUIData>().MetaUIMono;
            metaUI.PreviewStartDemoGameMono.OpenWindow();
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

        }
        
        private void OpenServerGame()
        {
            OnlineGameUIAction.OpenOnlineGameUI?.Invoke();
            CloseMainMenu();
        }

        private void OpenLocalGame()
        {
            var playerConfig = CreatePlayerDataLocalGame();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(playerConfig);
            CloseMainMenu();
        }

        private SelectLeaderData CreatePlayerDataLocalGame()
        {
            var selectPlayerData = new SelectPlayerData();
            var leadersConfig = _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            ref var botNames = ref _dataWorld.OneData<BotConfigData>().BotNameList;
            selectPlayerData.SelectLeaders = new();
            var playerName = "";
            playerName = PlatformAction.GetPlayerName?.Invoke();
            
            var playerLeaderData = new SelectLeaderData {
                PlayerID = 0,
                PlayerType = PlayerType.Player,
                SelectLeader = "cyberpsycho",
                NamePlayer = playerName,
                KeyVisualCity = cityVisualSO.PlayerVisualKeyList[0]
            };
            selectPlayerData.SelectLeaders.Add(playerLeaderData);
            
            var enemyLeaders = GeneratePlayerData.GetRandomLeader(leadersConfig, 3);
            
            for (int i = 1; i < 4; i++)
            {
                var botName = GeneratePlayerData.GenerateUniquePlayerName(botNames, selectPlayerData.SelectLeaders);
                
                selectPlayerData.SelectLeaders.Add(new SelectLeaderData {
                    PlayerID = i,
                    PlayerType = PlayerType.AIEasy,
                    SelectLeader = enemyLeaders[i-1],
                    NamePlayer = botName,
                    KeyVisualCity = cityVisualSO.PlayerVisualKeyList[i]
                });
            }

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