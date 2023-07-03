using System;
using UnityEngine.Serialization;

namespace CyberNet.Core.Ability
{
    [Serializable]
    public struct AbilityData
    {
        public int TotalAttack;
        public int TotalTrade;
        public int TotalInfluence;

        public int SpendAttack;
        public int SpendTrade;
        public int SpendInfluence;
        
        public ActionType ActionType;
    }
}