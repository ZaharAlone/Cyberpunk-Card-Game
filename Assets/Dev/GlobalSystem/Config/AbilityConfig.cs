using System;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct AbilityConfig
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
        [JsonProperty("ability_loc_name")]
        public string NameLoc;
        [JsonProperty("ability_loc_descr")]
        public string DescrLoc;
    }
}