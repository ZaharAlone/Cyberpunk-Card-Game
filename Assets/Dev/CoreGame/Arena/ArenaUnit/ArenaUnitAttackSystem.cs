using System.Threading.Tasks;
using CyberNet.Core.Arena.ArenaHUDUI;
using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.City;
using CyberNet.Global.Sound;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(ArenaModule))]
    public class ArenaUnitAttackSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            ArenaAction.StartShootingPlayerWithoutShield += StartShootingPlayerWithoutShield;
            ArenaAction.StartShootingPlayerWithShield += StartShootingPlayerWithShield;
            ArenaAction.KillUnitGUID += KillUnitGUID;
        }
        
        private void StartShootingPlayerWithoutShield()
        {
            ArenaAction.ArenaUnitFinishAttack += ArenaUnitFinishAttack;
            Shooting();
        }

        private void StartShootingPlayerWithShield()
        {
            ArenaAction.ArenaUnitFinishAttack += FinishBlockAttack;
            Shooting();
        }

        private void Shooting()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.OnShootingAnimations();
            
            UnitArenaAction.GunStartShootingVFX += ShootingGunPlayVFX;
            UnitArenaAction.GunShootingSFX += ShootingGunSFX;
            UnitArenaAction.EndShootingAnimations += EndShootingAnimations;
        }

        private async void ArenaUnitFinishAttack()
        {
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;

            PlayIdleAnimationsEndAttackUnit();
            
            //async for effect
            await Task.Delay(150);

            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            
            KillUnit(targetUnitEntity);

            ArenaAction.FinishRound?.Invoke();
            ArenaUIAction.StartNewRoundUpdateOrderPlayer?.Invoke();
        }

        private void KillUnitGUID(string guidTarget)
        {
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .Where<ArenaUnitComponent>(unit => unit.GUID == guidTarget)
                .SelectFirstEntity();
            KillUnit(targetUnitEntity);
        }
        
        private void KillUnit(Entity targetUnitEntity)
        {
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            var targetUnitMapComponent = targetUnitEntity.GetComponent<UnitMapComponent>();

            Object.DestroyImmediate(targetUnitComponent.UnitGO);
            Object.DestroyImmediate(targetUnitMapComponent.UnitIconsGO);

            targetUnitEntity.Destroy();
        }
        
        private void FinishBlockAttack()
        {
            ArenaAction.ArenaUnitFinishAttack -= FinishBlockAttack;

            PlayIdleAnimationsEndAttackUnit();
            
            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            var targetUnitComponent = targetUnitEntity.GetComponent<ArenaUnitComponent>();
            targetUnitComponent.UnitArenaMono.OffShield();

            var isOnShield = _dataWorld.Select<UnitOnShieldComponent>()
                .TrySelectFirstEntity(out var onShieldEntity);
            
            if (isOnShield)
                onShieldEntity.Destroy();
            
            ArenaAction.FinishRound?.Invoke();
            ArenaUIAction.StartNewRoundUpdateOrderPlayer?.Invoke();
        }

        private void PlayIdleAnimationsEndAttackUnit()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.OnIdleAnimations();
        }
        
        private void ShootingGunPlayVFX()
        {
            var currentUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirstEntity();
            var currentUnitComponent = currentUnitEntity.GetComponent<ArenaUnitComponent>();
            currentUnitComponent.UnitArenaMono.ShootingGunPlayVFX();
        }

        private void ShootingGunSFX()
        {
            var soundShoot = _dataWorld.OneData<SoundData>().Sound.Shoot;
            SoundAction.PlaySound?.Invoke(soundShoot);
        }

        public void EndShootingAnimations()
        {
            UnitArenaAction.EndShootingAnimations -= EndShootingAnimations;
            UnitArenaAction.GunStartShootingVFX -= ShootingGunPlayVFX;
            UnitArenaAction.GunShootingSFX -= ShootingGunSFX;
            
            ArenaAction.ArenaUnitFinishAttack?.Invoke();
        }
        
        public void Destroy()
        {
            ArenaAction.StartShootingPlayerWithoutShield -= StartShootingPlayerWithoutShield;
            ArenaAction.StartShootingPlayerWithShield -= StartShootingPlayerWithShield;
            ArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;
            ArenaAction.KillUnitGUID -= KillUnitGUID;
        }
    }
}