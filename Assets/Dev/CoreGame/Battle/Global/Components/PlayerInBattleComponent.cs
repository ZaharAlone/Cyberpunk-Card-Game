using System;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [Serializable]
    public struct PlayerInBattleComponent
    {
        public int PlayerID;
        public PlayerOrAI PlayerControlEntity;
        public bool IsAttacking;

        public int PowerPoint;
        public int KillPoint;
        public int DefencePoint;
    }
}