using CyberNet.Global;

namespace CyberNet.Core.Player
{
    public struct PlayerComponent
    {
        public PlayerOrAI playerOrAI;
        public int PlayerID;
        
        public int UnitCount;
        public int VictoryPoint;
        
        public int PositionInTurnQueue;
    }
}