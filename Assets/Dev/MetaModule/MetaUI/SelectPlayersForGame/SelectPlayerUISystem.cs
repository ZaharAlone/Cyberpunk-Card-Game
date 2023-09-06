using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Enemy;
using CyberNet.Global;
using CyberNet.Meta.StartGame;
using CyberNet.Tools;

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
            SelectPlayerAction.OnClickStartGame += OnClickStartGame;
            SelectPlayerAction.OnClickEditLeader += OnClickEditLeader;
            SelectPlayerAction.SwitchTypePlayer += SwitchTypePlayer;
        }
        private void OnClickStartGame()
        {
            StartGameAction.StartLocalGame?.Invoke();
            CloseWindow();
        }

        private void OpenWindow()
        {
            ref var uiSelectPlayer = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectPlayersUIMono;
            UpdateViewPlayers();
            uiSelectPlayer.OpenWindow();
        }

        private void UpdateViewPlayers()
        {
            ref var selectPlayers = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            ref var uiSelectPlayer = ref _dataWorld.OneData<MetaUIData>().MetaUIMono.SelectPlayersUIMono;
            var leadersView = _dataWorld.OneData<LeadersViewData>().LeadersView;
            var leadersConfigData = _dataWorld.OneData<LeadersConfigData>();
            ref var playerTypeLoc = ref _dataWorld.OneData<BoardGameData>().BoardGameConfig.PlayerTypeLoc;

            var counter = 0;
            foreach (var playerSlot in uiSelectPlayer.SelectPlayerSlot)
            {
                if (selectPlayers.Count < counter)
                {
                    playerSlot.OnClickClearSlot();
                }
                else
                {
                    leadersConfigData.LeadersConfig.TryGetValue(selectPlayers[counter].SelectLeader, out var leadersConfig);
                    leadersConfigData.AbilityConfig.TryGetValue(leadersConfig.Ability, out var abilityConfig);

                    leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
                    leadersView.TryGetValue(abilityConfig.ImageAbility, out var imAbility);
                    
                    playerSlot.SetViewLeader(imCardLeaders, imAbility, leadersConfig.NameLoc);
                    playerSlot.SetBaseNamePlayer(selectPlayers[counter].NamePlayer);
                    
                    playerTypeLoc.TryGetValue(selectPlayers[counter].PlayerType, out var locSelectTypePlayer);
                    playerSlot.SetLocTypePlayer(locSelectTypePlayer);
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
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.prevSelectLeader);
            CloseWindow();
        }

        private void OnClickEditLeader(int idSlot)
        {
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.SelectLeaders[idSlot]);
            CloseWindow();
        }
        
        private void SwitchTypePlayer(int indexSlot, bool isRightMove)
        {
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            ref var botNames = ref _dataWorld.OneData<BotConfigData>().BotNameList;
            
            var indexPlayerType = selectLeaders[indexSlot].PlayerType.GetHashCode();
            
            if (isRightMove)
                indexPlayerType++;
            else
                indexPlayerType--;

            if (indexPlayerType == 5)
                indexPlayerType = 1;
            if (indexPlayerType == 0)
                indexPlayerType = 4;

            var selectLeaderEdit = selectLeaders[indexSlot];
            selectLeaderEdit.PlayerType = (PlayerType)Enum.ToObject(typeof(PlayerType), indexPlayerType);
            if (selectLeaderEdit.PlayerType == PlayerType.Player)
                selectLeaderEdit.NamePlayer = $"Player {(indexSlot + 1).ToString()}";
            else
                selectLeaderEdit.NamePlayer = GenerateUniqueBotName.Generate(botNames, selectLeaders);

            selectLeaders[indexSlot] = selectLeaderEdit;
            
            UpdateViewPlayers();
        }
    }
}