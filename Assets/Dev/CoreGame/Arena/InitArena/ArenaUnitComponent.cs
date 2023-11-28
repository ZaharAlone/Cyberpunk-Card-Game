using CyberNet.Core.City;
using UnityEngine;

namespace CyberNet.Core.Arena
{
    public struct ArenaUnitComponent
    {
        public string GUID;
        public GameObject UnitGO;
        public UnitArenaMono UnitArenaMono;
        public PlayerControlEnum PlayerControlEnum;
        public int PlayerControlID;
    }
}