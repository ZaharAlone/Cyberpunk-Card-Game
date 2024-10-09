using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using System.Collections.Generic;
using CyberNet.Core.AI.Battle;
using CyberNet.Core.Arena;
using CyberNet.Core.Battle;
using CyberNet.Core.Battle.TacticsMode;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [EcsSystem(typeof(CoreModule))]
    public class BattleInitSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private string _targetDistrictGUID;

        private const string power_point_key = "PowerPoint";
        private const string kill_point_key = "KillPoint";
        private const string defence_point_key = "DefencePoint";

        private const int neutral_unit_id = -1;
        
        public void PreInit()
        {
            BattleAction.EndMovePlayerToNewDistrict += CheckCountEnemyInTargetDistrict;
            BattleAction.FinishBattle += FinishBattle;
        }

        private void CheckCountEnemyInTargetDistrict(string targetDistrictGUID)
        {
            var listUniquePlayers = CalculateCountUniquePlayersInTargetDistrict(targetDistrictGUID);
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            listUniquePlayers.Remove(currentPlayerID);

            _targetDistrictGUID = targetDistrictGUID;
            
            if (listUniquePlayers.Count > 1)
            {
                SelectEnemyToBattle(listUniquePlayers);
            }
            else
            {
                StartBattle(listUniquePlayers[0]);
            }
        }

        private List<int> CalculateCountUniquePlayersInTargetDistrict(string targetDistrictGUID)
        {
            var listUniquePlayers = new List<int>();
            
            var unitsInDistrictEntities = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == targetDistrictGUID)
                .GetEntities();

            foreach (var unitEntity in unitsInDistrictEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                
                if (!listUniquePlayers.Contains(unitComponent.PowerSolidPlayerID))
                {
                    listUniquePlayers.Add(unitComponent.PowerSolidPlayerID);
                }
            }
            
            return listUniquePlayers;
        }

        private void SelectEnemyToBattle(List<int> liseEnemyID)
        {
            Debug.LogError("Start select target enemy to battle, enemy > 2");
        }
        
        private void StartBattle(int enemyID)
        {
            CreateBattleData(enemyID);
            
            var currentPlayerType = _dataWorld.OneData<RoundData>().playerOrAI;

            if (currentPlayerType != PlayerOrAI.Player)
            {
                //TODO начинаем битву между ботами
                AIBattleAction.CheckEnemyBattle?.Invoke();
            }
            else
            {
                OpenTacticsScreen();
            }
        }
        
        private void CreateBattleData(int enemyID)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var battleData = new BattleCurrentData();

            battleData.DistrictBattleGUID = _targetDistrictGUID;
            
            battleData.AttackingPlayer = CreatePlayerInBattle(currentPlayerID);

            if (enemyID == neutral_unit_id)
                battleData.DefendingPlayer = CreateNeutralPlayerInBattle();
            else
                battleData.DefendingPlayer = CreatePlayerInBattle(enemyID);

            _dataWorld.CreateOneData(battleData);
        }

        private PlayerInBattleStruct CreatePlayerInBattle(int playerID)
        {
            var playerComponent = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity()
                .GetComponent<PlayerComponent>();
            
            var countUnit = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == _targetDistrictGUID 
                    && unit.PowerSolidPlayerID == playerComponent.PlayerID)
                .Count();

            var playerControlDistrictEntities = _dataWorld.Select<DistrictComponent>()
                .Where<DistrictComponent>(district => district.DistrictBelongPlayerID == playerID
                && district.PlayerControlEntity == PlayerControlEntity.PlayerControl)
                .GetEntities();

            var abilityPowerPoint = 0;
            var abilityKillPoint = 0;
            var abilityDefencePoint = 0;

            foreach (var districtEntity in playerControlDistrictEntities)
            {
                var bonusDistrict = districtEntity.GetComponent<DistrictComponent>().BonusDistrict;

                if (bonusDistrict.Item == power_point_key)
                    abilityPowerPoint += bonusDistrict.Value;
                if (bonusDistrict.Item == kill_point_key)
                    abilityKillPoint += bonusDistrict.Value;
                if (bonusDistrict.Item == defence_point_key)
                    abilityDefencePoint += bonusDistrict.Value;
            }
            
            var newPlayerInBattle = new PlayerInBattleStruct
            {
                PlayerID = playerComponent.PlayerID,
                PlayerControlEntity = playerComponent.playerOrAI,
                PowerPoint = new PlayerStatsInBattle
                {
                    BaseValue = countUnit,
                    AbilityValue = abilityPowerPoint,
                },
                KillPoint = new PlayerStatsInBattle
                {
                    BaseValue = 0,
                    AbilityValue = abilityKillPoint,
                },
                DefencePoint = new PlayerStatsInBattle
                {
                    BaseValue = 0,
                    AbilityValue = abilityDefencePoint,
                }
            };

            return newPlayerInBattle;
        }

        private PlayerInBattleStruct CreateNeutralPlayerInBattle()
        {
            var countUnit = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == _targetDistrictGUID 
                    && unit.PowerSolidPlayerID == neutral_unit_id)
                .Count();

            var newPlayerInBattle = new PlayerInBattleStruct
            {
                PlayerID = neutral_unit_id,
                PlayerControlEntity = PlayerOrAI.None,
                PowerPoint = new PlayerStatsInBattle
                {
                    BaseValue = countUnit,
                    AbilityValue = 0,
                },
                KillPoint = new PlayerStatsInBattle
                {
                    BaseValue = 0,
                    AbilityValue = 0,
                },
                DefencePoint = new PlayerStatsInBattle
                {
                    BaseValue = 0,
                    AbilityValue = 0,
                }
            };

            return newPlayerInBattle;
        }

        private void OpenTacticsScreen()
        {
            Debug.LogError("Start select card ui");
            
            //ref var roundData = ref _dataWorld.OneData<RoundData>();
            //roundData.CurrentGameStateMapVSArena = GameStateMapVSArena.Arena;
            ref var arenaData = ref _dataWorld.OneData<ArenaData>();
            arenaData.IsShowVisualBattle = true;
            
            BattleAction.OpenTacticsScreen?.Invoke();   
        }

        private void FinishBattle()
        {
            
        }

        public void Destroy()
        {
            BattleAction.EndMovePlayerToNewDistrict -= CheckCountEnemyInTargetDistrict;
            BattleAction.FinishBattle -= FinishBattle;
        }
    }
}