using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Player;
using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaAINeutralLogicSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAIAction.StartAINeutralLogic += StartAINeutralLogic;
        }
        
        private void StartAINeutralLogic()
        {
            _dataWorld.NewEntity().AddComponent(new TimeComponent
            {
                Time = 1f,
                Action = () => AttackEnemy()
            });
        }

        private void AttackEnemy()
        {
            var selectEnemyPlayerEntity = _dataWorld.Select<PlayerArenaInBattleComponent>()
                .Without<CurrentPlayerComponent>()
                .Where<PlayerArenaInBattleComponent>(playerArena => playerArena.Forwards)
                .SelectFirstEntity();
            var selectEnemyPlayerComponent = selectEnemyPlayerEntity.GetComponent<PlayerArenaInBattleComponent>();

            var selectEnemyUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.PlayerControlID == selectEnemyPlayerComponent.PlayerID)
                .SelectFirstEntity();
            selectEnemyUnitEntity.AddComponent(new ArenaSelectUnitForAttackComponent());
            
            var colorsConfig = _dataWorld.OneData<BoardGameData>().BoardGameConfig.ColorsGameConfigSO;
            var selectEnemyUnitComponent = selectEnemyUnitEntity.GetComponent<ArenaUnitComponent>();
            
            selectEnemyUnitComponent.UnitArenaMono.UnitPointVFXMono.SetColor(colorsConfig.SelectWrongTargetRedColor, true);
            selectEnemyUnitComponent.UnitArenaMono.UnitPointVFXMono.EnableEffect();
            
            ArenaAction.ArenaUnitPlayerStartShooting?.Invoke();
        }

        public void Destroy()
        {
            ArenaAIAction.StartAINeutralLogic -= StartAINeutralLogic;
        }
    }
}