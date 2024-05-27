using CyberNet.Global;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        [FormerlySerializedAs("CurrentMapArenaState")]
        [FormerlySerializedAs("CurrentRoundState")]
        public GameStateMapVSArena CurrentGameStateMapVSArena;
        public PlayerOrAI playerOrAI;
        public bool PauseInteractive;
    }
}