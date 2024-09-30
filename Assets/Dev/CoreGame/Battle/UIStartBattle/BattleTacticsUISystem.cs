using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.Arena;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.Battle.TacticsMode
{
    [EcsSystem(typeof(CoreModule))]
    public class BattleTacticsUISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const string unknown_quantity_add_value = "+?";
        private const string unknown_quantity_value = "?";
        
        public void PreInit()
        {
            BattleAction.OpenTacticsScreen += EnableTacticsUI;
        }
        private void EnableTacticsUI()
        {
            SetViewAvatarPlayers();
            SetStatsPlayer();

            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            uiTactics.ShowTacticsUI();
        }

        private void SetViewAvatarPlayers()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;

            var battleData = _dataWorld.OneData<BattleCurrentData>();
            var avatarAttackingPlayer = GetPlayerAvatar(battleData.AttackingPlayer.PlayerID);
            uiTactics.PlayerStatsContainer_Attack.SetAvatarPlayer(avatarAttackingPlayer);
            
            if (battleData.DefendingPlayer.PlayerControlEntity == PlayerOrAI.None)
            {
                var neutralAvatar = _dataWorld.OneData<LeadersViewData>().NeutralLeaderAvatar;
                uiTactics.PlayerStatsContainer_Defence.SetAvatarPlayer(neutralAvatar);
            }
            else
            {
                var avatarDefendingPlayer = GetPlayerAvatar(battleData.DefendingPlayer.PlayerID);
                uiTactics.PlayerStatsContainer_Defence.SetAvatarPlayer(avatarDefendingPlayer);
            }
        }

        private Sprite GetPlayerAvatar(int playerID)
        {
            var avatarPlayer = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirst<PlayerViewComponent>().Avatar;
            return avatarPlayer;
        }

        private void SetStatsPlayer()
        {
            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            var battleData = _dataWorld.OneData<BattleCurrentData>();

            SetStatsPlayer(battleData.AttackingPlayer, uiTactics.PlayerStatsContainer_Attack, false);
            SetStatsPlayer(battleData.DefendingPlayer, uiTactics.PlayerStatsContainer_Defence, true);
        }

        private void SetStatsPlayer(PlayerInBattleStruct playerStats, BattlePlayerStatsContainerUIMono uiContainer, bool isDefending)
        {
            var powerCount = playerStats.PowerPoint.BaseValue + playerStats.PowerPoint.AbilityValue;
            var killCount = playerStats.KillPoint.BaseValue + playerStats.KillPoint.AbilityValue;
            var defenceCount = playerStats.DefencePoint.BaseValue + playerStats.DefencePoint.AbilityValue;

            if (isDefending)
            {
                var powerString = TransformDefendingValueStats(powerCount);
                var killString = TransformDefendingValueStats(killCount);
                var defenceString = TransformDefendingValueStats(defenceCount);
                
                uiContainer.SetStats(powerString, killString, defenceString);
            }
            else
            {
                var powerString = powerCount.ToString();
                var killString = killCount.ToString();
                var defenceString = defenceCount.ToString();
                
                uiContainer.SetStats(powerString, killString, defenceString);
            }
        }

        private string TransformDefendingValueStats(int value)
        {
            var valueString = "";
            if (value == 0)
                valueString = unknown_quantity_value;
            else
                valueString = value + unknown_quantity_add_value;
            return valueString;
        }

        public void Destroy()
        {
            BattleAction.OpenTacticsScreen -= EnableTacticsUI;
        }
    }
}