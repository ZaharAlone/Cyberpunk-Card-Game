using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using CyberNet.Core.Player;
using CyberNet.Core.UI;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class UpdateCurrentValueCurrencyPlayerInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.UpdateCurrencyPlayerInBattle += UpdateCurrencyPlayerInBattle;
        }

        private void UpdateCurrencyPlayerInBattle()
        {
            var playerOpenTacticsScreenEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();
            var playerInBattleComponent = playerOpenTacticsScreenEntity.GetComponent<PlayerInBattleComponent>();

            var valuePowerInBattle = CalculateValueInMap(playerInBattleComponent);

            var selectCardInTacticsScreen = _dataWorld.Select<CardSelectInTacticsScreenComponent>();

            if (selectCardInTacticsScreen.Count() != 0)
            {
                var selectCardInTacticsScreenEntity = selectCardInTacticsScreen.SelectFirstEntity();
                var cardComponent = selectCardInTacticsScreenEntity.GetComponent<CardComponent>();
                var currentTacticsIndex = SelectCurrentTacticsIndex();

                var targetTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics[currentTacticsIndex];
                valuePowerInBattle = CalculateValueCardAbility(valuePowerInBattle, targetTactics.LeftCharacteristics, cardComponent.ValueLeftPoint);
                valuePowerInBattle = CalculateValueCardAbility(valuePowerInBattle, targetTactics.RightCharacteristics, cardComponent.ValueRightPoint);
                valuePowerInBattle = CalculateAbilityCard(valuePowerInBattle);
            }
            
            SetStatsViewPlayer(valuePowerInBattle);
        }

        private PowerKillDefenceDTO CalculateValueInMap(PlayerInBattleComponent playerStats)
        {
            var mapValue = new PowerKillDefenceDTO();
            
            mapValue.PowerPoint = playerStats.PowerPoint;
            mapValue.KillPoint = playerStats.KillPoint;
            mapValue.DefencePoint = playerStats.DefencePoint;

            return mapValue;
        }
        
        private PowerKillDefenceDTO CalculateValueCardAbility(PowerKillDefenceDTO powerKillDefenceDTO, BattleCharacteristics battleChar, int cardPower)
        {
            switch (battleChar)
            {
                case BattleCharacteristics.PowerPoint:
                    powerKillDefenceDTO.PowerPoint += cardPower;
                    break;
                case BattleCharacteristics.KillPoint:
                    powerKillDefenceDTO.KillPoint += cardPower;
                    break;
                case BattleCharacteristics.DefencePoint:
                    powerKillDefenceDTO.DefencePoint += cardPower;
                    break;
            }
            return powerKillDefenceDTO;
        }
        
        private int SelectCurrentTacticsIndex()
        {
            var playerOpenTacticsScreenComponent = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirst<OpenBattleTacticsUIComponent>();
            var battleTactics = _dataWorld.OneData<BattleTacticsData>().BattleTactics;

            var currentTacticsKey = playerOpenTacticsScreenComponent.CurrentSelectTacticsUI;
            
            var listTactics = new List<string>();
            foreach (var currentTactics in battleTactics)
                listTactics.Add(currentTactics.Key);
            
            var targetIndex = listTactics.IndexOf(currentTacticsKey);
            return targetIndex;
        }
        
        private PowerKillDefenceDTO CalculateAbilityCard(PowerKillDefenceDTO playerValueInBattle)
        {
            var cardComponent = _dataWorld.Select<CardSelectInTacticsScreenComponent>().SelectFirst<CardComponent>();

            if (cardComponent.Ability_1.AbilityType == AbilityType.None)
                return playerValueInBattle;
            
            switch (cardComponent.Ability_1.AbilityType)
            {
                case AbilityType.PowerPoint:
                    playerValueInBattle.PowerPoint += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.KillPoint:
                    playerValueInBattle.KillPoint += cardComponent.Ability_1.Count;
                    break;
                case AbilityType.DefencePoint:
                    playerValueInBattle.DefencePoint += cardComponent.Ability_1.Count;
                    break;
            }

            return playerValueInBattle;
        }
        
        private void SetStatsViewPlayer(PowerKillDefenceDTO powerKillDefenceDTO)
        {
            var playerInBattleComponent = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirst<PlayerInBattleComponent>();
            var battleTacticsMono = _dataWorld.OneData<CoreGameUIData>().BoardGameUIMono.BattleTacticsModeUIMono;

            var powerString = powerKillDefenceDTO.PowerPoint.ToString();
            var killString = powerKillDefenceDTO.KillPoint.ToString();
            var defenceString = powerKillDefenceDTO.DefencePoint.ToString();

            if (playerInBattleComponent.IsAttacking)
                battleTacticsMono.PlayerStatsContainer_Attack.SetStats(powerString, killString, defenceString);
            else
                battleTacticsMono.PlayerStatsContainer_Defence.SetStats(powerString, killString, defenceString);
        }

        public void Destroy()
        {
           BattleTacticsUIAction.UpdateCurrencyPlayerInBattle -= UpdateCurrencyPlayerInBattle;
        }
    }
}