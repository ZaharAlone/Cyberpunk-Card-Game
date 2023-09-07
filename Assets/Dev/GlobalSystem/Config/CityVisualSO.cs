using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using CyberNet.CoreGame.City;

namespace CyberNet
{
    [CreateAssetMenu(fileName = "CityVisualSO", menuName = "Scriptable Object/Board Game/City Visual SO")]
    public class CityVisualSO : SerializedScriptableObject
    {
        public Dictionary<string, UnitMono> UnitDictionary = new Dictionary<string, UnitMono>();
        public Dictionary<string, GameObject> UnitCityVFX = new Dictionary<string, GameObject>();
        public List<Color32> ColorEnemyStats = new();
    }
}