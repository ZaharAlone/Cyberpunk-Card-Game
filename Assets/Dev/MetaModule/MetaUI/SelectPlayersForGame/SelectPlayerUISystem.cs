using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Global;

namespace CyberNet.Meta.SelectPlayersForGame
{
    [EcsSystem(typeof(MetaModule))]
    public class SelectPlayerUISystem : IPreInitSystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            SelectPlayerAction.OpenSelectPlayerUI += OpenWindow;
            SelectPlayerAction.CloseSelectPlayerUI += CloseWindow;
            SelectPlayerAction.OnClickBack += OnClickBack;
            SelectPlayerAction.OnClickEditLeader += OnClickEditLeader;
        }

        private void OpenWindow()
        {
            ref var uiSelectPlayer = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectPlayersUIMono;
            SetViewSelectPlayer();
            uiSelectPlayer.OpenWindow();
        }
        
        private void SetViewSelectPlayer()
        {
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            ref var uiSelectPlayer = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectPlayersUIMono;
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            
            var counter = 0;
            foreach (var playerSlot in uiSelectPlayer.SelectPlayerSlot)
            {
                if (selectLeaders.Count < counter)
                {
                    playerSlot.OnClickClearSlot();
                }
                else
                {
                    leadersConfigData.LeadersConfig.TryGetValue(selectLeaders[counter].SelectLeader, out var leadersConfig);
                    leadersConfigData.AbilityConfig.TryGetValue(leadersConfig.Ability, out var abilityConfig);

                    leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
                    leadersView.TryGetValue(abilityConfig.ImageAbility, out var imAbility);
                    
                    playerSlot.SetViewLeader(imCardLeaders, imAbility, leadersConfig.NameLoc);
                }
                
                counter++;
            }
        }

        private void CloseWindow()
        {
            ref var uiSelectPlayer = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectPlayersUIMono;
            uiSelectPlayer.CloseWindow();
        }
        
        private void OnClickBack()
        {
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.PrevSelectLeader);
            CloseWindow();
        }

        private void OnClickEditLeader(int idSlot)
        {
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.SelectLeaders[idSlot]);
            CloseWindow();
        }
    }
}