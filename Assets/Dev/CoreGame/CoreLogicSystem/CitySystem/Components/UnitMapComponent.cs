using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public struct UnitMapComponent
    {
        //GUID башни где стоит юнит
        [FormerlySerializedAs("GUIDPoint")]
        public string GUIDTower;
        //Индекс пойнта в башне
        public int IndexPoint;
        //GUID юнита
        public string GUIDUnit;

        public GameObject UnitIconsGO;
        public IconsUnitInMapMono IconsUnitInMapMono; 
        
        //Кому принадлежит данный юнит
        public PlayerControlEnum PlayerControl;
        public int PowerSolidPlayerID;
    }
}