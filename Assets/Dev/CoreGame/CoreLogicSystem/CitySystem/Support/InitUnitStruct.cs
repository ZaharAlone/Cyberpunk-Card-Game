using UnityEngine;
using UnityEngine.Serialization;
namespace CyberNet.Core.City
{
    public struct InitUnitStruct
    {
        public string KeyUnit;
        [FormerlySerializedAs("SolidPoint")]
        public SquadPointMono squadPoint;
        public PlayerControlEnum PlayerControl;
        public int TargetPlayerID;
    }
}