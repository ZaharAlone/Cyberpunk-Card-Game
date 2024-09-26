/*
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
            CutsceneArenaAction.StartShootingPlayerWithoutShield += StartShootingPlayerWithoutShield;
            CutsceneArenaAction.StartShootingPlayerWithShield += StartShootingPlayerWithShield;
            CutsceneArenaAction.KillUnitGUID += KillUnitGUID;
        }
        
        private void StartShootingPlayerWithoutShield()
        {
            CutsceneArenaAction.ArenaUnitFinishAttack += ArenaUnitFinishAttack;
            Shooting();
        }

        private void StartShootingPlayerWithShield()
        {
            CutsceneArenaAction.ArenaUnitFinishAttack += FinishBlockAttack;
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
            CutsceneArenaAction.ArenaUnitFinishAttack -= ArenaUnitFinishAttack;

            PlayIdleAnimationsEndAttackUnit();
            
            //async for effect
            await Task.Delay(150);

            var targetUnitEntity = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirstEntity();
            
            KillUnit(targetUnitEntity);

            CutsceneArenaAction.FinishRound?.Invoke();
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
            CutsceneArenaAction.ArenaUnitFinishAttack -= FinishBlockAttack;

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
            
            CutsceneArenaAction.FinishRound?.Invoke();
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
            
            CutsceneArenaAction.ArenaUnitFinishAttack?.Invoke();
        }
        
        public void Destroy()
        {
        }
    }
}
*/