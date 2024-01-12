using CyberNet.Global;
using UnityEngine.Serialization;

namespace CyberNet.Core.Player
{
    public struct PlayerComponent
    {
        [FormerlySerializedAs("PlayerTypeEnum")]
        public PlayerOrAI playerOrAI;
        public int PlayerID;
        
        public int UnitCount;
        public int UnitAgentCountInHand;
        public int VictoryPoint;
        public int Cyberpsychosis;

        public int PositionInTurnQueue;
    }
}