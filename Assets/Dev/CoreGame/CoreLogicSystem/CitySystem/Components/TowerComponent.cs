using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    public struct TowerComponent
    {
        public string GUID;
        public string Key;
        public TowerMono TowerMono;
        public GameObject TowerGO;
        public List<SquadZoneMono> SquadZonesMono;
        public ParticleSystem SelectTowerEffect;
        
        //Кому принадлежит контроль над башней?
        public PlayerControlEnum playerIsBelong;
        public int TowerBelongPlyaerID;
    }

    public enum PlayerControlEnum
    {
        None,
        Neutral,
        Player
    }
}