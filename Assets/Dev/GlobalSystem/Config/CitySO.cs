using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.Core.Arena;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    [CreateAssetMenu(fileName = "CitySO", menuName = "Scriptable Object/Board Game/City SO")]
    public class CitySO : SerializedScriptableObject
    {
        public Dictionary<string, UnitVisual> UnitDictionary = new Dictionary<string, UnitVisual>();
        public ParticleSystem TowerSelectVFX;
        public UnitPointVFXMono UnitPointVFXMono;
        public GameObject ClearSolidPointVFX;

        public TextAsset TowerConfig;

        public List<string> PlayerVisualKeyList = new();
    }

    [Serializable]
    public struct UnitVisual
    {
        public IconsUnitInMapMono IconsUnitMap;
        public UnitArenaMono UnitArenaMono;
        public Color32 ColorUnit;
    }
}