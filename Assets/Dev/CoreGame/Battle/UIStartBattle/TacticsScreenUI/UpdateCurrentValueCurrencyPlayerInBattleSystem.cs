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
using GameAnalyticsSDK.Events;

namespace CyberNet.Core.Battle.TacticsMode.InteractiveCard
{
    [EcsSystem(typeof(CoreModule))]
    public class UpdateCurrentValueCurrencyPlayerInBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            BattleTacticsUIAction.UpdateCurrencyPlayerInBattle += UpdateCurrencyPlayerInBattleView;
        }
        
        private void UpdateCurrencyPlayerInBattleView()
        {
            var playerOpenTacticsScreenEntity = _dataWorld.Select<OpenBattleTacticsUIComponent>().SelectFirstEntity();
            var playerInBattleComponent = playerOpenTacticsScreenEntity.GetComponent<PlayerInBattleComponent>();
            
            var valuePowerInBattle = CalculatePlayerStatsInBattle.CalculateValueInMap(playerInBattleComponent);
            valuePowerInBattle = BattleAction.CalculatePlayerStatsInBattle(valuePowerInBattle, playerOpenTacticsScreenEntity);
            
            SetStatsViewPlayer(valuePowerInBattle);
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
           BattleTacticsUIAction.UpdateCurrencyPlayerInBattle -= UpdateCurrencyPlayerInBattleView;
        }
    }
}