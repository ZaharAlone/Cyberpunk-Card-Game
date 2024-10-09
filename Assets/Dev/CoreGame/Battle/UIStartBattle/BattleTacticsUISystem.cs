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
            BattleAction.OpenTacticsScreen += OpenTacticsUI;
        }
        private void OpenTacticsUI()
        {
            ShowHideMapUI(false);
            SetViewAvatarPlayers();
            SetStatsPlayer();

            var uiTactics = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;
            uiTactics.ShowTacticsUI();

            _dataWorld.NewEntity().AddComponent(new OpenBattleTacticsUIComponent());
        }

        private void CloseTacticsUI()
        {
            _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity().Destroy();
            ShowHideMapUI(true);
        }
        
        private void ShowHideMapUI(bool isShow)
        {
            var boardGameUI = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono;

            if (isShow)
            {
                boardGameUI.CoreHudUIMono.ShowCurrentPlayer();
                boardGameUI.CoreHudUIMono.ShowEnemyPassport();
                boardGameUI.CoreHudUIMono.ShowButtons();
                boardGameUI.TraderowMono.EnableTradeRow();
            }
            else
            {
                boardGameUI.CoreHudUIMono.HideCurrentPlayer();
                boardGameUI.CoreHudUIMono.HideEnemyPassport();
                boardGameUI.CoreHudUIMono.HideButtons();
                boardGameUI.TraderowMono.DisableTradeRow();
            }
            
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;

            var cardInTableCurrentPlayer = _dataWorld.Select<CardComponent>()
                .Without<CardHandComponent>()
                .Where<CardComponent>(card => card.PlayerID == currentPlayerID)
                .GetEntities();

            foreach (var entityDiscardCard in cardInTableCurrentPlayer)
            {
                var cardMono = entityDiscardCard.GetComponent<CardComponent>().CardMono;
                if (isShow)
                    cardMono.ShowCard();
                else
                    cardMono.HideCard();
            }
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
                .SelectFirst<PlayerViewComponent>().AvatarForBattle;
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
            BattleAction.OpenTacticsScreen -= OpenTacticsUI;
        }
    }
}