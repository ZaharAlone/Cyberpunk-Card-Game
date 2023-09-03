using System.Collections.Generic;
using UnityEngine;
namespace CyberNet.CoreGame.City
{
    public struct TowerComponent
    {
        public string GUID;
        public GameObject TowerGO;
        //Кому принадлежит контроль над башней?
        public PlayerEnum PowerTowerBelong;
        //Контроль полный?
        public bool IsFullPower;
        public List<SolidPointMono> SolidPointMono;
    }
}