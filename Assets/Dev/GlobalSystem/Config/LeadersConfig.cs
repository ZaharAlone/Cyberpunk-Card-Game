using System;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace CyberNet
{
    [Serializable]
    public struct LeadersConfig
    {
        [JsonProperty("name")]
        public string Name;
        [FormerlySerializedAs("ImageCardHero")]
        [JsonProperty("image_card_hero")]
        public string ImageCardLeaders;
        [JsonProperty("image_button_hero")]
        public string ImageButtonLeader;
        [JsonProperty("image_avatar_hero")]
        public string ImageAvatarLeader;
        [JsonProperty("image_background_avatar_hero")]
        public string ImageBackgroundAvatarLeader;
        [JsonProperty("name_loc")]
        public string NameLoc;
        [JsonProperty("descr_loc")]
        public string DescrLoc;
        [JsonProperty("ability")]
        public string Ability;
    }
}