using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.Player;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaSupportCalculateSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.CheckBlockAttack += CheckBlockAttack;
            ArenaAction.CheckFinishArenaBattle += CheckEndRound;
            ArenaAction.UpdateTurnOrderArena += UpdateTurnOrderArena;
            ArenaAction.UpdateTurnOrderArena += FindPlayerInCurrentRound;
        }

        private bool CheckBlockAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            var countCardInHandPlayer = _dataWorld.Select<CardComponent>()
                .Where<CardComponent>(card => card.PlayerID == targetUnitComponent.PlayerControlID)
                .With<CardHandComponent>()
                .Count();

            return countCardInHandPlayer > 0;
        }

        private bool CheckEndRound()
        {
            ClearPlayersNotUnit();
            
            var playersCountInArena = _dataWorld.Select<PlayerArenaInBattleComponent>().Count();
            if (playersCountInArena == 1)
                return true;
            
            var forwardsArenaPlayerID = 0;
            
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();

            foreach (var playerEntity in playersInBattleEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                if (playerComponent.Forwards)
                {
                    forwardsArenaPlayerID = playerComponent.PlayerID;
                    break;
                }
            }
            
            var countForwardsUnit = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == forwardsArenaPlayerID)
                .Count();

            return countForwardsUnit == 0;
        }
        
        private void ClearPlayersNotUnit()
        {
            var playerEntities = _dataWorld.Select<PlayerArenaInBattleComponent>().GetEntities();
            
            foreach (var playerEntity in playerEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();

                var countUnit = _dataWorld.Select<ArenaUnitComponent>()
                    .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == playerComponent.PlayerID)
                    .Count();
                if (countUnit == 0)
                    playerEntity.Destroy();
            }
        }

        private void UpdateTurnOrderArena()
        {
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();
            
            var playersCountInBattle = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Count();

            foreach (var playerEntity in playersInBattleEntities)
            {
                ref var playerComponent = ref playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                playerComponent.PositionInTurnQueue--;

                if (playerComponent.PositionInTurnQueue < 0)
                {
                    playerComponent.PositionInTurnQueue = playersCountInBattle -1;
                }
            }
        }
        
        private void FindPlayerInCurrentRound()
        {
            ref var roundData = ref _dataWorld.OneData<ArenaRoundData>();
            var playersInBattleEntities = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .GetEntities();

            var positionInTurnQueue = 50;
            foreach (var playerEntity in playersInBattleEntities)
            {
                var playerComponent = playerEntity.GetComponent<PlayerArenaInBattleComponent>();
                if (playerComponent.PositionInTurnQueue < positionInTurnQueue)
                {
                    positionInTurnQueue = playerComponent.PositionInTurnQueue;
                    
                    roundData.PlayerControlEntity = playerComponent.PlayerControlEntity;
                    roundData.CurrentPlayerID = playerComponent.PlayerID;
                }
            }

            var currentPlayerID = roundData.CurrentPlayerID;
            
            var playerEntityCurrentRound = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Where<PlayerArenaInBattleComponent>(player => player.PlayerID == currentPlayerID)
                .SelectFirstEntity();
            playerEntityCurrentRound.AddComponent(new CurrentPlayerComponent());
        }

        public void Destroy() { }
    }
}