using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.Map
{
    public struct InitUnitStruct
    {
        public string KeyUnit;
        public UnitZoneMono UnitZone;
        public PlayerControlEntity PlayerControl;
        public int TargetPlayerID;
    }
}