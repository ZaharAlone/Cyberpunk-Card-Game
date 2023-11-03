using System;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct CardPopupConfig
    {
        [JsonProperty("is_popup")]
        public bool IsPoput;
        [JsonProperty("ab_descr_loc")]
        public string AbilityDescrLoc;
        [JsonProperty("artistic_text_descr_loc")]
        public string ArtisticDescrLoc;
    }
}