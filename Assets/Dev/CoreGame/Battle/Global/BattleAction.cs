using System;
using CyberNet.Core.Battle.TacticsMode;

namespace CyberNet.Core.Battle
{
    public static class BattleAction
    {
        public static Action<string> EndMovePlayerToNewDistrict;
        public static Action FinishBattle;
        
        public static Action<int> OpenTacticsScreen;

        public static Action<int> SelectTacticsAI;
        public static Action SelectTacticsCardNeutralUnit;
    }
}