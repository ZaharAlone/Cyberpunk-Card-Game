using System;
namespace CyberNet.Core.UI
{
    public static class BoardGameUIAction
    {
        public static Action UpdateStatsMainPlayersPassportUI;
        public static Action UpdateStatsAllPlayersPassportUI;
        public static Action UpdateStatsPlayersCurrency;
        public static Action UpdateCountCardInHand;
        public static Action<bool> ControlVFXCurrentPlayerArena;
    }
}