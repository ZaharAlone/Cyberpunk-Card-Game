using CyberNet.Global;

namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        public RoundState CurrentRoundState;
        public PlayerTypeEnum PlayerTypeEnum;
        public bool PauseInteractive;
    }
}