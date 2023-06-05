using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace CyberNet
{
    [Serializable]
    public struct CardsConfig
    {
        public Dictionary<string, CardConfig> Cards;
    }

    [Serializable]
    public struct CardConfig
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
        [JsonProperty("type")]
        public TypeCard Type;
        [JsonProperty("shield")]
        public int Shield;

        [JsonProperty("ability_0")]
        public AbilityCard Ability_0;
        [JsonProperty("ability_1")]
        public AbilityCard Ability_1;
        [JsonProperty("ability_2")]
        public AbilityCard Ability_2;
    }

    [Serializable]
    public struct AbilityCard
    {
        [JsonProperty("action")]
        public AbilityAction Action;
        [JsonProperty("count")]
        public int Count;
        [JsonProperty("condition")]
        public AbilityCondition Condition;
    }

    [Serializable]
    public enum AbilityAction
    {
        None,
        Attack,
        Trade,
        Influence,
        DrawCard,
        DiscardCardEnemy,
        DestroyCard,
        DownCyberpsychosisEnemy,
        CloneCard,
        NoiseCard,
        ThiefCard,
        DestroyTradeCard,
        DestroyEnemyBase
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
    public enum TypeCard
    {
        Unit,
        Base
    }
}