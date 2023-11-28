using System.Collections.Generic;
using CyberNet.Core.City;

namespace CyberNet.Core.Arena
{
    public struct ArenaBattleData
    {
        public List<PlayerInBattle> PlayersInBattle;
    }

    public struct PlayerInBattle
    {
        public PlayerControlEnum PlayerControlEnum;
        public int PlayerID;
        public bool Forwards;
    }
}