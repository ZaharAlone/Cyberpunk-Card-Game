using System;
using CyberNet.Core.Map;
using CyberNet.Global;

namespace CyberNet.Core.Battle
{
    [Serializable]
    public struct PlayerInBattleStruct
    {
        public int PlayerID;
        public PlayerOrAI PlayerControlEntity;

        public PlayerStatsInBattle PowerPoint;
        public PlayerStatsInBattle KillPoint;
        public PlayerStatsInBattle DefencePoint;
    }

    [Serializable]
    public struct PlayerStatsInBattle
    {
        public int BaseValue;
        public int AbilityValue;
        public int CardValue;
    }
}