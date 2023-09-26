using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public struct SquadComponent
    {
        public string GUIDPoint;
        public int IndexPoint;

        public GameObject SquadGO;
        public SquadMono SquadMono;
        
        //Кому принадлежит данный юнит
        public PlayerControlEnum PlayerControl;
        public int PowerSolidPlayerID;
    }
}