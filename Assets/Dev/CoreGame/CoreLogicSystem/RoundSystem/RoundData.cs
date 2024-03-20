using CyberNet.Global;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        public RoundState CurrentRoundState;
        public PlayerOrAI playerOrAI;
        public bool PauseInteractive;
    }
}