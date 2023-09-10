using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace CyberNet.Core.City
{
    [CreateAssetMenu(fileName = "CityVisualSO", menuName = "Scriptable Object/Board Game/City Visual SO")]
    public class CityVisualSO : SerializedScriptableObject
    {
        public Dictionary<string, UnitMono> UnitDictionary = new Dictionary<string, UnitMono>();
        public Dictionary<string, GameObject> UnitCityVFX = new Dictionary<string, GameObject>();
        public List<Color32> ColorEnemyStats = new();
        public ParticleSystem TowerSelectVFX;
    }
}