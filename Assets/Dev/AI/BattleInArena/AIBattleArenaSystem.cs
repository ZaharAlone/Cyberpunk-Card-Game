using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.City;
using CyberNet.Core.Map;
using CyberNet.Core.Player;
using CyberNet.Global;

namespace CyberNet.Core.AI.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class AIBattleArenaSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AIBattleArenaAction.CheckEnemyBattle += CheckEnemy;
        }

        private void CheckEnemy()
        {
            var unitEntities = _dataWorld.Select<UnitInBattleArenaComponent>().GetEntities();

            var isPlayerEnemy = false;

            foreach (var unitEntity in unitEntities)
            {
                var unitComponent = unitEntity.GetComponent<UnitMapComponent>();
                if (unitComponent.PlayerControl != PlayerControlEnum.Player)
                    continue;

                var playerEntity = _dataWorld.Select<PlayerComponent>()
                    .Where<PlayerComponent>(player => player.PlayerID == unitComponent.PowerSolidPlayerID)
                    .SelectFirstEntity();
                if (playerEntity.GetComponent<PlayerComponent>().PlayerTypeEnum == PlayerTypeEnum.Player)
                {
                    isPlayerEnemy = true;
                    break;
                }
            }

            if (!isPlayerEnemy)
            {
                CalculateBattle();
            }
            else
            {
                MapMoveUnitsAction.ZoomCameraToBattle?.Invoke();
            }
        }

        private void CalculateBattle()
        {
            Debug.LogError("Calculate Battle");
            
            
        }

        public void Destroy() { }
    }
}