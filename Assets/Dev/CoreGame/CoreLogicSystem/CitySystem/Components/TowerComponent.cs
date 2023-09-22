using System.Collections.Generic;
using UnityEngine;

namespace CyberNet.Core.City
{
    public struct TowerComponent
    {
        public string GUID;
        public string Key;
        public TowerMono TowerMono;
        public GameObject TowerGO;
        public List<SolidPointMono> SolidPointMono;
        public ParticleSystem SelectTowerEffect;
        
        //Кому принадлежит контроль над башней?
        public PlayerControlEnum playerIsBelong;
        public int TowerBelongPlyaerID;
        //Контроль полный?
        public bool IsFullTowerControl;
    }

    public enum PlayerControlEnum
    {
        None,
        Neutral,
        Player
    }
}