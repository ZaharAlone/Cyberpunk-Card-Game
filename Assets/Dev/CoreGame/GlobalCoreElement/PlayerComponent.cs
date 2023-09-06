using CyberNet.Global;
using UnityEngine;

namespace CyberNet.Core
{
    public struct PlayerComponent
    {
        public PlayerType PlayerType;
        public int PlayerID;
        
        public int UnitCount;
        public int VictoryPoint;
        
        public string LeaderKey;
        public string Name;
        public Sprite Avatar;
    }
}