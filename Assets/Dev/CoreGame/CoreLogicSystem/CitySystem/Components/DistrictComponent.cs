using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace CyberNet.Core.Map
{
    public struct DistrictComponent
    {
        public string GUID;
        public string Key;
        public DistrictMono DistrictMono;
        public GameObject TowerGO;
        public List<UnitZoneMono> SquadZonesMono;
        public MeshRenderer VisualEffectZone;
        
        //Кому принадлежит контроль над районом?
        public PlayerControlEntity PlayerControlEntity;
        public int DistrictBelongPlayerID;

        public ItemValue BonusDistrict;
    }

    public enum PlayerControlEntity
    {
        None,
        NeutralUnits,
        PlayerControl
    }
}