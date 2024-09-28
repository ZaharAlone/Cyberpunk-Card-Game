using System;
using CyberNet.Core.Map;

namespace CyberNet.Core.Battle
{
    [Serializable]
    public struct PlayerInBattleStruct
    {
        public int PlayerID;
        public PlayerControlEntity PlayerControlEntity;

        public PlayerStatsInBattle PowerPoint;
        public PlayerStatsInBattle KillPoint;
        public PlayerStatsInBattle DefencePoint;
    }

    [Serializable]
    public struct PlayerStatsInBattle
    {
        public int BaseValue;
        public int AbilityValue;
    }
}