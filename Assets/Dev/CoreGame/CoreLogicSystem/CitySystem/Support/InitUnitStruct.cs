using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.City
{
    public struct InitUnitStruct
    {
        public string KeyUnit;
        [FormerlySerializedAs("SquadZone")]
        public UnitZoneMono UnitZone;
        public PlayerControlEnum PlayerControl;
        public int TargetPlayerID;
    }
}