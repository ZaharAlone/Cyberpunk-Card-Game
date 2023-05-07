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

        [JsonProperty("ability_0")]
        public AbilityCard Ability_0;
        [JsonProperty("ability_1")]
        public AbilityCard Ability_1;
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
        attack,
        trade,
        influence,
        drawCard,
        discardCard,
        destroyCard,
        upСyberpsychosis,
        downCyberpsychosis,
        cloneCard,
        noiseCard,
        thiefCard
    }

    [Serializable]
    public enum AbilityCondition
    {
        None,
        cyberpsychosis_5,
        cyberpsychosis_10,
        cyberpsychosis_15,
        doubleCorporates,
        doubleGuns,
        doubleNomads,
        doubleNetrunners,
        destroyCard,
    }
}