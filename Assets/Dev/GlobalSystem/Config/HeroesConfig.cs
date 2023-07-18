using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace CyberNet
{
    [Serializable]
    public struct HeroesConfig
    {
        public Dictionary<string, HeroesConfigJson> Heroes;
    }

    [Serializable]
    public struct HeroesConfigJson
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("image_card_hero")]
        public string ImageCardHero;
        [JsonProperty("image_button_hero")]
        public string ImageButtonHero;
        [JsonProperty("image_avatar_hero")]
        public string imageAvatarHero;
        [JsonProperty("name_loc")]
        public string NameLoc;
        [JsonProperty("descr_loc")]
        public string DescrLoc;
        
    }
}