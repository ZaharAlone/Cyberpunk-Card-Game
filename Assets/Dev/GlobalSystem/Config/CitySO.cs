using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.Core.Battle.CutsceneArena;
using I2.Loc;
using UnityEngine.Serialization;

namespace CyberNet.Core.Map
{
    [CreateAssetMenu(fileName = "CitySO", menuName = "Scriptable Object/Board Game/City SO")]
    public class CitySO : SerializedScriptableObject
    {
        public IconsContainerUnitInMapMono IconsContainerUnitMap;
        public Dictionary<string, UnitVisual> UnitDictionary = new Dictionary<string, UnitVisual>();
        public ParticleSystem TowerSelectVFX;
        public GameObject ClearSolidPointVFX;

        public List<string> PlayerVisualKeyList = new();
        
        [Header("District config")]
        public TextAsset DistrictConfig;
        public LocalizedString DistrictClearLoc;
        public LocalizedString DistrictNeutralLoc;
    }

    [Serializable]
    public struct UnitVisual
    {
        public UnitArenaMono UnitArenaMono;
        public Sprite IconsUnit;
        public Color32 ColorUnit;
    }
}