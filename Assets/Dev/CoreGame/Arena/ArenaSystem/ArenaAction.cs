using System;
using CyberNet.Core.City;

namespace CyberNet.Core.Arena
{
    public static class ArenaAction
    {
        public static Action StartArenaBattle;
        public static Action EndBattleArena;

        public static Action UpdateRound;

        public static Action ArenaUnitAIStartShooting;
        public static Action ArenaUnitFinishAttack;

        public static Action FinishRound;
        public static Action UpdatePlayerInputsRound;
        
        //Support
        public static Func<bool> CheckBlockAttack;
        public static Func<bool> CheckReactionsShooting;
        
        public static Func<bool> CheckFinishArenaBattle;
        public static Action UpdateTurnOrderArena;
        public static Action FindPlayerInCurrentRound;
        public static Action CreateBattleData;
        public static Action CreateUnitInArena;
        public static Action DeselectPlayer;
        public static Func<PlayerControlEntity, int, string> GetKeyPlayerVisual;

        public static Action StartShootingPlayerWithoutShield;
        public static Action StartShootingPlayerWithShield;
        public static Action StartInteractiveBlockingShooting;

        public static Action<string> KillUnitGUID;

        public static Action SelectUnitEnemyTargetingPlayer;
    }
}