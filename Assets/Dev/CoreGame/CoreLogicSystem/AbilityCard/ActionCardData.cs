using System;
using UnityEngine.Serialization;

namespace CyberNet.Core.AbilityCard
{
    [Serializable]
    public struct ActionCardData
    {
        public int TotalAttack;
        public int TotalTrade;

        public int SpendAttack;
        public int SpendTrade;
        
        public ActionPlayerButtonType ActionPlayerButtonType;
    }
}