using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Map
{
    public struct UnitMapComponent
    {
        //GUID башни где стоит юнит
        public string GUIDTower;
        //Индекс пойнта в башне
        public int IndexPoint;
        //GUID юнита
        public string GUIDUnit;

        public GameObject UnitIconsGO;
        public IconsContainerUnitInMapMono IconsUnitInMapMono; 
        
        //Кому принадлежит данный юнит
        public PlayerControlEntity PlayerControl;
        public int PowerSolidPlayerID;
    }
}