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
        public MeshRenderer VisualEffectZone;
        
        //Кому принадлежит контроль над башней?
        public PlayerControlEntity PlayerControlEntity;
        public int TowerBelongPlayerID;
    }

    public enum PlayerControlEntity
    {
        None,
        Neutral,
        Player
    }
}