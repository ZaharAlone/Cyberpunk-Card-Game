using System;
using UnityEngine.Serialization;

namespace CyberNet.Core.ActionCard
{
    [Serializable]
    public struct ActionCardData
    {
        public int TotalAttack;
        public int TotalTrade;
        public int TotalInfluence;

        public int SpendAttack;
        public int SpendTrade;
        public int SpendInfluence;
        
        [FormerlySerializedAs("ActionType")]
        public ActionCardType actionCardType;
    }
}