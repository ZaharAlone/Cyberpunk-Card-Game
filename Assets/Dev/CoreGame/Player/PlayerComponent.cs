using CyberNet.Global;

namespace CyberNet.Core.Player
{
    public struct PlayerComponent
    {
        public PlayerTypeEnum PlayerTypeEnum;
        public int PlayerID;
        
        public int UnitCount;
        public int UnitAgentCountInHand;
        public int VictoryPoint;
        public int Cyberpsychosis;

        public int PositionInTurnQueue;
    }
}