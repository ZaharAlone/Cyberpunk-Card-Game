using CyberNet.Core.City;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Arena
{
    public struct ArenaUnitComponent
    {
        public string GUID;
        public GameObject UnitGO;
        public UnitArenaMono UnitArenaMono;
        public PlayerControlEntity PlayerControlEntity;
        public int PlayerControlID;

        public bool IsActionCurrentRound;
        public int IndexTurnOrder;
    }
}