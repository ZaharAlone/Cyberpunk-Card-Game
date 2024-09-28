using System;

namespace CyberNet.Core.Battle.TacticsMode
{
    [Serializable]
    public struct BattleTactics
    {
        public string Key;
        public BattleCharacteristics LeftCharacteristics;
        public BattleCharacteristics RightCharacteristics;
    }
}