using System;

namespace CyberNet.Core.Battle.TacticsMode
{
    [Serializable]
    public struct BattleCurrentData
    {
        public PlayerInBattleStruct AttackingPlayer;
        public PlayerInBattleStruct DefencePlayer;
    }
}