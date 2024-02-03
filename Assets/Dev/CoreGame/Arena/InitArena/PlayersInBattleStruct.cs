using CyberNet.Core.City;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena
{
    public struct PlayersInBattleStruct
    {
        public int PlayerID;
        public PlayerControlEntity PlayerControlEntity;
        public bool Forwards;
    }
}