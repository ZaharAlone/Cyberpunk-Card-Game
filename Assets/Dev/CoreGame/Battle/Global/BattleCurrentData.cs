using System;
using UnityEngine.Serialization;

namespace CyberNet.Core.Battle.TacticsMode
{
    [Serializable]
    public struct BattleCurrentData
    {
        public string DistrictBattleGUID;
        
        public PlayerInBattleStruct AttackingPlayer;
        public PlayerInBattleStruct DefendingPlayer;
    }
}