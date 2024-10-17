using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.UI;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class UpdateCurrentValueCurrencyPlayerInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            //BattleTacticsUIAction.UpdateCurrencyPlayerInBattle += UpdateCurrencyPlayerInBattle;
        }
        /*
        private void UpdateCurrencyPlayerInBattle()
        {
            var playerOpenTacticsScreenEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();

            if (playerOpenTacticsScreenEntity.HasComponent<PlayerSelectCardTacticsScreenComponent>())
            {
                
            }
            
            var attackingPlayerStats = SetZeroCardStats(battleCurrentData.AttackingPlayer);
            attackingPlayerStats = CalculatePlayerTacticsCard(attackingPlayerStats);
            attackingPlayerStats = CalculateAbilityCard(attackingPlayerStats);
            
            battleCurrentData.AttackingPlayer = attackingPlayerStats;
            SetStatsViewPlayer(attackingPlayerStats);
        }

        private PlayerInBattleComponent SetZeroCardStats(PlayerInBattleComponent playerStats)
        {
            playerStats.PowerPoint.CardValue = 0;
            playerStats.KillPoint.CardValue = 0;
            playerStats.DefencePoint.CardValue = 0;
            return playerStats;
        }

        private PlayerInBattleComponent CalculatePlayerTacticsCard(PlayerInBattleComponent playerStats)
        {
            var cardComponent = _dataWorld.Select<CardSelectInTacticsScreenComponent>().SelectFirst<CardComponent>();
            var currentTacticsIndex = 1;//TODO вернуть SelectCurrentTacticsIndex();
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;

            var currentTactics = battleTactics[currentTacticsIndex];
            playerStats = WriteStatsCardValue(playerStats, currentTactics.LeftCharacteristics, cardComponent.ValueLeftPoint);
            playerStats = WriteStatsCardValue(playerStats, currentTactics.RightCharacteristics, cardComponent.ValueRightPoint);
            
            return playerStats;
        }

        private PlayerInBattleComponent WriteStatsCardValue(PlayerInBattleComponent playerStats, BattleCharacteristics typeCharacteristics, int value)
        {
            switch (typeCharacteristics)
            {
                case BattleCharacteristics.PowerPoint:
                    playerStats.PowerPoint.CardValue += value;
                    break;
                case BattleCharacteristics.KillPoint:
                    playerStats.KillPoint.CardValue += value;
                    break;
                case BattleCharacteristics.DefencePoint:
                    playerStats.DefencePoint.CardValue += value;
                    break;
            }
            return playerStats;
        }
        
        private int SelectCurrentTacticsIndex()
        {
            var currentTacticsKey = _dataWorld.OneData<BattleCurrentData>().CurrentTacticsKey;
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;
            
            var listTactics = new List<string>();
            foreach (var currentTactics in battleTactics)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(currentTacticsKey);
            return targetIndex;
        }

        private PlayerInBattleComponent CalculateAbilityCard(PlayerInBattleComponent playerStats)
        {
            var cardComponent = _dataWorld.Select<CardSelectInTacticsScreenComponent>().SelectFirst<CardComponent>();

            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
                return playerStats;
            
            switch (cardComponent.Ability_1.AbilityType)
            {
                case AbilityType.PowerPoint:
                    playerStats.PowerPoint.CardValue += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.KillPoint:
                    playerStats.KillPoint.CardValue += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.DefencePoint:
                    playerStats.DefencePoint.CardValue += cardComponent.Ability_1.Count;
                    break;
            }

            return playerStats;
        }
        
        private void SetStatsViewPlayer(PlayerInBattleComponent playerStats)
        {
            var attackingPlayerUIStats = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono.PlayerStatsContainer_Attack;

            var powerCount = playerStats.PowerPoint.BaseValue + playerStats.PowerPoint.AbilityValue;
            var killCount = playerStats.KillPoint.BaseValue + playerStats.KillPoint.AbilityValue;
            var defenceCount = playerStats.DefencePoint.BaseValue + playerStats.DefencePoint.AbilityValue;

            var powerString = powerCount.ToString();
            var killString = killCount.ToString();
            var defenceString = defenceCount.ToString();
                
            attackingPlayerUIStats.SetStats(powerString, killString, defenceString);
        }
*/
        public void Destroy()
        {
  //          BattleTacticsUIAction.UpdateCurrencyPlayerInBattle -= UpdateCurrencyPlayerInBattle;
        }
    }
}