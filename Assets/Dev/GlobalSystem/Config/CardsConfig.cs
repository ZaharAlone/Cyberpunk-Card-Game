using System;
using System.Collections.Generic;
using CyberNet.Core.AbilityCard;
using Newtonsoft.Json;

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
        [JsonProperty("card_type")]
        public CardType CardType;
        [JsonProperty("price")]
        public int Price;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("value_left_point")]
        public int ValueLeftPoint;
        [JsonProperty("value_right_point")]
        public int ValueRightPoint;
        [JsonProperty("ability_0")]
        public AbilityCardContainer Ability_0;
        [JsonProperty("ability_1")]
        public AbilityCardContainer Ability_1;
    }

    [Serializable]
    public struct AbilityCardContainer
    {
        [JsonProperty("action")]
        public AbilityType AbilityType;
        [JsonProperty("count")]
        public int Count;
    }
    
    [Serializable]
    public enum CardType
    {
        None,
        StartDeck,
        TradeRow,
        Ability,
    }

    [Serializable]
    public struct AbilityCardConfig
    {
        [JsonProperty("abilityLoc")]
        public string AbilityLoc;
        [JsonProperty("abilityLocMany")]
        public string abilityLocMany;
        [JsonProperty("visualPlayingCard")]
        public VisualPlayingCardType VisualPlayingCard;
        [JsonProperty("selectFrame_headerLoc")]
        public string SelectFrameHeader;
        [JsonProperty("selectFrame_descrLoc")]
        public string SelectFrameDescr;
        [JsonProperty("selectFrame_descrLoc_2")]
        public string SelectFrameDescr_2;
    }

    [Serializable]
    public enum VisualPlayingCardType
    {
        None,
        Table,
        Target,
        Battle,
    }
}