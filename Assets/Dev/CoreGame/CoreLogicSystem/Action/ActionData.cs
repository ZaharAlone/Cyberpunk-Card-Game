using System;

namespace CyberNet.Core
{
    [Serializable]
    public struct ActionData
    {
        public int TotalAttack;
        public int TotalTrade;
        public int TotalInfluence;

        public int SpendAttack;
        public int SpendTrade;
        public int SpendInfluence;

        public ActionType CurrentAction;
    }
}