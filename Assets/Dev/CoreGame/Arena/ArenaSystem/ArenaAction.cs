using System;

namespace CyberNet.Core.Arena
{
    public static class ArenaAction
    {
        public static Action StartArenaBattle;
        public static Action EndBattleArena;

        public static Action UpdateRound;

        public static Action ArenaUnitStartAttack;
        public static Action ArenaUnitFinishAttack;

        public static Action FinishRound;
        
        //Support
        public static Func<bool> CheckBlockAttack;
        public static Func<bool> CheckFinishArenaBattle;
        public static Action UpdateTurnOrderArena;
    }
}