using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Battle.TacticsMode.InteractiveCard;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;

namespace CyberNet.Core.Battle.CutsceneArena
{
    [EcsSystem(typeof(CoreModule))]
    public class CalculateResultBattleSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            BattleAction.CalculateResultBattle += CalculateResultBattle;
        }

        private void CalculateResultBattle()
        {
            var attackingPlayerEntity = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(inBattle => inBattle.IsAttacking)
                .SelectFirstEntity();
            
            var defendingPlayerEntity = _dataWorld.Select<PlayerInBattleComponent>()
                .Where<PlayerInBattleComponent>(inBattle => !inBattle.IsAttacking)
                .SelectFirstEntity();

            var attackingPlayerStats = GetPowerPlayer(attackingPlayerEntity);
            var defendingPlayerStats = GetPowerPlayer(defendingPlayerEntity);

            //Определяем победителя
            if (attackingPlayerStats.PowerPoint >= defendingPlayerStats.PowerPoint)
            {
                attackingPlayerEntity.AddComponent(new PlayerWinBattleComponent());
                defendingPlayerEntity.AddComponent(new PlayerLoseBattleComponent());
            }
            else
            {
                attackingPlayerEntity.AddComponent(new PlayerLoseBattleComponent());
                defendingPlayerEntity.AddComponent(new PlayerWinBattleComponent());
            }
            
            //Считаем убитых
            var numberOfDeathsAttackingPlayer = CalculateCountKillUnit(defendingPlayerStats, attackingPlayerStats);
            var numberOfDeathsDefendingPlayer = CalculateCountKillUnit(attackingPlayerStats, defendingPlayerStats);

            attackingPlayerEntity.AddComponent(numberOfDeathsAttackingPlayer);
            defendingPlayerEntity.AddComponent(numberOfDeathsDefendingPlayer);
        }

        private PowerKillDefenceDTO GetPowerPlayer(Entity playerEntity)
        {
            var playerInBattleComponent = playerEntity.GetComponent<PlayerInBattleComponent>();
            
            var valuePowerInBattle = CalculatePlayerStatsInBattle.CalculateValueInMap(playerInBattleComponent);
            valuePowerInBattle = BattleAction.CalculatePlayerStatsInBattle(valuePowerInBattle, playerEntity);

            return valuePowerInBattle;
        }

        private NumberOfDeathsUnitsInBattleComponent CalculateCountKillUnit(PowerKillDefenceDTO attackingSideStats, PowerKillDefenceDTO defendingSideStats)
        {
            var countKillUnit = attackingSideStats.KillPoint - defendingSideStats.DefencePoint;

            var numberOfDeathsUnitsInBattleComponent = new NumberOfDeathsUnitsInBattleComponent {
                CountUnitDeaths = countKillUnit
            };
            return numberOfDeathsUnitsInBattleComponent;
        }
        
        public void Destroy()
        {
            BattleAction.CalculateResultBattle -= CalculateResultBattle;
        }
    }
}