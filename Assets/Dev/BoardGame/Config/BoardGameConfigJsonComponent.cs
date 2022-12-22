using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BoardGame
{
    [System.Serializable]
    public struct BoardGameConfigJsonComponent
    {
        public List<CardStats> CardConfig;
    }

    [System.Serializable]
    public struct CardStats
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("header")]
        public string Header;
        [JsonProperty("descr_mini")]
        public string Descr_mini;
        [JsonProperty("descr")]
        public string Descr;

        [JsonProperty("image")]
        public string ImageKey;
        [JsonProperty("nations")]
        public string Nations;
        [JsonProperty("price")]
        public int Price;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("type")]
        public string Type;

        [JsonProperty("ability")]
        public AbilityCard Ability;
        [JsonProperty("fractions_ability")]
        public AbilityCard FractionsAbility;
        [JsonProperty("drop_ability")]
        public AbilityCard DropAbility;
    }

    [System.Serializable]
    public struct AbilityCard
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("value")]
        public int Value;
        //[JsonProperty("parameter")]
        //public string Parameter;
    }
}