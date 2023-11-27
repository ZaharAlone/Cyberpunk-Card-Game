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
        public List<UnitZoneMono> SquadZonesMono;
        [FormerlySerializedAs("SelectTowerEffect")]
        public MeshRenderer VisualEffectZone;
        
        //Кому принадлежит контроль над башней?
        [FormerlySerializedAs("playerIsBelong")]
        public PlayerControlEnum PlayerIsBelong;
        public int TowerBelongPlyaerID;
    }

    public enum PlayerControlEnum
    {
        None,
        Neutral,
        Player
    }
}