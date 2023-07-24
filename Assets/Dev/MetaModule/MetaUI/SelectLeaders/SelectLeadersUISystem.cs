using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class SelectLeadersUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectLeaderAction.OpenSelectLeaderUI += OpenFirstSelectLeaderUI;
            SelectLeaderAction.SelectLeader += SelectLeaderView;
            SelectLeaderAction.BackMainMenu += BackMainMenu;
            SelectLeaderAction.StartGame += StartGame;
            SelectLeaderAction.InitButtonLeader += InitButtonLeader;
        }
        private void OpenFirstSelectLeaderUI(GameModeEnum gameModeEnum)
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.OpenWindow(gameModeEnum);

            _dataWorld.CreateOneData(new SelectLeadersData {SelectGameMode = gameModeEnum });
        }
        
        private void OpenSecondSelectLeaderUI()
        {
            ref var selectLeaderData = ref _dataWorld.OneData<SelectLeadersData>();
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.OpenWindow(selectLeaderData.SelectGameMode);
        }
        
        private void StartGame()
        {
            ref var selectLeadersData = ref _dataWorld.OneData<SelectLeadersData>();
            var isNextSelectPlayer2 = false;
            
            switch (selectLeadersData.SelectGameMode)
            {
                case GameModeEnum.Campaign:
                    break;
                case GameModeEnum.LocalVSAI:
                    StartGameAction.StartGameLocalVSAI?.Invoke(selectLeadersData.CurrentSelectLeader_Player1);
                    break;
                case GameModeEnum.LocalVSPlayer:
                    isNextSelectPlayer2 = true;
                    selectLeadersData.SelectGameMode = GameModeEnum.LocalVSPlayer2;
                    OpenSecondSelectLeaderUI();
                    break;
                case GameModeEnum.LocalVSPlayer2:
                    StartGameAction.StartGameLocalVSPlayer?.Invoke(selectLeadersData.CurrentSelectLeader_Player1, selectLeadersData.CurrentSelectLeader_Player2);
                    break;
                case GameModeEnum.OnlineGame:
                    break;
            }

            if (!isNextSelectPlayer2)
            {
                _dataWorld.RemoveOneData<SelectLeadersData>();
                CloseSelectLeader();   
            }
        }

        private void SelectLeaderView(string nameLeader)
        {
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            leadersConfigData.LeadersConfig.TryGetValue(nameLeader, out var leadersConfig);
            leadersConfigData.AbilityConfig.TryGetValue(leadersConfig.Ability, out var abilityConfig);

            leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
            leadersView.TryGetValue(abilityConfig.ImageAbility, out var imAbility);
            
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.SetSelectViewLeader(imCardLeaders, leadersConfig.NameLoc, leadersConfig.DescrLoc);
            
            uiSelectLeader.SetSelectViewLeaderAbility(imAbility, abilityConfig.NameLoc, abilityConfig.DescrLoc);
            WriteInComponentSelectLeader(nameLeader);
        }

        private Sprite InitButtonLeader(string nameLeader, bool isFirstButton)
        {
            _dataWorld.OneData<LeadersConfigData>().LeadersConfig.TryGetValue(nameLeader, out var leaderConfig);
            _dataWorld.OneData<LeadersViewData>().LeadersView.TryGetValue(leaderConfig.ImageButtonLeader, out var imageButton);

            if (isFirstButton)
            {
                //TO-DO add select first button
                SelectLeaderView(nameLeader);
                WriteInComponentSelectLeader(nameLeader);
            }

            return imageButton;
        }

        private void WriteInComponentSelectLeader(string nameLeader)
        {
            ref var selectLeaderData = ref _dataWorld.OneData<SelectLeadersData>();
            if (selectLeaderData.SelectGameMode != GameModeEnum.LocalVSPlayer2)
            {
                selectLeaderData.CurrentSelectLeader_Player1 = nameLeader;
            }
            else
            {
                selectLeaderData.CurrentSelectLeader_Player2 = nameLeader;
            }
        }

        private void BackMainMenu()
        {
            MainMenuAction.OpenMainMenu?.Invoke();
            _dataWorld.RemoveOneData<SelectLeadersData>();
            CloseSelectLeader();
        }

        private void CloseSelectLeader()
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            uiSelectLeader.CloseWindow();
        }
    }
}