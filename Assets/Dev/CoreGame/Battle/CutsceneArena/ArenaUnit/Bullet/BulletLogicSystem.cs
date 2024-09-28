using EcsCore;
using ModulesFramework.Attributes;
using ModulesFramework.Data;
using ModulesFramework.Systems;
using CyberNet.Core.Arena;
using CyberNet.Global.Sound;

namespace CyberNet.Core.Bullet
{
    [EcsSystem(typeof(CoreModule))]
    public class BulletLogicSystem : IPreInitSystem, IDestroySystem
    {
        private DataWorld _dataWorld;

        public void PreInit()
        {
            UnitArenaAction.BulletCollisionUnit += BulletCollisionUnit;
            UnitArenaAction.BulletCollisionNotUnit += BulletCollisionNotUnit;
        }
        
        private void BulletCollisionUnit()
        {
            var isOnShieldUnit = _dataWorld.Select<UnitOnShieldComponent>()
                .Count() > 0;

            if (isOnShieldUnit)
                PlayHitEffectBulletInShield();
            else
                PlayHitEffectBulletInUnit();
        }

        private void PlayHitEffectBulletInShield()
        {
            var soundHitShield = _dataWorld.OneData<SoundData>().Sound.HitShield;
            SoundAction.PlaySound?.Invoke(soundHitShield);
        }

        private void PlayHitEffectBulletInUnit()
        {
            var soundHitUnit = _dataWorld.OneData<SoundData>().Sound.HitUnit;
            SoundAction.PlaySound?.Invoke(soundHitUnit);
        }

        private void BulletCollisionNotUnit()
        {
            var soundHitWall = _dataWorld.OneData<SoundData>().Sound.HitWall;
            SoundAction.PlaySound?.Invoke(soundHitWall);
        }

        public void Destroy()
        {
            UnitArenaAction.BulletCollisionUnit -= BulletCollisionUnit;
        }
    }
}