using CyberNet.Global;
using UnityEngine.Serialization;

namespace CyberNet.Core.Player
{
    public struct PlayerComponent
    {
        public PlayerOrAI playerOrAI;
        public int PlayerID;
        
        public int UnitCount;
        public int CurrentCountControlTerritory;
        public int PointAbilityPlayerCurrent;
        public int PointAbilityPlayerMax;

        public int PositionInTurnQueue;
    }
}