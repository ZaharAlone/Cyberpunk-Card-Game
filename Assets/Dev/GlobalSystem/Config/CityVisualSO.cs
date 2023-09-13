using System;
using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace CyberNet.Core.City
{
    [CreateAssetMenu(fileName = "CityVisualSO", menuName = "Scriptable Object/Board Game/City Visual SO")]
    public class CityVisualSO : SerializedScriptableObject
    {
        public Dictionary<string, UnitVisual> UnitDictionary = new Dictionary<string, UnitVisual>();
        public ParticleSystem TowerSelectVFX;
        public SolidPointVFXMono SolidPointVFXMono;
        public GameObject ClearSolidPointVFX;

        public List<string> PlayerVisualKeyList = new();
    }

    [Serializable]
    public struct UnitVisual
    {
        public UnitMono UnitMono;
        public Color32 ColorUnit;
    }
}