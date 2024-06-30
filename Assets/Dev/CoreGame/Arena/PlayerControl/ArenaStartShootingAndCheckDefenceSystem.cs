using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using CyberNet.Core.Arena.ArenaHUDUI;
using CyberNet.Core.City;
using CyberNet.Core.Player;
using CyberNet.Core.UI.CorePopup;
using CyberNet.Global;
using CyberNet.Global.Sound;

namespace CyberNet.Core.Arena
{
    /// <summary>
    /// Старт стрельбы, и проверка может ли игрок заблокировать выстрел
    /// </summary>
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaStartShootingAndCheckDefenceSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;
        
        public void PreInit()
        {
            ArenaUIAction.ClickAttack += PlayerStartShooting;
            ArenaAction.ArenaUnitPlayerStartShooting += PlayerStartShooting;
            ArenaAction.ArenaUnitAIStartShooting += AIStartShooting;
        }
        
        private void PlayerStartShooting()
        {
            //TODO переписать, чтобы всегда был автовыбор цели
            var isEnemyAttack = _dataWorld.Select<ArenaSelectUnitForAttackComponent>()
                .Count() > 0;

            if (!isEnemyAttack)
            {
                //Show Warning frame
                Debug.LogError("Not select unit for attack");
                return;
            }
            else
            {
                ArenaUIAction.HideHUDButton?.Invoke();
                ArenaUnitAimToTargetUnit();
            }
            
            CoreElementInfoPopupAction.ClosePopupCard?.Invoke();
        }

        private void AIStartShooting()
        {
            ArenaUnitAimToTargetUnit();
        }
        
        private void ArenaUnitAimToTargetUnit()
        {
            DisableAllColliderUnit();
            
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();

            currentUnitComponent.UnitArenaMono.OnAimAnimations();
            currentUnitComponent.UnitArenaMono.ViewToTargetUnit(targetUnitComponent.UnitGO.transform);

            var soundAim = _dataWorld.OneData<SoundData>().Sound.AimGun;
            SoundAction.PlaySound?.Invoke(soundAim);
            
            _dataWorld.NewEntity().AddComponent(new TimeComponent
            {
                Time = 0.5f,
                Action = () => CheckBlockAttack()
            });
        }
        
        private void DisableAllColliderUnit()
        {
            var unitEntities = _dataWorld.Select<ArenaUnitComponent>().GetEntities();
            
            foreach (var unitEntity in unitEntities)
            {
                var unitMono = unitEntity.GetComponent<ArenaUnitComponent>().UnitArenaMono;
                unitMono.DisableCollider();
            }
        }
        
        private void CheckBlockAttack()
        {
            var isPlayerBlockAttack = TurnPlayerAndHeCanBlockAttack();
            
            if (isPlayerBlockAttack)
            {
                ArenaAction.StartInteractiveBlockingShooting?.Invoke();
            }
            else
            {
                ArenaAction.StartShootingPlayerWithoutShield?.Invoke();
            }
        }
        
        private bool TurnPlayerAndHeCanBlockAttack()
        {
            var targetUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirst<ArenaUnitComponent>();
            
            if (targetUnitComponent.PlayerControlEntity == PlayerControlEntity.NeutralUnits)
                return false;

            var enoughCardToBlock = ArenaAction.CheckBlockAttack.Invoke();

            return enoughCardToBlock;
        }
        
        public void Destroy()
        {
            ArenaAction.ArenaUnitPlayerStartShooting -= PlayerStartShooting;
            ArenaUIAction.ClickAttack -= PlayerStartShooting;
            ArenaAction.ArenaUnitAIStartShooting -= AIStartShooting;
        }
    }
}