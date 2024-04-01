using System;
using CyberNet.Core.City;

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
        public static Action UpdatePlayerInputsRound;
        
        //Support
        public static Func<bool> CheckBlockAttack;
        public static Func<bool> CheckFinishArenaBattle;
        public static Action UpdateTurnOrderArena;
        public static Action FindPlayerInCurrentRound;
        public static Action CreateBattleData;
        public static Action CreateUnitInArena;
        public static Action DeselectPlayer;
        public static Func<PlayerControlEntity, int, string> GetKeyPlayerVisual;
    }
}