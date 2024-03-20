using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Global;
using CyberNet.Global.Sound;
using CyberNet.Tools;

namespace CyberNet.Core.Arena
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBulletSystem : IPreInitSystem, IRunPhysicSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            UnitArenaAction.CreateBulletCurrentUnit += CreateBullet;
            UnitArenaAction.BulletCollision += BulletCollision;
        }
        private void BulletCollision(string guid)
        {
            var targetBulletEntity = _dataWorld.Select<BulletComponent>()
                .Where<BulletComponent>(bullet => bullet.GUID == guid)
                .SelectFirstEntity();

            targetBulletEntity.RemoveComponent<BulletMoveComponent>();
            targetBulletEntity.RemoveComponent<TimeComponent>();
            
            var bulletComponent = targetBulletEntity.GetComponent<BulletComponent>();
            bulletComponent.BulletMono.PlayEffectHit();

            var soundHit = _dataWorld.OneData<SoundData>().Sound.HitUnit;
            SoundAction.PlaySound?.Invoke(soundHit);

            targetBulletEntity.AddComponent(new TimeComponent {
                Time = 1,
                Action = () => DestroyBulletInTime(targetBulletEntity),
            });
        }

        public void CreateBullet()
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
                Time = 5,
                Action = () => DestroyBulletInTime(bulletEntity)
            });
        }

        public void RunPhysic()
        {
            var bulletEntities = _dataWorld.Select<BulletComponent>()
                .With<BulletMoveComponent>()
                .GetEntities();

            foreach (var bulletEntity in bulletEntities)
            {
                MoveBullet(bulletEntity);
            }
        }

        private void MoveBullet(Entity bulletEntity)
        {
            var bulletComponent = bulletEntity.GetComponent<BulletComponent>();
            bulletComponent.BulletMono.Move();
        }
        
        public void DestroyBulletInTime(Entity bulletEntity)
        {
            var bulletComponent = bulletEntity.GetComponent<BulletComponent>();
            bulletComponent.BulletMono.DestroyBulletToTime();
        }

        public void Destroy()
        {
            UnitArenaAction.CreateBulletCurrentUnit -= CreateBullet;
            UnitArenaAction.BulletCollision -= BulletCollision;
        }
    }
}