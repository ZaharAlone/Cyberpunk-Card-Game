using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct AbilityConfig
    {
        public Dictionary<string, AbilityConfigJson> Ability;
    }

    [Serializable]
    public struct AbilityConfigJson
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("energy_one_use")]
        public string EnergyOneUse;
        [JsonProperty("max_energy")]
        public string MaxEnergy;
        [JsonProperty("value")]
        public string Value;
        [JsonProperty("image_ability")]
        public string ImageAbility;

    }
}