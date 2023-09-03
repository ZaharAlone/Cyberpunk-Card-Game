using UnityEngine;

namespace CyberNet.CoreGame.City
{
    public struct UnitComponent
    {
        public string GUIDPoint;
        public int IndexPoint;

        public GameObject UnitGO;
        public UnitMono UnitMono;
        //Кому принадлежит данный пойнт
        public PlayerEnum PowerSolid;
    }
}