using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.City;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaUnitAttackSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.ArenaUnitStartAttack += ArenaUnitStartAttack;
            ArenaAction.ArenaUnitFinishAttack += ArenaUnitFinishAttack;
        }
        
        private void ArenaUnitStartAttack()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            currentUnitComponent.UnitArenaMono.ShowTargetUnit(targetUnitComponent.UnitGO.transform);
            currentUnitComponent.UnitArenaMono.Shooting();
        }
        
        private void ArenaUnitFinishAttack()
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            var targetUnitMapComponent = targetUnitEntity.GetComponent<UnitMapComponent>();
            
            Object.Destroy(targetUnitComponent.UnitGO);
            Object.Destroy(targetUnitMapComponent.UnitIconsGO);
            
            targetUnitEntity.Destroy();
            
            ArenaAction.FinishRound?.Invoke();
        }

        public void Destroy()
        {
            ArenaAction.ArenaUnitStartAttack -= ArenaUnitStartAttack;
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;
        }
    }
}