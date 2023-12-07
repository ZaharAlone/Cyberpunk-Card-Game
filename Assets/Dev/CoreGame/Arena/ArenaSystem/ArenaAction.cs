using System;

namespace CyberNet.Core.Arena
{
    public static class ArenaAction
    {
        public static Action StartArenaBattle;
        public static Action DisableArenaBattle;

        public static Action UpdateRound;

        public static Action ArenaUnitStartAttack;
        public static Action ArenaUnitFinishAttack;

        public static Action FinishRound;
    }
}