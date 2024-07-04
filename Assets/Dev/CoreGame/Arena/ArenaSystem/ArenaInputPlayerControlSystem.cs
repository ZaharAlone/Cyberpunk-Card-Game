using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI.Arena;
using CyberNet.Core.Arena;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI;
using CyberNet.Global;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaInputPlayerControlSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.UpdatePlayerInputsRound += EnableControlPlayer;
        }

        private void EnableControlPlayer()
        {
            var currentPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirstEntity();

            var currentPlayerComponent = currentPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();
            var isControlPlayer = false;
            
            if (currentPlayerComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
            {
                ArenaAIAction.StartAINeutralLogic?.Invoke();
                BoardGameUIAction.ControlVFXCurrentPlayerArena?.Invoke(false);
            }
            else
            {
                var playerGlobalEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == currentPlayerComponent.PlayerID)
                    .SelectFirstEntity();
                var playerGlobalComponent = playerGlobalEntity.GetComponent<PlayerComponent>();

                if (playerGlobalComponent.playerOrAI == PlayerOrAI.Player)
                {
                    ArenaUIAction.ShowHUDButton?.Invoke();
                    BoardGameUIAction.ControlVFXCurrentPlayerArena?.Invoke(true);
                    isControlPlayer = true;
                }
                else
                {
                    AIBattleArenaAction.StartAIRound?.Invoke();
                    BoardGameUIAction.ControlVFXCurrentPlayerArena?.Invoke(false);
                }
            }
            
            SwitchPlayerControl(isControlPlayer);
            ManagingUnitColliders();
        }

        private void SwitchPlayerControl(bool isControlPlayer)
        {
            if (isControlPlayer)
            {
                var controlPlayerEntity = _dataWorld.NewEntity();
                controlPlayerEntity.AddComponent(new PlayerStageChoosesAnOpponentComponent());
                
                ArenaAction.SelectUnitEnemyTargetingPlayer?.Invoke();
            }
            else
            {
                var controlPlayerQuery = _dataWorld.Select<PlayerStageChoosesAnOpponentComponent>();
                if (controlPlayerQuery.Count() > 0)
                {
                    controlPlayerQuery.SelectFirstEntity().Destroy();   
                }
            }
        }

        private void ManagingUnitColliders()
        {
            var currentPlayerComponent = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .With<CurrentPlayerComponent>()
                .SelectFirst<PlayerArenaInBattleComponent>();

            var currentPlayerUnitEntities = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == currentPlayerComponent.PlayerID)
                .GetEntities();
            
            foreach (var unitEntity in currentPlayerUnitEntities)
            {
                var unitMono = unitEntity.GetComponent<ArenaUnitComponent>().UnitArenaMono;
                unitMono.DisableCollider();
            }

            var enemyUnitEntities = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID != currentPlayerComponent.PlayerID)
                .GetEntities();
            
            foreach (var unitEntity in enemyUnitEntities)
            {
                var unitMono = unitEntity.GetComponent<ArenaUnitComponent>().UnitArenaMono;
                unitMono.EnableCollider();
            }
        }

        public void Destroy()
        {
            var controlPlayerEntities = _dataWorld.Select<PlayerStageChoosesAnOpponentComponent>()
                .GetEntities();

            foreach (var controlPlayer in controlPlayerEntities)
            {
                controlPlayer.Destroy();
            }

            ArenaAction.UpdatePlayerInputsRound -= EnableControlPlayer;
        }
    }
}