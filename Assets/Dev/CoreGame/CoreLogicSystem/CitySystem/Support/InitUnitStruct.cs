using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.City
{
    public struct InitUnitStruct
    {
        public string KeyUnit;
        public SquadZoneMono SquadZone;
        public PlayerControlEnum PlayerControl;
        public int TargetPlayerID;
    }
}