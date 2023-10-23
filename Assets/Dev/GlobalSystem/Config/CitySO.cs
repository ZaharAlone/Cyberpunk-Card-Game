using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CyberNet.Core.City
{
    [CreateAssetMenu(fileName = "CitySO", menuName = "Scriptable Object/Board Game/City SO")]
    public class CitySO : SerializedScriptableObject
    {
        public Dictionary<string, UnitVisual> UnitDictionary = new Dictionary<string, UnitVisual>();
        public ParticleSystem TowerSelectVFX;
        [FormerlySerializedAs("SolidPointVFXMono")]
        public SquadPointVFXMono squadPointVFXMono;
        public GameObject ClearSolidPointVFX;

        public TextAsset TowerConfig;

        public List<string> PlayerVisualKeyList = new();
    }

    [Serializable]
    public struct UnitVisual
    {
        public GameObject IconsUnitMap;
        public SquadMono SquadMono;
        public Color32 ColorUnit;
    }
}