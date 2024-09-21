using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Threading.Tasks;
using CyberNet.Core.AI;
using CyberNet.Global;
using CyberNet.Meta.SelectPlayersForGame;
using CyberNet.Meta.StartGame;
using CyberNet.Tools;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class SelectLeadersUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;
        private bool _isFirstSelectLeader;
        
        public void PreInit()
        {
            SelectLeaderAction.OpenSelectLeaderUI += OpenSelectLeaderUI;
            SelectLeaderAction.SelectLeader += SelectLeaderView;
            SelectLeaderAction.BackMainMenu += BackMainMenu;
            SelectLeaderAction.ConfirmSelect += ConfirmSelectLeader;
            SelectLeaderAction.InitButtonLeader += InitButtonLeader;
        }
        
        private void OpenSelectLeaderUI(SelectLeaderData selectLeaderConfig, bool isStartGame)
        {
            _isFirstSelectLeader = isStartGame;
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            _dataWorld.CreateOneData(selectLeaderConfig);
            
            SelectLeaderView(selectLeaderConfig.SelectLeader);
            
            uiSelectLeader.SetLocSelectPlayer(selectLeaderConfig.NamePlayer);
            uiSelectLeader.OpenWindow();
        }
        
        private void ConfirmSelectLeader()
        {
            ref var selectLeadersData = ref _dataWorld.OneData<SelectLeaderData>();
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();

            var counter = 0;
            foreach (var selectLeader in selectPlayersData.SelectLeaders)
            {
                if (selectLeader.PlayerID == selectLeadersData.PlayerID)
                {
                    selectPlayersData.SelectLeaders[counter] = selectLeadersData;
                    break;
                }
                counter++;
            }

            selectPlayersData.PrevSelectLeader = selectLeadersData;
            
            if (_isFirstSelectLeader)
            {
                SelectStartEnemyLeader();
            }
            
            _dataWorld.RemoveOneData<SelectLeaderData>();
            CloseSelectLeader(); 
            SelectPlayerAction.OpenSelectPlayerUI?.Invoke();
        }

        private void SelectStartEnemyLeader()
        {
            var leadersConfig = _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            ref var botNames = ref _dataWorld.OneData<BotConfigData>().BotNameList;
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();
            var enemyLeaders = GeneratePlayerData.GetRandomLeader(leadersConfig, 3, selectPlayersData.SelectLeaders[0].SelectLeader);
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            
            for (int i = 1; i < 4; i++)
            {
                var botName = GeneratePlayerData.GenerateUniquePlayerName(botNames, selectPlayersData.SelectLeaders);
                
                selectPlayersData.SelectLeaders.Add(new SelectLeaderData {
                    PlayerID = i,
                    PlayerOrAI = PlayerOrAI.AIEasy,
                    SelectLeader = enemyLeaders[i-1],
                    NamePlayer = botName,
                    KeyVisualCity = cityVisualSO.PlayerVisualKeyList[i]
                });
            }
        }

        private void SelectLeaderView(string nameLeader)
        {
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            var uiSelectLeader = _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            
            leadersConfigData.LeadersConfig.TryGetValue(nameLeader, out var leadersConfig);
            leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
            
            uiSelectLeader.SetSelectViewLeader(imCardLeaders, leadersConfig.NameLoc, leadersConfig.DescrLoc);
            uiSelectLeader.SelectButton(nameLeader);
            
            WriteInComponentSelectLeader(nameLeader);
        }

        private Sprite InitButtonLeader(string nameLeader, bool isFirstButton)
        {
            _dataWorld.OneData<LeadersConfigData>().LeadersConfig.TryGetValue(nameLeader, out var leaderConfig);
            _dataWorld.OneData<LeadersViewData>().LeadersView.TryGetValue(leaderConfig.ImageButtonLeader, out var imageButton);

            if (isFirstButton)
            {
                //TODO add select first button view
                SelectLeaderView(nameLeader);
            }

            return imageButton;
        }

        private void WriteInComponentSelectLeader(string nameLeader)
        {
            _dataWorld.OneData<SelectLeaderData>().SelectLeader = nameLeader;
        }

        private void BackMainMenu()
        {
            MainMenuAction.OpenMainMenu?.Invoke();
            _dataWorld.RemoveOneData<SelectLeaderData>();
            _dataWorld.RemoveOneData<SelectPlayerData>();
            CloseSelectLeader();
        }

        private void CloseSelectLeader()
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.CloseWindow();
        }
    }
}