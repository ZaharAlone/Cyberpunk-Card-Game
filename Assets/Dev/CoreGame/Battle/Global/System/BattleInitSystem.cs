using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System.Collections.Generic;
using CyberNet.Core.AI;
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

        //Начало боя, сначала проверяем сколько фракций на территории, если больше 2х,
        //выбираем с кем будем сражаться
        private void CheckCountEnemyInTargetDistrict(string targetDistrictGUID)
        {
            var listUniquePlayers = CalculateCountUniquePlayersInTargetDistrict(targetDistrictGUID);
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            listUniquePlayers.Remove(currentPlayerID);
            
            _targetDistrictGUID = targetDistrictGUID;

            _dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena = GameStateMapVSArena.Arena;
            
            if (listUniquePlayers.Count > 1)
                SelectEnemyToBattle(listUniquePlayers);
            else
                StartBattle(listUniquePlayers[0]);
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

            var playerInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();

            var isWaitPlayerSelectTactics = false;
            foreach (var playerInBattleEntity in playerInBattleEntities)
            {
                var playerComponent = playerInBattleEntity.GetComponent<PlayerInBattleComponent>();

                if (playerComponent.PlayerControlEntity == PlayerOrAI.Player)
                {
                    var playerForCurrentDevice = playerInBattleEntity.HasComponent<PlayerCurrentDeviceControlComponent>();

                    if (playerForCurrentDevice)
                    {
                        BattleAction.OpenTacticsScreen?.Invoke(playerComponent.PlayerID);
                        isWaitPlayerSelectTactics = true;
                    }
                }
                else if (playerComponent.PlayerControlEntity == PlayerOrAI.None)
                {
                    BattleAction.SelectTacticsCardNeutralUnit?.Invoke();
                }
                else
                {
                    BattleAction.SelectTacticsAI?.Invoke(playerComponent.PlayerID);
                }
            }

            if (!isWaitPlayerSelectTactics)
            {
                BattleAction.StartBattleInMap?.Invoke();
            }
        }
        
        private void CreateBattleData(int enemyID)
        {
            var currentPlayerID = _dataWorld.OneData<RoundData>().CurrentPlayerID;
            var battleData = new BattleCurrentData();

            battleData.DistrictBattleGUID = _targetDistrictGUID;
            _dataWorld.CreateOneData(battleData);
            
            CreatePlayerInBattleComponent(currentPlayerID, true);
            
            if (enemyID == neutral_unit_id)
                CreateNeutralPlayerInBattle();
            else
                CreatePlayerInBattleComponent(enemyID, false);
        }

        private void CreatePlayerInBattleComponent(int playerID, bool isAttacking)
        {
            var playerEntity = _dataWorld.Select<PlayerComponent>()
                .Where<PlayerComponent>(player => player.PlayerID == playerID)
                .SelectFirstEntity();
            var playerComponent = playerEntity.GetComponent<PlayerComponent>();
            
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
            
            var playerInBattleComponent = new PlayerInBattleComponent
            {
                PlayerID = playerComponent.PlayerID,
                PlayerControlEntity = playerComponent.playerOrAI,
                IsAttacking = isAttacking,
                PowerPoint = countUnit,
                KillPoint = 0,
                DefencePoint = 0,
            };

            playerEntity.AddComponent(playerInBattleComponent);
        }

        private void CreateNeutralPlayerInBattle()
        {
            var countUnit = _dataWorld.Select<UnitMapComponent>()
                .Where<UnitMapComponent>(unit => unit.GUIDDistrict == _targetDistrictGUID 
                    && unit.PowerSolidPlayerID == neutral_unit_id)
                .Count();

            var neutralPlayerInBattleComponent = new PlayerInBattleComponent
            {
                PlayerID = neutral_unit_id,
                PlayerControlEntity = PlayerOrAI.None,
                PowerPoint = countUnit,
                KillPoint = 0,
                DefencePoint = 0,
            };

            var neutralPlayerInBattleEntity = _dataWorld.NewEntity();
            neutralPlayerInBattleEntity.AddComponent(neutralPlayerInBattleComponent);
        }

        private void FinishBattle()
        {
            Debug.Log("Finish battle");
            _dataWorld.OneData<RoundData>().CurrentGameStateMapVSArena = GameStateMapVSArena.Map;

            var playersInBattleEntities = _dataWorld.Select<PlayerInBattleComponent>().GetEntities();
            foreach (var playerInBattleEntity in playersInBattleEntities)
            {
                playerInBattleEntity.RemoveComponent<PlayerInBattleComponent>();
                playerInBattleEntity.RemoveComponent<SelectTacticsAndCardComponent>();

                if (playerInBattleEntity.HasComponent<PlayerLoseBattleComponent>())
                    playerInBattleEntity.RemoveComponent<PlayerLoseBattleComponent>();

                if (playerInBattleEntity.HasComponent<PlayerWinBattleComponent>())
                    playerInBattleEntity.RemoveComponent<PlayerWinBattleComponent>();
            }

            var playerComponent = _dataWorld.Select<PlayerComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirst<PlayerComponent>();
            
            if (playerComponent.playerOrAI != PlayerOrAI.Player)
                BotAIAction.ContinuePlayingCards?.Invoke();
        }

        public void Destroy()
        {
            BattleAction.EndMovePlayerToNewDistrict -= CheckCountEnemyInTargetDistrict;
            BattleAction.FinishBattle -= FinishBattle;
        }
    }
}