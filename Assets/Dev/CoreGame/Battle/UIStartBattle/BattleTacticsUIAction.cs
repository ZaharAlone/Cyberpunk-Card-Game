using System;

namespace CyberNet.Core.Battle.TacticsMode
{
    public static class BattleTacticsUIAction
    {
        public static Action<string> SelectBattleTactics;

        public static Action NextLeftBattleTactics;
        public static Action NextRightBattleTactics;
    }
}