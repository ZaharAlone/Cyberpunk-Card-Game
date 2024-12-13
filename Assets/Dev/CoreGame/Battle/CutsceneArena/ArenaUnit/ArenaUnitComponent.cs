using CyberNet.Core.Map;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Battle.CutsceneArena
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