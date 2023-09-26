using CyberNet.Global;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core
{
    public struct PlayerComponent
    {
        [FormerlySerializedAs("PlayerType")]
        public PlayerTypeEnum playerTypeEnum;
        public int PlayerID;
        
        public int UnitCount;
        public int UnitAgentCountInHand;
        public int VictoryPoint;
        public int Cyberpsychosis;

        public int PositionInTurnQueue;
    }
}