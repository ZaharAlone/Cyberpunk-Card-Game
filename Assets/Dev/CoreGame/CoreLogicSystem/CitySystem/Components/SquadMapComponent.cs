using UnityEngine;

namespace CyberNet.Core.City
{
    public struct SquadMapComponent
    {
        public string GUIDPoint;
        public int IndexPoint;

        public GameObject UnitIconsGO; 
        
        //Кому принадлежит данный юнит
        public PlayerControlEnum PlayerControl;
        public int PowerSolidPlayerID;
    }
}