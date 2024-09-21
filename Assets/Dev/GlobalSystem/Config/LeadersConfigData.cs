using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace CyberNet
{
    /// <summary>
    /// Все данные героев и абилок
    /// </summary>
    public struct LeadersConfigData
    {
        [FormerlySerializedAs("HeroesConfig")]
        public Dictionary<string, LeadersConfig> LeadersConfig;
    }
}