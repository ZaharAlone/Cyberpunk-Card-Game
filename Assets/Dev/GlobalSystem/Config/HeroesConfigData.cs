using UnityEngine;
using System.Collections.Generic;

namespace CyberNet
{
    /// <summary>
    /// Все данные героев и абилок
    /// </summary>
    public struct HeroesConfigData
    {
        public Dictionary<string, HeroesConfig> HeroesConfig;
        public Dictionary<string, AbilityConfig> AbilityConfig;
    }
}