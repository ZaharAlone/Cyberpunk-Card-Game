using System;
using CyberNet.Core.Arena.Bullet;

namespace CyberNet.Core.Arena
{
    public static class UnitArenaAction
    {
        public static Action GunShootingVFX;
        public static Action EndShootingAnimations;
        
        public static Action CreateBulletCurrentUnit;
        public static Action<BulletCollisionStruct> BulletCollision;
    }
}