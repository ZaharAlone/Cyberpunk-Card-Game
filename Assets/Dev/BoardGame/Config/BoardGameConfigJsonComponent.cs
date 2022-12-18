using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace BoardGame
{
    public struct BoardGameConfigJsonComponent
    {
        public List<CardStats> CardConfig;
    }

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

        [JsonProperty("primary")]
        public CurrencyCard Primary;
        [JsonProperty("ally")]
        public CurrencyCard Ally;
        [JsonProperty("scrap")]
        public CurrencyCard Scrap;
    }

    public struct CurrencyCard
    {
        [JsonProperty("type")]
        public string Type;
        [JsonProperty("value")]
        public int Value;
        //[JsonProperty("parameter")]
        //public string Parameter;
    }
}