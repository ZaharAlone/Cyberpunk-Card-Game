using CyberNet.Global;

namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        public GameStateMapVSArena CurrentGameStateMapVSArena;
        public PlayerOrAI playerOrAI;
        public bool PauseInteractive;
    }
}