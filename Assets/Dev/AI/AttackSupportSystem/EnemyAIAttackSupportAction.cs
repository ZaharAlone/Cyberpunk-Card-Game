using System;
using System.Collections.Generic;

namespace CyberNet.Core.AI
{
    public static class EnemyAIAttackSupportAction
    {
        public static Func<List<BuildFreeSlotStruct>> GetTowerFreeSlotPlayerPresence;
        public static Func<List<BuildFreeSlotStruct>> GetConnectPointFreeSlotPlayerPresence;
        public static Func<List<EnemyInBuildLink>> CheckEnemyPresenceInBuild;
        public static Func<string> SelectTargetAttack;
    }
}