using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global;
using CyberNet.Meta.SelectPlayersForGame;
using CyberNet.Meta.StartGame;

namespace CyberNet.Meta
{
    [EcsSystem(typeof(MetaModule))]
    public class SelectLeadersUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectLeaderAction.OpenSelectLeaderUI += OpenSelectLeaderUI;
            SelectLeaderAction.SelectLeader += SelectLeaderView;
            SelectLeaderAction.BackMainMenu += BackMainMenu;
            SelectLeaderAction.ConfirmSelect += ConfirmSelectLeader;
            SelectLeaderAction.InitButtonLeader += InitButtonLeader;
        }
        private void OpenSelectLeaderUI(SelectLeadersData selectLeaderConfig)
        {
            ref var uiSelectLeader = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectLeadersUIMono;
            _dataWorld.CreateOneData(selectLeaderConfig);
            uiSelectLeader.OpenWindow();
        }
        
        private void ConfirmSelectLeader()
        {
            ref var selectLeadersData = ref _dataWorld.OneData<SelectLeadersData>();
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();

            var counter = 0;
            foreach (var selectLeader in selectPlayersData.SelectLeaders)
            {
                if (selectLeader.IDPlayer == selectLeadersData.IDPlayer)
                {
                    selectPlayersData.SelectLeaders[counter] = selectLeadersData;
                    break;
                }
                counter++;
            }
            
            _dataWorld.RemoveOneData<SelectLeadersData>();
            CloseSelectLeader(); 
            SelectPlayerAction.OpenSelectPlayerUI?.Invoke();
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
                //TODO add select first button view
                SelectLeaderView(nameLeader);
                WriteInComponentSelectLeader(nameLeader);
            }

            return imageButton;
        }

        private void WriteInComponentSelectLeader(string nameLeader)
        {
            _dataWorld.OneData<SelectLeadersData>().SelectLeader = nameLeader;
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