using UnityEngine;

namespace CyberNet.Core.City
{
    public struct UnitComponent
    {
        public string GUIDPoint;
        public int IndexPoint;

        public GameObject UnitGO;
        public UnitMono UnitMono;
        
        //Кому принадлежит данный юнит
        public PlayerControlEnum PlayerControl;
        public int PowerSolidPlayerID;
    }
}