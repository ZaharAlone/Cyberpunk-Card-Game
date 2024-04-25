using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Global;
using CyberNet.Global.Sound;
using CyberNet.Tools;
using UnityEngine;

namespace CyberNet.Core.Arena.Bullet
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBulletSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        private const float time_life_bullet = 3;
        private const float time_life_bullet_after_collision = 1;
        
        public void PreInit()
        {
            UnitArenaAction.CreateBulletCurrentUnit += CreateBullet;
            UnitArenaAction.BulletCollision += BulletCollision;
        }
        
        private void CreateBullet()
        {
            var currentUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaUnitCurrentComponent>()
                .SelectFirst<ArenaUnitComponent>();
            
            var bulletMono = currentUnitComponent.UnitArenaMono.ShootingCreateBullet();
            var bulletGUID = CreateGUID.Create();
            bulletMono.SetGUID(bulletGUID);
            
            var bulletEntity = _dataWorld.NewEntity();
            
            bulletEntity.AddComponent(new BulletComponent {
                GUID = bulletGUID,
                BulletMono = bulletMono,
            });

            bulletEntity.AddComponent(new BulletMoveComponent());
            bulletEntity.AddComponent(new TimeComponent
            {
                Time = time_life_bullet,
                Action = () => DestroyBullet(bulletEntity),
            });
        }
        
        private void BulletCollision(BulletCollisionStruct bulletCollision)
        {
            var targetBulletEntity = _dataWorld.Select<BulletComponent>()
                .Where<BulletComponent>(bullet => bullet.GUID == bulletCollision.GUID)
                .SelectFirstEntity();

            targetBulletEntity.RemoveComponent<BulletMoveComponent>();
            targetBulletEntity.RemoveComponent<TimeComponent>();
            
            var bulletComponent = targetBulletEntity.GetComponent<BulletComponent>();
            bulletComponent.BulletMono.PlayEffectHit();
            bulletComponent.BulletMono.StopFlyBullet();

            var soundHit = _dataWorld.OneData<SoundData>().Sound.HitUnit;
            SoundAction.PlaySound?.Invoke(soundHit);

            targetBulletEntity.AddComponent(new TimeComponent {
                Time = time_life_bullet_after_collision,
                Action = () => DestroyBullet(targetBulletEntity),
            });

            EffectHitTarget();
        }

        private void EffectHitTarget()
        {
            var targetUnitComponent = _dataWorld.Select<ArenaUnitComponent>()
                .With<ArenaSelectUnitForAttackComponent>()
                .SelectFirst<ArenaUnitComponent>();
            targetUnitComponent.UnitArenaMono.HitAnimations();
        }
        
        public void DestroyBullet(Entity bulletEntity)
        {
            var bulletComponent = bulletEntity.GetComponent<BulletComponent>();
            bulletComponent.BulletMono.DestroyBulletToTime();
        }

        public void Destroy()
        {
            var bulletEntities = _dataWorld.Select<BulletComponent>()
                .GetEntities();
            
            foreach (var bulletEntity in bulletEntities)
            {
                DestroyBullet(bulletEntity);
            }
            
            UnitArenaAction.CreateBulletCurrentUnit -= CreateBullet;
            UnitArenaAction.BulletCollision -= BulletCollision;
        }
    }
}