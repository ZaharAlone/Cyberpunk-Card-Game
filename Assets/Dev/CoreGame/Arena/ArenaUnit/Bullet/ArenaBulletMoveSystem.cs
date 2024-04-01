using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using UnityEngine;
using System;

namespace CyberNet.Core.Arena.Bullet
{
    [EcsSystem(typeof(CoreModule))]
    public class ArenaBulletMoveSystem : IRunPhysicSystem
    {
        private DataWorld _dataWorld;

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
    }
}