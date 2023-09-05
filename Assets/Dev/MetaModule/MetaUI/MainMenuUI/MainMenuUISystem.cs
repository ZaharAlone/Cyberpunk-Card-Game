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
using System.Collections.Generic;
using System.Linq;
using CyberNet.Core;
using CyberNet.Global;
using CyberNet.Server;
using Random = UnityEngine.Random;

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
            MainMenuAction.OpenLocalGame += OpenLocalGame;
            MainMenuAction.OpenServerGame += OpenServerGame;
            MainMenuAction.OpenSettingsGame += OpenSettingsGame;
            MainMenuAction.OpenExitGame += OpenExitGame;
            MainMenuAction.ExitGame += Exit;
        }
        private void OpenCampaign()
        {
            CampaignUIAction.OpenCampaignUI?.Invoke();
            CloseMainMenu();
        }
        
        private void OpenExitGame()
        {
            ref var popupViewConfig = ref _dataWorld.OneData<PopupData>().PopupViewConfig;
            var popupView = new PopupConfimStruct();
            popupView.HeaderLoc = popupViewConfig.HeaderPopupConfim;
            popupView.DescrLoc = popupViewConfig.DescrPopupConfim;
            popupView.ButtonConfimLoc = popupViewConfig.ConfimButtonPopupConfim;
            popupView.ButtonCancelLoc = popupViewConfig.CancelButtonPopupConfim;
            popupView.ButtonConfimAction = MainMenuAction.ExitGame;
            PopupAction.ConfirmPopup?.Invoke(popupView);
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

        private SelectLeadersData CreatePlayerDataLocalGame()
        {
            var selectPlayerData = new SelectPlayerData();
            selectPlayerData.SelectLeaders = new();
            
            var playerLeaderData = new SelectLeadersData {
                IDPlayer = 0,
                PlayerType = PlayerType.Player,
                SelectLeader = ""
            };
            selectPlayerData.SelectLeaders.Add(playerLeaderData);

            var enemyLeaders = GetRandomLeader(3);
            
            for (int i = 1; i < 4; i++)
            {
                selectPlayerData.SelectLeaders.Add(new SelectLeadersData {
                    IDPlayer = i,
                    PlayerType = PlayerType.AIEasy,
                    SelectLeader = enemyLeaders[i-1]
                });
            }

            _dataWorld.CreateOneData(selectPlayerData);
            return playerLeaderData;
        }

        private List<string> GetRandomLeader(int count)
        {
            var leadersConfig = _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            var nameLeaders = new List<string>();

            while (nameLeaders.Count != count)
            {
                var random = new System.Random();
                var selectLeader = leadersConfig.ElementAt(random.Next(leadersConfig.Count));

                var isUseLeader = false;
                foreach (var useLeader in nameLeaders)
                {
                    if (useLeader == selectLeader.Key)
                    {
                        isUseLeader = true;
                    }
                }
                
                if (!isUseLeader)
                    nameLeaders.Add(selectLeader.Key);
            }
            
            return nameLeaders;
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