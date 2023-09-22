using System;
using System.Collections.Generic;

namespace CyberNet.Core.AI
{
    public static class EnemyAISupportAction
    {
        public static Func<List<BuildFreeSlotStruct>> GetTowerFreeSlotPlayerPresence;
        public static Func<List<BuildFreeSlotStruct>> GetConnectPointFreeSlotPlayerPresence;
        public static Func<List<EnemyInTowerLink>> CheckEnemyPresenceInTower;
    }
}