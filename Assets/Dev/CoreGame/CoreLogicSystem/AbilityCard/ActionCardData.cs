using System;
using CyberNet.Core.UI.ActionButton;
using UnityEngine.Serialization;

namespace CyberNet.Core.AbilityCard
{
    [Serializable]
    public struct ActionCardData
    {
        public int TotalTrade;
        public int SpendTrade;
        public int BonusDistrictTrade;

        public ActionPlayerButtonType ActionPlayerButtonType;
    }
}