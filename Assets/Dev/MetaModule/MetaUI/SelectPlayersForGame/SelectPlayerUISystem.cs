using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI;
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
            SelectPlayerAction.SetPlayerName += SetPlayerName;
            SelectPlayerAction.ClearSlot += ClearSlotPlayer;
            SelectPlayerAction.CreatePlayer += CreatePlayerSlot;
        }
        private void CreatePlayerSlot(int indexSlot)
        {
            var leadersConfig = _dataWorld.OneData<LeadersConfigData>().LeadersConfig;
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            ref var botNameConfig = ref _dataWorld.OneData<BotConfigData>().BotNameList;
            var cityVisualSO = _dataWorld.OneData<BoardGameData>().CitySO;
            
            var enemyLeaders = GeneratePlayerData.GetRandomLeader(leadersConfig, 1, selectLeaders[0].SelectLeader);
            var botName = GeneratePlayerData.GenerateUniquePlayerName(botNameConfig, selectLeaders);

            var newPlayerID = 0;
            foreach (var leader in selectLeaders)
            {
                if (leader.PlayerID > newPlayerID)
                    newPlayerID = leader.PlayerID;
            }
            newPlayerID++; // делаем уникальный ID
            
            selectLeaders.Add(new SelectLeaderData {
                PlayerID = newPlayerID,
                PlayerOrAI = PlayerOrAI.AIEasy,
                SelectLeader = enemyLeaders[0],
                NamePlayer = botName,
                KeyVisualCity = cityVisualSO.PlayerVisualKeyList[indexSlot]
            });
            
            UpdateViewPlayers();
            
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
                if (selectPlayers.Count <= counter)
                {
                    playerSlot.ClearSlot();
                }
                else
                {
                    playerSlot.OpenSlot();
                    leadersConfigData.LeadersConfig.TryGetValue(selectPlayers[counter].SelectLeader, out var leadersConfig);

                    leadersView.TryGetValue(leadersConfig.ImageCardLeaders, out var imCardLeaders);
                    
                    playerSlot.SetViewLeader(imCardLeaders);
                    
                    playerSlot.SetCustomNamePlayer(selectPlayers[counter].NamePlayer);
                    
                    if (counter != 0 && selectPlayers[counter].PlayerOrAI == PlayerOrAI.Player)
                    {
                        playerSlot.SetCustomNamePlayer(selectPlayers[counter].NamePlayer);
                    }
                    else
                    {
                        playerSlot.SetBaseNamePlayer(selectPlayers[counter].NamePlayer);
                    }
                    
                    playerTypeLoc.TryGetValue(selectPlayers[counter].PlayerOrAI, out var locSelectTypePlayer);
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
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.PrevSelectLeader, false);
            CloseWindow();
        }

        private void OnClickEditLeader(int idSlot)
        {
            ref var selectPlayersData = ref _dataWorld.OneData<SelectPlayerData>();
            SelectLeaderAction.OpenSelectLeaderUI?.Invoke(selectPlayersData.SelectLeaders[idSlot], false);
            CloseWindow();
        }

        private void SwitchTypePlayer(int indexSlot, bool isRightMove)
        {
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            ref var botNames = ref _dataWorld.OneData<BotConfigData>().BotNameList;
            
            var indexPlayerType = selectLeaders[indexSlot].PlayerOrAI.GetHashCode();
            
            if (isRightMove)
                indexPlayerType++;
            else
                indexPlayerType--;

            if (indexPlayerType == 3)
                indexPlayerType = 1;
            if (indexPlayerType == 0)
                indexPlayerType = 2;
            
            //TODO: вернуть после демки
            /*+
            if (indexPlayerType == 5)
                indexPlayerType = 1;
            if (indexPlayerType == 0)
                indexPlayerType = 4;
*/
            var selectLeaderEdit = selectLeaders[indexSlot];
            selectLeaderEdit.PlayerOrAI = (PlayerOrAI)Enum.ToObject(typeof(PlayerOrAI), indexPlayerType);
            if (selectLeaderEdit.PlayerOrAI == PlayerOrAI.Player)
            {
                selectLeaderEdit.NamePlayer = $"Player {(indexSlot + 1).ToString()}";
            }
            else
                selectLeaderEdit.NamePlayer = GeneratePlayerData.GenerateUniquePlayerName(botNames, selectLeaders);

            selectLeaders[indexSlot] = selectLeaderEdit;
            
            UpdateViewPlayers();
        }
        
        private void SetPlayerName(int indexSlot, string namePlayer)
        {
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            var selectLeaderEdit = selectLeaders[indexSlot];
            selectLeaderEdit.NamePlayer = namePlayer;
            selectLeaders[indexSlot] = selectLeaderEdit;
        }

        private void ClearSlotPlayer(int indexSlot)
        {
            ref var selectLeaders = ref _dataWorld.OneData<SelectPlayerData>().SelectLeaders;
            selectLeaders.RemoveAt(indexSlot);
            UpdateViewPlayers();
        }
    }
}