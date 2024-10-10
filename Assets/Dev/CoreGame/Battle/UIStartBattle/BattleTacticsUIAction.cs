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

        public static Action<string> SelectCard;
        public static Action<string> DeselectCard;
        public static Action<string> StartMoveCard;
        public static Action<string> EndMoveCard;
    }
}