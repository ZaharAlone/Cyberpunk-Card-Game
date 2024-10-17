using System;

namespace CyberNet.Core.UI
{
    public static class ActionPlayerButtonEvent
    {
        public static Action ClickActionButton;
        public static Func<bool> CheckPlayerHasAnyActionsLeft;
        public static Action ForceEndRound;
        public static Action ActionEndTurnBot;
        
        public static Action UpdateActionButton;
        public static Action SetViewBattle;
    }
}