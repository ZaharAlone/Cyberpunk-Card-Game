using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
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
            
            //Считаем убитых
            var numberOfDeathsAttackingPlayer = CalculateCountKillUnit(defendingPlayerStats, attackingPlayerStats);
            var numberOfDeathsDefendingPlayer = CalculateCountKillUnit(attackingPlayerStats, defendingPlayerStats);

            Debug.Log($"Атакующий убивает у защитника {numberOfDeathsDefendingPlayer.CountUnitDeaths}");
            Debug.Log($"Защищающийся убивает у атакующего {numberOfDeathsAttackingPlayer.CountUnitDeaths}");
            
            if (numberOfDeathsAttackingPlayer.CountUnitDeaths > 0)
                attackingPlayerEntity.AddComponent(numberOfDeathsAttackingPlayer);
            if (numberOfDeathsDefendingPlayer.CountUnitDeaths > 0)
                defendingPlayerEntity.AddComponent(numberOfDeathsDefendingPlayer);

            //Считаем выживших
            var attackingCountLifeUnits = CountLifeUnitPlayers(attackingPlayerEntity, numberOfDeathsAttackingPlayer.CountUnitDeaths);
            var defendingCountLifeUnits = CountLifeUnitPlayers(defendingPlayerEntity, numberOfDeathsDefendingPlayer.CountUnitDeaths);

            //Определяем победителя если с одной стороны все погибли
            if (attackingCountLifeUnits > 0 && defendingCountLifeUnits <= 0)
            {
                SetWinLosePlayer(attackingPlayerEntity, defendingPlayerEntity);
                Debug.Log("Победил атакующий, т.к. убил всех защищающихся");
                return;
            }
            else if (attackingCountLifeUnits <= 0 && defendingCountLifeUnits > 0)
            {
                Debug.Log("Победил защищающийся, т.к. убил всех атакующих");
                SetWinLosePlayer(defendingPlayerEntity, attackingPlayerEntity);
                return;
            }
            
            //Определяем победителя по паверу
            if (attackingPlayerStats.PowerPoint >= defendingPlayerStats.PowerPoint)
            {
                SetWinLosePlayer(attackingPlayerEntity, defendingPlayerEntity);
                Debug.Log("Победил атакующий, по паверу");

                CheckFriendlyNeighboringDistrict(defendingPlayerEntity);
            }
            else
            {
                SetWinLosePlayer(defendingPlayerEntity, attackingPlayerEntity);
                Debug.Log("Победил защищающийся, по паверу");
                CheckFriendlyNeighboringDistrict(attackingPlayerEntity);
            }
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

        private int CountLifeUnitPlayers(Entity playerEntity, int countKillUnit)
        {
            var playerInBattleComponent = playerEntity.GetComponent<PlayerInBattleComponent>();
            var targetDistrictGUID = _dataWorld.OneData<BattleCurrentData>().DistrictBattleGUID;

            var countPlayerInDistrict = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.PowerSolidPlayerID == playerInBattleComponent.PlayerID
                    && unit.GUIDDistrict == targetDistrictGUID)
                .Count();
            
            var countLifeUnit = countPlayerInDistrict - countKillUnit;
            return countLifeUnit;
        }

        private void SetWinLosePlayer(Entity winnerEntity, Entity losingEntity)
        {
            winnerEntity.AddComponent(new PlayerWinBattleComponent());
            losingEntity.AddComponent(new PlayerLoseBattleComponent());
        }

        private void CheckFriendlyNeighboringDistrict(Entity playerEntity)
        {
            var playerInBattleComponent = playerEntity.GetComponent<PlayerInBattleComponent>();

            var losingPlayerHasSomewhereToRetreat = BattleAction.CheckBattleFriendlyUnitsPresenceNeighboringDistrict.Invoke(playerInBattleComponent.PlayerID);
            if (!losingPlayerHasSomewhereToRetreat)
            {
                Debug.Log("Игроку некуда отступать, все его юниты на территории умирают");
                playerEntity.AddComponent(new NotZoneToRetreatLozingPlayerComponent());
                if (!playerEntity.HasComponent<NumberOfDeathsUnitsInBattleComponent>())
                    playerEntity.AddComponent(new NumberOfDeathsUnitsInBattleComponent());
            }
            else
            {
                playerEntity.AddComponent(new SquadMustRetreatComponent());
            }
        }
        
        public void Destroy()
        {
            BattleAction.CalculateResultBattle -= CalculateResultBattle;
        }
    }
}