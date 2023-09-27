using System;
using System.Collections;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.Serialization;

namespace CyberNet
{
    [Serializable]
    public struct CardsConfig
    {
        public Dictionary<string, CardConfigJson> Cards;
        public Dictionary<string, AbilityCardConfig> AbilityCard;
    }

    [Serializable]
    public struct CardConfigJson
    {
        [JsonProperty("name")]
        public string Name;
        [JsonProperty("header")]
        public string Header;
        [JsonProperty("descr")]
        public string Descr;

        [JsonProperty("image")]
        public string ImageKey;
        [JsonProperty("nations")]
        public string Nations;
        [JsonProperty("cyberpsychosis_count")]
        public int CyberpsychosisCount;
        [JsonProperty("price")]
        public int Price;
        [JsonProperty("count")]
        public int Count;

        [JsonProperty("ability_0")]
        public AbilityCardContainer Ability_0;
        [JsonProperty("ability_1")]
        public AbilityCardContainer Ability_1;
        [JsonProperty("ability_2")]
        public AbilityCardContainer Ability_2;
    }

    [Serializable]
    public struct AbilityCardContainer
    {
        [JsonProperty("action")]
        public AbilityType AbilityType;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("condition")]
        public AbilityCondition Condition;
    }

    [Serializable]
    public enum AbilityCondition
    {
        None,
        Corporates,
        Guns,
        Nomads,
        Netrunners,
        Destroy
    }

    [Serializable]
    public struct AbilityCardConfig
    {
        [JsonProperty("abilityLoc")]
        public string AbilityLoc;
        [JsonProperty("selectFrame_headerLoc")]
        public string SelectFrameHeader;
        [JsonProperty("selectFrame_descrLoc")]
        public string SelectFrameDescr;
    }
}