using CyberNet.Global;
namespace CyberNet.Core
{
    public struct RoundData
    {
        public int CurrentRound;
        public int CurrentTurn;
        public int CurrentPlayerID;
        public PlayerType PlayerType;
        public bool EndPreparationRound;
    }
}