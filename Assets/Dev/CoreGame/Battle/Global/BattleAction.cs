using System;
using System.Collections.Generic;
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

        public static Func<int, int> CalculatePlayerMaxPower;
        public static Func<int, List<CardSelectTacticsPotential>> CalculatePlayerCardsPotential;
    }
}