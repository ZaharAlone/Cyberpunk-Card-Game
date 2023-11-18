using System;
using UnityEngine.Serialization;

namespace CyberNet.Core.AbilityCard
{
    [Serializable]
    public struct ActionCardData
    {
        public int TotalTrade;

        public int SpendTrade;
        
        public ActionPlayerButtonType ActionPlayerButtonType;
    }
}