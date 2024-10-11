using System;

namespace CyberNet.Core.Battle.TacticsMode
{
    public static class BattleTacticsUIAction
    {
        public static Action CreateCardTactics;
        
        public static Action<string> SelectBattleTactics;

        public static Action NextLeftBattleTactics;
        public static Action NextRightBattleTactics;

        public static Action MoveCardToTacticsScreen;

        public static Action<string> SelectCardTactics;
        public static Action<string> DeselectCardTactics;
        
        public static Action<string> StartMoveCardTactics;
        public static Action EndMoveCardTactics;

        public static Action<string> CheckIsSelectCardTactics;
        
        public static Action UpdateCurrencyPlayerInBattle;
    }
}