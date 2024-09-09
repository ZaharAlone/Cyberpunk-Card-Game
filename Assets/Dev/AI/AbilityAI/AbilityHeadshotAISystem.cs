using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;
using CyberNet.Core.AI;
using CyberNet.Core.Arena;
using CyberNet.Core.Player;

namespace CyberNet.Core.AI.Ability
{
    [EcsSystem(typeof(CoreModule))]
    public class AbilityHeadshotAISystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            AbilityAIAction.Headshot += SelectTargetEnemyToHeadshot;
        }
        
        private void SelectTargetEnemyToHeadshot(string guidCard)
        {
            //Находим первого попавшегося противника на арене
            var selectEnemyPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Without<CurrentPlayerComponent>()
                .SelectFirstEntity();
            var selectEnemyPlayerComponent = selectEnemyPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();

            var selectEnemyUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == selectEnemyPlayerComponent.PlayerID)
                .SelectFirst<ArenaUnitComponent>();
            
            
            ArenaAction.KillUnitGUID?.Invoke(selectEnemyUnitComponent.GUID);
            ArenaAction.SelectUnitEnemyTargetingPlayer?.Invoke();

            //TODO дописать, пока не дообрабатывается сброс карты
        }

        public void Destroy()
        {
            AbilityAIAction.Headshot -= SelectTargetEnemyToHeadshot;
        }
    }
}